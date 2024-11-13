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

        public MainWindow()
        {
            InitializeComponent();
            InitializePlayer();
            volumeSlider.Value = 100;
            VolumeText.Text = "100%";
            LoadTitleAllSongs();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

        }

        private void InitializePlayer()
        {
            mediaPlayer.Volume = volumeSlider.Value;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
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
                isPlaying = false; // Cập nhật trạng thái thành "đã dừng"
                PauseButton.Content = "Continue";
            }
            else
            {
                mediaPlayer.Play();
                isPlaying = true; // Cập nhật trạng thái thành "đang phát"
                PauseButton.Content = "Pause";

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
                txtText.Text = FavoriteListBox.SelectedItem.ToString();
                mediaPlayer.Source = new Uri(txtText.Text);
                mediaPlayer.Play();
                timer.Start();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                var selectedTitle = playlistListBox.SelectedItem.ToString();
                var selectedSong = playlist.FirstOrDefault(s => s.Title == selectedTitle);

                if (selectedSong != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the song '{selectedSong.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _songService.RemoveSong(selectedSong);
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
            playlistListBox.ItemsSource = null;
            LoadTitleAllSongs();
        }

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            
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
            playlist = await _songService.GetAllSongsAsync();
            playlistListBox.Items.Clear();
            foreach (var song in playlist)
            {
                playlistListBox.Items.Add(song.Title);
            }
        }
        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
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
               
    }
}