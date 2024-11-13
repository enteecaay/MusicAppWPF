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

            volumeSlider.Value = 100;
            VolumeText.Text = "100%";
            LoadTitleAllSongs();
            InitializeFavoriteList();
        }

        // Phương thức bất đồng bộ để tải danh sách yêu thích
        private async void InitializeFavoriteList()
        {
            await LoadFavoriteList();
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
            PLayMode.Content = "Shuffle";
            MessageBox.Show("Shuffle mode activated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SequentialButton_Click(object sender, RoutedEventArgs e)
        {
            isShuffle = false;
            PLayMode.Content = "Sequential";
            MessageBox.Show("Sequential mode activated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isShuffle)
            {
                var random = new Random();
                playlistListBox.SelectedIndex = random.Next(playlist.Count);
            }
            else
            {
                if (playlistListBox.SelectedIndex < playlist.Count - 1)
                    playlistListBox.SelectedIndex++;
                else
                    playlistListBox.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedIndex < playlistListBox.Items.Count - 1)
            {
                 playlistListBox.SelectedIndex++;
                if (playlistListBox.SelectedItem != null)
                {
                    var selectedTitle = playlistListBox.SelectedItem.ToString();
                    var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                    if (selectedSong != null)
                    {
                        var filePath = selectedSong.Album;

                        // Check if the file exists
                        if (File.Exists(filePath))
                        {
                            mediaPlayer.Source = new Uri(filePath);
                            mediaPlayer.Play();
                            timer.Start();
                        }
                        else
                        {
                            MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
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
                //mediaPlayer.Play();
                //timer.Start();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedIndex > 0)
            {
                playlistListBox.SelectedIndex--;

                if (playlistListBox.SelectedItem != null)
                {
                    var selectedTitle = playlistListBox.SelectedItem.ToString();
                    var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                    if (selectedSong != null)
                    {
                        var filePath = selectedSong.Album;

                        // Check if the file exists
                        if (File.Exists(filePath))
                        {
                            mediaPlayer.Source = new Uri(filePath);
                            mediaPlayer.Play();
                            timer.Start();
                        }
                        else
                        {
                            MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
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
        }

        private void TimeCount_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mouseX = e.GetPosition(TimeCount).X;
            var percentage = mouseX / TimeCount.ActualWidth;
            var newTime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds * percentage;

            mediaPlayer.Position = TimeSpan.FromSeconds(newTime);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
           

        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void FavoriteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoriteListBox.SelectedItem != null)
            {
                // Khi một bài hát được chọn trong FavoriteListBox, đặt SelectedItem của PlaylistListBox về null
                playlistListBox.SelectedItem = null;

                var selectedSong = FavoriteListBox.SelectedItem as Song;
                if (selectedSong != null)
                {
                    var album = selectedSong.Album;
                    txtText.Text = selectedSong.Title;

                    // Check if the file exists
                    if (File.Exists(album))
                    {
                        mediaPlayer.Source = new Uri(album);
                        string selectedPath = album;
                        string fileExtension = System.IO.Path.GetExtension(selectedPath).ToLower();
                        bool isMusic = fileExtension == ".mp3" || fileExtension == ".wav";
                        if (isMusic)
                        {
                            CDImage.Visibility = Visibility.Visible;
                            StartRotatingDisk();

                            // Reset trạng thái phát nhạc
                            isPlaying = true;
                            PauseButton.Content = "Pause";
                        }
                        else
                        {
                            CDImage.Visibility = Visibility.Hidden;
                        }

                        mediaPlayer.Play();
                        timer.Start();
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
                // Khi một bài hát được chọn trong PlaylistListBox, đặt SelectedItem của FavoriteListBox về null
                FavoriteListBox.SelectedItem = null;

                var selectedTitle = playlistListBox.SelectedItem.ToString();
                var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);
                if (selectedSong != null)
                {
                    var album = selectedSong.Album;
                    txtText.Text = selectedSong.Title;

                    // Check if the file exists
                    if (File.Exists(album))
                    {
                        mediaPlayer.Source = new Uri(album);
                        string selectedPath = album;
                        string fileExtension = System.IO.Path.GetExtension(selectedPath).ToLower();
                        bool isMusic = fileExtension == ".mp3" || fileExtension == ".wav";
                        if (isMusic)
                        {
                            CDImage.Visibility = Visibility.Visible;
                            StartRotatingDisk();

                            // Reset trạng thái phát nhạc
                            isPlaying = true;
                            PauseButton.Content = "Pause";
                        }
                        else
                        {
                            CDImage.Visibility = Visibility.Hidden;
                        }

                        mediaPlayer.Play();
                        timer.Start();
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }



        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            // Tìm chỉ mục của bài hiện tại
            int currentIndex = playlist.FindIndex(s => s.Title == txtText.Text);

            // Nếu bài hiện tại không phải là bài cuối, chuyển sang bài tiếp theo
            if (currentIndex >= 0 && currentIndex < playlist.Count - 1)
            {
                var nextSong = playlist[currentIndex + 1];
                var album = nextSong.Album;
                txtText.Text = nextSong.Title;

                // Kiểm tra và phát bài nhạc tiếp theo
                if (File.Exists(album))
                {
                    mediaPlayer.Source = new Uri(album);
                    mediaPlayer.Play();
                    timer.Start();
                }
                else
                {
                    MessageBox.Show($"File not found: {album}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Nếu hết danh sách, có thể dừng hoặc lặp lại danh sách tùy ý
                MessageBox.Show("Đã hết danh sách phát", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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






    }
}