using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using MusicPlayApp.BLL.Service;
using MusicPlayApp.DLL.Entities;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation; // Để sử dụng DoubleAnimation



namespace MusicPlayList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer timer;
        private List<Song> playlist = new List<Song>();
        private bool isShuffle = false;
        private string[] files;
        private string[] paths;






        private SongService _songService = new SongService();


        public MusicPlayApp.DLL.Entities.User CurrentUser { get; set; }

        private bool isFullScreen = false; // Thêm biến theo dõi trạng thái full screen

        public MainWindow(MusicPlayApp.DLL.Entities.User currentUser)
        {
            InitializeComponent();
            CurrentUser = currentUser;
            if (CurrentUser == null)
            {
                MessageBox.Show("User not authenticated. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            InitializePlayer();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.MouseLeftButtonDown += MediaPlayer_MouseLeftButtonDown;
            this.KeyDown += Window_KeyDown; // Thêm xử lý phím ESC
            volumeSlider.Value = 100;
            VolumeText.Text = "100%";
            LoadTitleAllSongs();
            InitializeFavoriteList();
        }

        // Xử lý double click bằng đếm thời gian giữa các lần click
        private DateTime lastClick = DateTime.MinValue;
        private void MediaPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((DateTime.Now - lastClick).TotalMilliseconds < 300) // Double click threshold
            {
                ToggleFullScreen();
                e.Handled = true;
            }
            lastClick = DateTime.Now;
        }



        // Phương thức bất đồng bộ để tải danh sách yêu thích
        private async void InitializeFavoriteList()
        {
            try
            {
                if (CurrentUser != null)
                {
                    int userId = CurrentUser.UserId;
                    FavoriteService favoriteService = new FavoriteService();

                    // Get the list of favorite songs
                    var favoriteSongs = await favoriteService.GetFavoritesByUserIdAsync(userId);

                    // Populate the favoriteList with the fetched songs
                    favoriteList = favoriteSongs;

                    // Clear and reload the FavoriteListBox
                    FavoriteListBox.Items.Clear();
                    foreach (var song in favoriteList)
                    {
                        FavoriteListBox.Items.Add(song);  // Add the song to the FavoriteListBox
                    }
                }
                else
                {
                    MessageBox.Show("User not authenticated. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading favorite songs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializePlayer()
        {
            mediaPlayer.Volume = volumeSlider.Value;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            CDImage.Visibility = Visibility.Hidden;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeCount.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                TimeCount.Value = mediaPlayer.Position.TotalSeconds;

                currentTimeText.Text = mediaPlayer.Position.ToString(@"mm\:ss");
                totalTimeText.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
            else
            {
                // Đặt giá trị mặc định khi Duration chưa sẵn sàng
                TimeCount.Value = 0;
                currentTimeText.Text = "00:00";
                totalTimeText.Text = "00:00";
            }
        }



        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            AddSongWindow addSong = new AddSongWindow();
            addSong.ShowDialog();
            LoadTitleAllSongs();

        }




        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            //if (playlistListBox.SelectedItem != null)
            //{
            //    var selectedTitle = playlistListBox.SelectedItem.ToString();
            //    var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

            //    if (selectedSong != null)
            //    {
            //        var filePath = selectedSong.Album;

            //        // Check if the file exists
            //        if (File.Exists(filePath))
            //        {
            //            mediaPlayer.Source = new Uri(filePath);
            //            mediaPlayer.Play();
            //            timer.Start();
            //        }
            //        else
            //        {
            //            MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Selected song not found in the playlist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please select a song from the playlist.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }



        private bool isPlaying = true;

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                mediaPlayer.Pause();
                isPlaying = false;
                PauseButton.Content = "Continue";
                timer.Stop(); // Dừng timer để ngừng quay đĩa
            }
            else
            {
                mediaPlayer.Play();
                isPlaying = true;
                PauseButton.Content = "Pause";
                timer.Start(); // Bắt đầu lại timer để quay đĩa
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            if (timer != null)
            {
                timer.Stop();
            }
            TimeCount.Value = 0;
            currentTimeText.Text = "00:00";
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Volume = volumeSlider.Value / 100; // Chuyển đổi giá trị từ 0-100 về 0-1
            }

            if (VolumeText != null)
            {
                VolumeText.Text = $"{volumeSlider.Value:0}%";
            }
        }


        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            isShuffle = true;
            //PLayMode.Content = "Shuffle";  // Hiển thị chế độ Shuffle
            MessageBox.Show("Shuffle mode activated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SequentialButton_Click(object sender, RoutedEventArgs e)
        {
            isShuffle = false;
            //PLayMode.Content = "Sequential";
            MessageBox.Show("Sequential mode activated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    if (isShuffle)
        //    {
        //        var random = new Random();
        //        playlistListBox.SelectedIndex = random.Next(playlist.Count);
        //    }
        //    else
        //    {
        //        if (playlistListBox.SelectedIndex < playlist.Count - 1)
        //            playlistListBox.SelectedIndex++;
        //        else
        //            playlistListBox.SelectedIndex = 0;
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isPlayingFromFavoriteList)
            {
                if (favoriteList.Count == 0)
                {
                    MessageBox.Show("Favorite list is empty.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Di chuyển đến bài tiếp theo trong favorite list
                int currentIndex = FavoriteListBox.SelectedIndex;
                FavoriteListBox.SelectedIndex = (currentIndex < favoriteList.Count - 1) ? currentIndex + 1 : 0;

                PlaySelectedSong(favoriteList, FavoriteListBox);
            }
            else if (isPlayingFromPlaylist)
            {
                if (playlist.Count == 0)
                {
                    MessageBox.Show("Playlist is empty.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Di chuyển đến bài tiếp theo trong playlist
                int currentIndex = playlistListBox.SelectedIndex;
                playlistListBox.SelectedIndex = (currentIndex < playlist.Count - 1) ? currentIndex + 1 : 0;

                PlaySelectedSong(playlist, playlistListBox);
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (isPlayingFromFavoriteList)
            {
                if (favoriteList.Count == 0)
                {
                    MessageBox.Show("Favorite list is empty.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Di chuyển đến bài trước đó trong favorite list
                int currentIndex = FavoriteListBox.SelectedIndex;
                FavoriteListBox.SelectedIndex = (currentIndex > 0) ? currentIndex - 1 : favoriteList.Count - 1;

                PlaySelectedSong(favoriteList, FavoriteListBox);
            }
            else if (isPlayingFromPlaylist)
            {
                if (playlist.Count == 0)
                {
                    MessageBox.Show("Playlist is empty.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Di chuyển đến bài trước đó trong playlist
                int currentIndex = playlistListBox.SelectedIndex;
                playlistListBox.SelectedIndex = (currentIndex > 0) ? currentIndex - 1 : playlist.Count - 1;

                PlaySelectedSong(playlist, playlistListBox);
            }
        }

        private void PlaySelectedSong(List<Song> songList, ListBox listBox)
        {
            if (listBox.SelectedItem != null)
            {
                var selectedTitle = listBox.SelectedItem.ToString();
                var selectedSong = songList.FirstOrDefault(s => s.Title == selectedTitle);

                if (selectedSong != null)
                {
                    var filePath = selectedSong.Album;

                    if (File.Exists(filePath))
                    {
                        mediaPlayer.Source = new Uri(filePath);
                        mediaPlayer.Play();
                        timer.Start();
                        txtText.Text = selectedSong.Title;
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
              
            }
            else
            {
                MessageBox.Show("Please select a song from the list.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void TimeCount_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mouseX = e.GetPosition(TimeCount).X;
            var percentage = mouseX / TimeCount.ActualWidth;
            var newTime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds * percentage;

            // Update the position of the media player based on the slider's new value
            mediaPlayer.Position = TimeSpan.FromSeconds(newTime);
        }

        private void TimeCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                // Sync the slider with the media player position
                mediaPlayer.Position = TimeSpan.FromSeconds(TimeCount.Value);
            }
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
           

        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private bool isPlayingFromFavoriteList = false;  
        private bool isPlayingFromPlaylist = false;


        private void FavoriteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoriteListBox.SelectedItem != null)
            {
                isPlayingFromFavoriteList = true;  // Mark as playing from Favorite List
                isPlayingFromPlaylist = false;    // Stop playing from Playlist

                // Reset the previous selection
                playlistListBox.SelectedItem = null;

                var selectedSong = FavoriteListBox.SelectedItem as Song;
                if (selectedSong != null)
                {
                    var album = selectedSong.Album;
                    txtText.Text = selectedSong.Title;

                    if (File.Exists(album))
                    {
                        string fileExtension = System.IO.Path.GetExtension(album).ToLower();

                        if (fileExtension == ".mp3" || fileExtension == ".wav")
                        {
                            // Audio file: Show disk and play audio
                            cdContainer.Visibility = Visibility.Visible;  // Show disk container
                            CDImage.Visibility = Visibility.Visible;  // Show rotating disk
                            mediaPlayer.Visibility = Visibility.Collapsed;  // Hide video player
                            StartRotatingDisk();  // Start rotating disk

                            // Play the audio
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                        }
                        else
                        {
                            cdContainer.Visibility = Visibility.Collapsed;  // Hide disk container
                            CDImage.Visibility = Visibility.Collapsed;  // Hide rotating disk
                            mediaPlayer.Visibility = Visibility.Visible;  // Show video player
                            StopRotatingDisk(); // Stop disk rotation

                            // Play the video or other media
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }






        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Handle deletion from Playlist
                if (playlistListBox.SelectedItem != null)
                {
                    // Reset FavoriteListBox selection to avoid conflicts
                    FavoriteListBox.SelectedItem = null;

                    var selectedTitle = playlistListBox.SelectedItem.ToString();
                    var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                    if (selectedSong != null)
                    {
                        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bài hát '{selectedSong.Title}' khỏi playlist?",
                                                      "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Call service to remove the song from Playlist
                            _songService.RemoveSong(selectedSong);
                            MessageBox.Show($"Bài hát '{selectedSong.Title}' đã được xóa khỏi playlist.",
                                             "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Reload the Playlist
                            playlistListBox.ItemsSource = null;
                            LoadTitleAllSongs();

                            // Reload FavoriteListBox to reflect changes immediately
                            FavoriteListBox.ItemsSource = null;  // Clear the existing items
                            await LoadFavoriteList();  // Load updated favorite list
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy bài hát được chọn trong playlist.",
                                         "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                // Handle deletion from Favorite List
                else if (FavoriteListBox.SelectedItem != null)
                {
                    // Reset playlistListBox selection to avoid conflicts
                    playlistListBox.SelectedItem = null;

                    var selectedSong = FavoriteListBox.SelectedItem as Song;

                    if (selectedSong != null && CurrentUser != null)
                    {
                        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bài hát '{selectedSong.Title}' khỏi danh sách yêu thích?",
                                                      "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Call service to remove the song from Favorite List
                            FavoriteService favoriteService = new FavoriteService();
                            await favoriteService.RemoveFavoriteAsync(CurrentUser.UserId, selectedSong.SongId);
                            MessageBox.Show($"Bài hát '{selectedSong.Title}' đã được xóa khỏi danh sách yêu thích.",
                                             "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Reload the Favorite List
                            FavoriteListBox.ItemsSource = null;
                            await LoadFavoriteList();  // Reload updated favorite list
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy bài hát được chọn trong danh sách yêu thích.",
                                         "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một bài hát từ playlist hoặc danh sách yêu thích.",
                                     "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                MessageBox.Show($"Đã xảy ra lỗi khi xóa bài hát: {ex.Message}",
                                 "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }







        private async void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                // Lấy bài hát được chọn
                var selectedTitle = playlistListBox.SelectedItem.ToString();
                var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                if (selectedSong != null)
                {
                    int songId = selectedSong.SongId;

                    // Kiểm tra người dùng hiện tại
                    if (CurrentUser != null)
                    {
                        int userId = CurrentUser.UserId; // Lấy UserId từ người dùng đã đăng nhập

                        try
                        {
                            // Kiểm tra xem bài hát đã có trong danh sách yêu thích của người dùng chưa
                            var favoriteService = new FavoriteService();
                            var existingFavorite = await favoriteService.CheckIfFavoriteExistsAsync(userId, songId);

                            if (existingFavorite)
                            {
                                // Nếu bài hát đã có trong danh sách yêu thích, thông báo cho người dùng
                                MessageBox.Show("Bài hát này đã có trong danh sách yêu thích của bạn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                // Gọi service để thêm bài hát vào danh sách yêu thích
                                await favoriteService.AddFavoriteAsync(userId, songId, "Danh sách yêu thích của tôi");

                                // Load lại danh sách yêu thích
                                await LoadFavoriteList();

                                MessageBox.Show("Bài hát đã được thêm vào danh sách yêu thích.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Xử lý lỗi nếu có
                            MessageBox.Show($"Đã xảy ra lỗi khi thêm bài hát vào danh sách yêu thích: {ex.Message}",
                                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng đăng nhập để thêm bài hát vào danh sách yêu thích.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Không có bài hát nào được chọn để thêm vào danh sách yêu thích.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bài hát từ playlist.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private async Task LoadFavoriteList()
        {
            if (CurrentUser != null)
            {
                try
                {
                    int userId = CurrentUser.UserId;

                    FavoriteService favoriteService = new FavoriteService();

                    // Lấy danh sách bài hát yêu thích
                    var favoriteSongs = await favoriteService.GetFavoritesByUserIdAsync(userId);

                    // Làm sạch danh sách trước khi thêm mới
                    FavoriteListBox.Items.Clear();

                    // Hiển thị các bài hát yêu thích
                    foreach (var song in favoriteSongs)
                    {
                        FavoriteListBox.Items.Add(song);  // Thêm đối tượng Song vào FavoriteListBox
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading favorite songs: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("User not authenticated. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void playlistListBox_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        //public async void LoadUserSongs()
        //{
        //    var playlist = await _songService.GetSongsByUserIdAsync(CurrentUser.UserId);
        //    foreach (var song in songs)
        //    {
        //        playlist.Add(song.Title);
        //        playlistListBox.Items.Add(song.Title);
        //    }
        //    if(songs == null)
        //    {
        //        return;
        //    }
        //}

        public async void LoadTitleAllSongs()
        {
            try
            {
                // Lấy danh sách tất cả các bài hát từ service
                playlist = await _songService.GetAllSongsAsync();

                // Làm sạch danh sách trước khi thêm mới
                playlistListBox.Items.Clear();

                // Thêm các bài hát vào playlistListBox
                foreach (var song in playlist)
                {
                    playlistListBox.Items.Add(song.Title);  // Chỉ thêm tiêu đề bài hát
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading songs: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                isPlayingFromPlaylist = true;  // Mark as playing from Playlist
                isPlayingFromFavoriteList = false;  // Stop playing from Favorite List

                // Reset the previous selection
                FavoriteListBox.SelectedItem = null;

                var selectedTitle = playlistListBox.SelectedItem.ToString();
                var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                if (selectedSong != null)
                {
                    var album = selectedSong.Album;
                    txtText.Text = selectedSong.Title;

                    if (File.Exists(album))
                    {
                        string fileExtension = System.IO.Path.GetExtension(album).ToLower();
                        if (fileExtension == ".mp3" || fileExtension == ".wav")
                        {
                            // Audio file: Show disk and play audio
                            cdContainer.Visibility = Visibility.Visible;  // Show disk container
                            CDImage.Visibility = Visibility.Visible;  // Show rotating disk
                            mediaPlayer.Visibility = Visibility.Collapsed;  // Hide video player
                            StartRotatingDisk();  // Start rotating disk

                            // Play the audio
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                        }
                        else
                        {
                            // Non-audio file (video): Show video and hide the disk
                            cdContainer.Visibility = Visibility.Collapsed;  // Hide disk container
                            CDImage.Visibility = Visibility.Collapsed;  // Hide rotating disk
                            mediaPlayer.Visibility = Visibility.Visible;  // Show video player
                            StopRotatingDisk(); // Stop disk rotation

                            // Play the video or other media
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private void StopRotatingDisk()
        {
            if (rotateTransform != null)
            {
                rotateTransform.Angle = 0;
            }
        }






        private List<Song> favoriteList = new List<Song>(); // Ensure this is initialized

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (isShuffle)
            {
                if (isPlayingFromFavoriteList)
                {
                    var random = new Random();
                    int currentIndex = favoriteList.FindIndex(s => s.Title == txtText.Text);  // Get current song index in favorite list
                    int nextSongIndex = random.Next(favoriteList.Count);  // Pick a random song from Favorite List

                    // Ensure that the random song is not the current one
                    while (nextSongIndex == currentIndex)
                    {
                        nextSongIndex = random.Next(favoriteList.Count);
                    }

                    var nextSong = favoriteList[nextSongIndex];
                    var album = nextSong.Album;
                    txtText.Text = nextSong.Title;

                    if (File.Exists(album))
                    {
                        mediaPlayer.Source = new Uri(album);
                        mediaPlayer.Play();
                        timer.Start();

                        // Update the selected item in Favorite List
                        FavoriteListBox.SelectedItem = nextSong;
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (isPlayingFromPlaylist)
                {
                    var random = new Random();
                    int currentIndex = playlist.FindIndex(s => s.Title == txtText.Text);  // Get current song index in playlist
                    int nextSongIndex = random.Next(playlist.Count);  // Pick a random song from Playlist

                    // Ensure that the random song is not the current one
                    while (nextSongIndex == currentIndex)
                    {
                        nextSongIndex = random.Next(playlist.Count);
                    }

                    var nextSong = playlist[nextSongIndex];
                    var album = nextSong.Album;
                    txtText.Text = nextSong.Title;

                    if (File.Exists(album))
                    {
                        mediaPlayer.Source = new Uri(album);
                        mediaPlayer.Play();
                        timer.Start();

                        // Update the selected item in Playlist
                        playlistListBox.SelectedItem = nextSong;
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                // Sequential Mode (one song after another)
                if (isPlayingFromFavoriteList)
                {
                    int currentIndex = favoriteList.FindIndex(s => s.Title == txtText.Text);

                    if (currentIndex >= 0 && currentIndex < favoriteList.Count - 1)
                    {
                        var nextSong = favoriteList[currentIndex + 1];
                        var album = nextSong.Album;
                        txtText.Text = nextSong.Title;

                        if (File.Exists(album))
                        {
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();

                            // Update the selected item in Favorite List
                            FavoriteListBox.SelectedItem = nextSong;
                        }
                        else
                        {
                            MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Reached the end of Favorite List, go back to the first song
                        var firstSong = favoriteList[0];
                        var album = firstSong.Album;
                        txtText.Text = firstSong.Title;

                        if (File.Exists(album))
                        {
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();

                            // Update the selected item to the first song in Favorite List
                            FavoriteListBox.SelectedItem = firstSong;
                        }
                        else
                        {
                            MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else if (isPlayingFromPlaylist)
                {
                    int currentIndex = playlist.FindIndex(s => s.Title == txtText.Text);

                    if (currentIndex >= 0 && currentIndex < playlist.Count - 1)
                    {
                        var nextSong = playlist[currentIndex + 1];
                        var album = nextSong.Album;
                        txtText.Text = nextSong.Title;

                        if (File.Exists(album))
                        {
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();

                            // Update the selected item in Playlist
                            playlistListBox.SelectedItem = nextSong;
                        }
                        else
                        {
                            MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Reached the end of Playlist, go back to the first song
                        var firstSong = playlist[0];
                        var album = firstSong.Album;
                        txtText.Text = firstSong.Title;

                        if (File.Exists(album))
                        {
                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();

                            // Update the selected item to the first song in Playlist
                            playlistListBox.SelectedItem = firstSong;
                        }
                        else
                        {
                            MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }


        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                var selectedTitle = playlistListBox.SelectedItem.ToString();
                var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                if (selectedSong != null)
                {
                    AddSongWindow addSongWindow = new AddSongWindow();
                    addSongWindow.TitleTextBox.Text = selectedSong.Title;
                    addSongWindow.ArtistTextBox.Text = selectedSong.Artist;
                    addSongWindow.AlbumTextBox.Text = selectedSong.Album;
                    addSongWindow.SelectedOne = selectedSong;
                    addSongWindow.ShowDialog();

                    
                    playlistListBox.ItemsSource = null;
                    LoadTitleAllSongs();
                }
                else
                {
                    MessageBox.Show("Selected song not found in the playlist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a song from the playlist.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void StartRotatingDisk()
        {
            if (timer == null)
                return;

            // Reduce the interval to make the update smoother
            timer.Interval = TimeSpan.FromMilliseconds(10); // Cập nhật mỗi 10ms thay vì 20ms
            timer.Tick += (s, e) =>
            {
                // Quay đĩa mượt mà hơn với việc tăng nhỏ góc quay
                rotateTransform.Angle += 0.5;  // Thay đổi góc quay mỗi 0.5 độ
                if (rotateTransform.Angle >= 360)
                {
                    rotateTransform.Angle = 0;
                }
            };

            timer.Start(); // Bắt đầu quay đĩa ngay khi khởi tạo
        }

        // Xử lý double click để phóng to/thu nhỏ video
        private void mediaPlayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleFullScreen();
        }

        // Thêm xử lý phím ESC
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && isFullScreen)
            {
                ToggleFullScreen();
                e.Handled = true;
            }
        }

        // Method chuyển đổi chế độ full screen
        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                // Chuyển sang full screen
                playerContainer.SetValue(Grid.ColumnSpanProperty, 3);
                playerContainer.SetValue(Grid.ColumnProperty, 0);
                playerContainer.SetValue(Grid.RowSpanProperty, 3);
                playerContainer.Margin = new Thickness(0);
                mediaPlayer.Height = Double.NaN; // Auto height
                mediaPlayer.MaxHeight = Double.PositiveInfinity;
                mediaPlayer.MinHeight = 0;
                mediaPlayer.Margin = new Thickness(0);

                // Ẩn các control khác
                txtText.Visibility = Visibility.Collapsed;
                currentTimeText.Visibility = Visibility.Collapsed;
                totalTimeText.Visibility = Visibility.Collapsed;
                TimeCount.Visibility = Visibility.Collapsed;
                PauseButton.Visibility = Visibility.Collapsed;
                PLayMode.Visibility = Visibility.Collapsed;

                isFullScreen = true;
            }
            else
            {
                // Trở về chế độ bình thường
                playerContainer.SetValue(Grid.ColumnSpanProperty, 1);
                playerContainer.SetValue(Grid.ColumnProperty, 1);
                playerContainer.SetValue(Grid.RowSpanProperty, 1);
                playerContainer.Margin = new Thickness(10);
                mediaPlayer.Height = 300;
                mediaPlayer.MaxHeight = 300;
                mediaPlayer.MinHeight = 300;

                // Hiện lại các control
                txtText.Visibility = Visibility.Visible;
                currentTimeText.Visibility = Visibility.Visible;
                totalTimeText.Visibility = Visibility.Visible;
                TimeCount.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Visible;
                PLayMode.Visibility = Visibility.Visible;

                isFullScreen = false;
            }
        }




    }
}