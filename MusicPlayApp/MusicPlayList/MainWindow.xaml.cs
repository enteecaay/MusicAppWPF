using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using MusicPlayApp.BLL.Service;
using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;
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
using System.Windows.Media.Animation;
using Microsoft.Identity.Client.NativeInterop; // Để sử dụng DoubleAnimation



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

        private readonly SongService _songService;
        private readonly PlaylistService _playlistService;
        private readonly UserService _userService;
        private readonly FavoriteService _favoriteService;

        public MusicPlayApp.DAL.Entities.User? CurrentUser { get; set; }

        private bool isFullScreen = false; // Thêm biến theo dõi trạng thái full screen

        public MainWindow(MusicPlayApp.DAL.Entities.User currentUser)
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

            // Initialize repositories
            var songRepository = new SongRepository();
            var playlistRepo = new PlaylistRepo();
            var userRepo = new UserRepo();
            var favoriteRepo = new FavoriteRepo();

            // Initialize services
            _songService = new SongService(songRepository);
            _playlistService = new PlaylistService(playlistRepo);
            _userService = new UserService(userRepo);
            _favoriteService = new FavoriteService(favoriteRepo, songRepository);

            LoadTitleAllSongs();
            InitializeFavoriteList();
            LoadFavoriteList();

            // Initialize other components
            InitializeTimer();
            LoadPlaylist();
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

                    // Get the list of favorite songs
                    var favoriteSongs = await _favoriteService.GetFavoritesByUserIdAsync(userId);

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



        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            AddSongWindow addSong = new AddSongWindow();
            addSong.ShowDialog();
            // Refresh the list
            await LoadTitleAllSongs();
        }

        private bool isPlaying = true;

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if mediaPlayer has a valid media source
            if (mediaPlayer.Source == null)
            {
                MessageBox.Show("No song is currently loaded. Please select a song to play.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (isPlaying)
            {
                mediaPlayer.Pause();
                isPlaying = false;
                PauseButton.Content = "Continue";
                PauseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1DB954"));

                timer.Stop(); // Stop the timer to pause the rotation of the disc
            }
            else
            {
                mediaPlayer.Play();
                isPlaying = true;
                PauseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8c1010"));
                PauseButton.Content = "Pause";
                timer.Start();
            }
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


        private void PlaybackModeButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle between shuffle and sequential modes
            isShuffle = !isShuffle;

            if (isShuffle)
            {
                PlaybackModeButton.Content = "Shuffle";
                PlaybackModeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1DB954"));
                MessageBox.Show("Chế độ phát ngẫu nhiên đã được kích hoạt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                PlaybackModeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
                MessageBox.Show("Chế độ phát tuần tự đã được kích hoạt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                        txtText.Text = $"{selectedSong.Title}  {selectedSong.Artist}";
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
                // Reset the pause/play button state
                isPlaying = false;
                PauseButton.Content = "Play"; // Set to "Play" as initial state

                isPlayingFromFavoriteList = true;
                isPlayingFromPlaylist = false;

                // Reset the previous selection
                playlistListBox.SelectedItem = null;

                var selectedSong = FavoriteListBox.SelectedItem as Song;
                if (selectedSong != null)
                {
                    var album = selectedSong.Album;
                    txtText.Text = $"{selectedSong.Title}  {selectedSong.Artist}";

                    if (File.Exists(album))
                    {
                        string fileExtension = System.IO.Path.GetExtension(album).ToLower();

                        if (fileExtension == ".mp3" || fileExtension == ".wav")
                        {
                            cdContainer.Visibility = Visibility.Visible;
                            CDImage.Visibility = Visibility.Visible;
                            mediaPlayer.Visibility = Visibility.Collapsed;
                            StartRotatingDisk();

                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                            isPlaying = true;
                            PauseButton.Content = "Pause";
                        }
                        else
                        {
                            cdContainer.Visibility = Visibility.Collapsed;
                            CDImage.Visibility = Visibility.Collapsed;
                            mediaPlayer.Visibility = Visibility.Visible;
                            StopRotatingDisk();

                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                            isPlaying = true;
                            PauseButton.Content = "Pause";
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
                mediaPlayer.Stop();
                timer?.Stop();
                txtText.Text = "";
                mediaPlayer.Source = null;

                TimeCount.Value = 0;  // Reset progress slider to the start
                currentTimeText.Text = "00:00";  // Display 00:00 as the current time
                totalTimeText.Text = "00:00";  // Display 00:00 as the total time

                // Xử lý xóa từ Playlist
                if (playlistListBox.SelectedItem != null)
                {
                    FavoriteListBox.SelectedItem = null;  // Reset FavoriteListBox selection

                    var selectedTitle = playlistListBox.SelectedItem.ToString();
                    var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                    if (selectedSong != null)
                    {
                        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa '{selectedSong.Title}' khỏi danh sách phát?",
                                                     "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            _songService.RemoveSongAsync(selectedSong);  // Xóa bài hát khỏi cơ sở dữ liệu

                            // Tải lại Playlist và Favorite List để cập nhật thay đổi
                            playlistListBox.ItemsSource = null;
                            LoadTitleAllSongs();
                            FavoriteListBox.ItemsSource = null;
                            await LoadFavoriteList();
                        }
                    }
                }
                // Xử lý xóa từ Favorite List
                else if (FavoriteListBox.SelectedItem != null)
                {
                    playlistListBox.SelectedItem = null;  // Reset playlistListBox selection

                    var selectedSong = FavoriteListBox.SelectedItem as Song;

                    if (selectedSong != null && CurrentUser != null)
                    {
                        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa '{selectedSong.Title}' khỏi danh sách yêu thích?",
                                                     "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            await _favoriteService.RemoveFavoriteAsync(CurrentUser.UserId, selectedSong.SongId.Value);

                            // Tải lại danh sách yêu thích
                            FavoriteListBox.ItemsSource = null;
                            await LoadFavoriteList();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một bài hát để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi xóa bài hát: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                // Get the selected song title
                var selectedTitle = playlistListBox.SelectedItem.ToString();
                var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                if (selectedSong != null)
                {
                    if (selectedSong.SongId.HasValue)
                    {
                        int songId = (int)selectedSong.SongId;
                        Console.WriteLine($"Song ID: {songId}");

                        // Check the current user
                        if (CurrentUser != null)
                        {
                            int userId = CurrentUser.UserId; // Get UserId from the logged-in user

                            try
                            {
                                // Check if the song is already in the user's favorite list
                                var existingFavorite = await _favoriteService.CheckIfFavoriteExistsAsync(userId, songId);

                                if (existingFavorite)
                                {
                                    // If the song is already in the favorite list, notify the user
                                    MessageBox.Show("This song is already in your favorite list.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                else
                                {
                                    // Call service to add the song to the favorite list
                                    await _favoriteService.AddFavoriteAsync(userId, songId, "My Favorite List");
                                    // Reload the favorite list
                                    await LoadFavoriteList();

                                    MessageBox.Show("The song has been added to your favorite list.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle any errors
                                MessageBox.Show($"An error occurred while adding the song to the favorite list: {ex.Message}",
                                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please log in to add songs to your favorite list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The selected song does not have a valid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No song selected to add to the favorite list.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a song from the playlist.", "Notice", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private async Task LoadFavoriteList()
        {
            if (CurrentUser != null)
            {
                try
                {
                    int userId = CurrentUser.UserId;

                    // Lấy danh sách bài hát yêu thích
                    var favoriteSongs = await _favoriteService.GetFavoritesByUserIdAsync(userId);
                    // Làm sạch danh sách trước khi thêm mới
                    FavoriteListBox.Items.Clear();

                    // Hiển thị các bài hát yêu thích
                    foreach (var song in favoriteSongs)
                    {
                        Song addSong = await _songService.GetSongByIdAsync((int)song.SongId);
                        FavoriteListBox.Items.Add(addSong);  // Thêm đối tượng Song vào FavoriteListBox
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

        public async Task LoadTitleAllSongs()
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
                // Reset the pause/play button state
                isPlaying = false;
                PauseButton.Content = "Play";

                isPlayingFromPlaylist = true;  // Mark as playing from Playlist
                isPlayingFromFavoriteList = false;  // Stop playing from Favorite List

                // Reset the previous selection
                FavoriteListBox.SelectedItem = null;

                var selectedTitle = playlistListBox.SelectedItem.ToString();
                var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                if (selectedSong != null)
                {
                    var album = selectedSong.Album;
                    txtText.Text = $"{selectedSong.Title}  {selectedSong.Artist}";

                    if (File.Exists(album))
                    {
                        string fileExtension = System.IO.Path.GetExtension(album).ToLower();
                        if (fileExtension == ".mp3" || fileExtension == ".wav")
                        {
                            // Audio file: Show disk and play audio
                            cdContainer.Visibility = Visibility.Visible;
                            CDImage.Visibility = Visibility.Visible;
                            mediaPlayer.Visibility = Visibility.Collapsed;
                            StartRotatingDisk();

                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                            isPlaying = true; // Set playing to true as the song starts playing automatically
                            PauseButton.Content = "Pause"; // Set the button to "Pause" as it's playing now
                        }
                        else
                        {
                            // Non-audio file (video): Show video and hide the disk
                            cdContainer.Visibility = Visibility.Collapsed;
                            CDImage.Visibility = Visibility.Collapsed;
                            mediaPlayer.Visibility = Visibility.Visible;
                            StopRotatingDisk();

                            mediaPlayer.Source = new Uri(album);
                            mediaPlayer.Play();
                            timer.Start();
                            isPlaying = true;
                            PauseButton.Content = "Pause";
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

        private List<Song> favoriteList = new List<Song>(); 

        private List<int> playedIndices = new List<int>();

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (isShuffle)
            {
                // Shuffle mode
                if (isPlayingFromFavoriteList)
                {
                    PlayRandomSong(favoriteList, FavoriteListBox);
                }
                else if (isPlayingFromPlaylist)
                {
                    PlayRandomSong(playlist, playlistListBox);
                }
            }
            else
            {
                // Sequential Mode
                if (isPlayingFromFavoriteList)
                {
                    PlayNextSequentialSong(favoriteList, FavoriteListBox);
                }
                else if (isPlayingFromPlaylist)
                {
                    PlayNextSequentialSong(playlist, playlistListBox);
                }
            }
        }

        // Method to play a random song without repetition until all songs have been played
        private void PlayRandomSong(List<Song> songList, ListBox listBox)
        {
            if (songList.Count == 0) return;

            var random = new Random();
            int nextSongIndex;

            // If all songs have been played, reset the playedIndices list
            if (playedIndices.Count >= songList.Count)
            {
                playedIndices.Clear();
            }

            // Find a new random index that has not been played
            do
            {
                nextSongIndex = random.Next(songList.Count);
            } while (playedIndices.Contains(nextSongIndex));

            // Add the new song index to playedIndices
            playedIndices.Add(nextSongIndex);

            // Play the selected song
            var nextSong = songList[nextSongIndex];
            var album = nextSong.Album;
            txtText.Text = $"{nextSong.Title}  {nextSong.Artist}";

            if (File.Exists(album))
            {
                mediaPlayer.Source = new Uri(album);
                mediaPlayer.Play();
                timer.Start();

                // Update the selected item in the ListBox
                listBox.SelectedItem = nextSong;
            }
            else
            {
                MessageBox.Show($"Không tìm thấy tệp: {album}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to play the next song in sequential mode
        private void PlayNextSequentialSong(List<Song> songList, ListBox listBox)
        {
            if (songList.Count == 0) return;

            int currentIndex = songList.FindIndex(s => s.Title == txtText.Text);

            // Check if we are at the last song in the list
            if (currentIndex >= 0 && currentIndex < songList.Count - 1)
            {
                var nextSong = songList[currentIndex + 1];
                var album = nextSong.Album;
                txtText.Text = $"{nextSong.Title}  {nextSong.Artist}";

                if (File.Exists(album))
                {
                    mediaPlayer.Source = new Uri(album);
                    mediaPlayer.Play();
                    timer.Start();

                    // Update the selected item in the ListBox
                    listBox.SelectedItem = nextSong;
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy tệp: {album}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Reached the end of the list, go back to the first song
                var firstSong = songList[0];
                var album = firstSong.Album;
                txtText.Text = $"{firstSong.Title}  {firstSong.Artist}";

                if (File.Exists(album))
                {
                    mediaPlayer.Source = new Uri(album);
                    mediaPlayer.Play();
                    timer.Start();

                    // Update the selected item to the first song in the ListBox
                    listBox.SelectedItem = firstSong;
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy tệp: {album}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                PlayMode.Visibility = Visibility.Collapsed;

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
                PlayMode.Visibility = Visibility.Visible;

                isFullScreen = false;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Stop any playing music
            if (isPlaying)
            {
                StopMusic();
            }
            var result = MessageBox.Show("Do you really want to logout", "Logout Confirmation", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow(CurrentUser);
                // Clear the current user session
                CurrentUser = null;
                this.Close();
                // Navigate to the login window
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
            }
            return;
        }
        private void StopMusic()
        {
            isPlaying = false;
        }

        private async void LoadPlaylist()
        {
            try
            {
                playlist = await _songService.GetAllSongsAsync();
                // Bind playlist to UI, e.g., ListBox or DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading playlist: {ex.Message}");
            }
        }

        private async void AddSongToPlaylist(Song song)
        {
            try
            {
                await _songService.AddSongAsync(song);
                playlist.Add(song);
                // Update UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding song: {ex.Message}");
            }
        }

        private async void RemoveSongFromPlaylist(Song song)
        {
            try
            {
                await _songService.RemoveSongAsync(song);
                playlist.Remove(song);
                // Update UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing song: {ex.Message}");
            }
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            // Search in the playlist
            var filteredPlaylistSongs = await _songService.SearchSongsAsync(query);

            // Search in the favorite list
            var favoriteSongs = await _favoriteService.GetFavoritesByUserIdAsync(CurrentUser.UserId);
            var filteredFavoriteSongs = favoriteSongs.Where(s => s.Title.ToLower().Contains(query) || s.Artist.ToLower().Contains(query)).ToList();

            // Update the playlistListBox
            playlistListBox.Items.Clear();
            foreach (var song in filteredPlaylistSongs)
            {
                playlistListBox.Items.Add(song.Title);
            }

            // Update the FavoriteListBox
            FavoriteListBox.Items.Clear();
            foreach (var song in filteredFavoriteSongs)
            {
                FavoriteListBox.Items.Add(song);
            }
        }
    }
}