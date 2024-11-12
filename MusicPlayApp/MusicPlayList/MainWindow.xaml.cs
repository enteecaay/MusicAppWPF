using Microsoft.Win32;
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
using Microsoft.Extensions.DependencyInjection;
using MusicPlayApp.BLL.Service;
using MusicPlayApp.DLL.Repository;
using MusicPlayApp.DLL.Model;
using System.Collections.Generic;
using MusicPlayApp.BLL.Service;
using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DLL;


namespace MusicPlayList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserService _userService;
        private SongService _songService;
        private ServiceProvider _serviceProvider;

        private DispatcherTimer timer;
        private List<string> playlist = new List<string>();
        private bool isShuffle = false;
        private string[] files;
        private string[] paths;


        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();


            // Configure services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Initialize services
            _userService = _serviceProvider.GetRequiredService<UserService>();
            _songService = _serviceProvider.GetRequiredService<SongService>();

            // Load user data
            LoadUserData();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MusicPlayListDbContext>(options =>
                options.UseSqlServer("YourConnectionStringHere"));

            services.AddScoped<UserRepo>();
            services.AddScoped<UserService>();
            services.AddScoped<SongRepo>();
            services.AddScoped<SongService>();

            // Register other services
        }

        private void LoadUserData()
        {
            var user = _userService.GetUserByUsername("exampleUser");
            if (user != null)
            {
                var playlists = user.Playlists.ToList();
                var favoriteMusics = user.FavoriteMusics.ToList();
                // Bind playlists and favorite musics to UI
            }
        }

        private void AddUser(User user)
        {
            _userService.AddUser(user);
        }

        private void AddSongToPlaylist(int userId, Song song)
        {
            var user = _userService.GetUserById(userId);
            if (user != null)
            {
                var playlist = user.Playlists.FirstOrDefault();
                if (playlist != null)
                {
                    playlist.Songs.Add(song);
                    _songService.AddSong(song);
                }
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

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.mp4;*.wav;*.wma|All Files|*.*" // Bộ lọc file
            };

            // Chỉ thực hiện import nếu người dùng nhấn "OK"
            if (openFileDialog.ShowDialog() != true)
            {
                return; // Người dùng nhấn Cancel, không làm gì cả
            }

            // Lấy danh sách file và đường dẫn
            files = openFileDialog.FileNames;
            paths = openFileDialog.FileNames;

            // Thêm từng file vào playlistListBox
            foreach (var file in files)
            {
                playlistListBox.Items.Add(file);
            }
        }

        private void AddToPlaylist(string filePath)
        {
            playlist.Add(filePath);
            playlistListBox.Items.Add(System.IO.Path.GetFileName(filePath));
        }

        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtText.Text = playlistListBox.SelectedItem.ToString();
            mediaPlayer.Source = new Uri(paths[playlistListBox.SelectedIndex]);
            string selectedPath = paths[playlistListBox.SelectedIndex];
            string fileExtension = System.IO.Path.GetExtension(selectedPath).ToLower();
            bool isMusic = fileExtension == ".mp3" || fileExtension == ".wav";
            if (isMusic)
            {
                CDImage.Visibility = Visibility.Visible;
                StartRotatingDisk();
            }
            else
            {
                CDImage.Visibility = Visibility.Hidden;
            }
            mediaPlayer.Play();
            timer.Start();

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            StartRotatingDisk();
            timer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            if (timer != null)
            {
                timer.Stop();
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
        }

        private void SequentialButton_Click(object sender, RoutedEventArgs e)
        {
            isShuffle = false;
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
            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedIndex < playlistListBox.Items.Count - 1)
            {
                playlistListBox.SelectedIndex++;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedIndex > 0)
            {
                playlistListBox.SelectedIndex--;
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
            if (FavoriteListBox.Items.Count == 0)
            {
                MessageBox.Show("Danh sách yêu thích trống, không có gì để lưu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using (StreamWriter save = new StreamWriter(@"D:\data\PRN212\MusicPlayApp\MusicPlayList\Storage\Favorite.txt"))
            {
                foreach (var item in FavoriteListBox.Items)
                {
                    save.WriteLine(item.ToString());
                }
            }

            MessageBox.Show("Đã lưu danh sách yêu thích thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            FavoriteListBox.Items.Clear();

        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            using (StreamReader read = new StreamReader(@"D:\data\PRN212\MusicPlayApp\MusicPlayList\Storage\Favorite.txt"))
            {
                string line;
                while ((line = read.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        FavoriteListBox.Items.Add(line);
                    }
                }
            }
        }

        private void FavoriteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoriteListBox.SelectedItem != null)
            {
                txtText.Text = FavoriteListBox.SelectedItem.ToString();
                mediaPlayer.Source = new Uri(txtText.Text);
                string selectedPath = paths[playlistListBox.SelectedIndex];
                string fileExtension = System.IO.Path.GetExtension(selectedPath).ToLower();
                bool isMusic = fileExtension == ".mp3" || fileExtension == ".wav";
                if (isMusic)
                {
                    CDImage.Visibility = Visibility.Visible;
                    StartRotatingDisk();
                }
                else
                {
                    CDImage.Visibility = Visibility.Hidden;
                }
                mediaPlayer.Play();
                timer.Start();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FavoriteListBox.SelectedItem != null)
            {
                FavoriteListBox.Items.Remove(FavoriteListBox.SelectedItem);
            }
        }

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                string selectedSong = playlistListBox.SelectedItem.ToString();

                if (!FavoriteListBox.Items.Contains(selectedSong))
                {
                    FavoriteListBox.Items.Add(selectedSong);
                    Addbtn.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Bài hát đã có trong danh sách yêu thích.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bài hát để thêm vào danh sách yêu thích.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void StartRotatingDisk()
        {
            if (timer == null)
                return;

            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += (s, e) =>
            {
                rotateTransform.Angle += 1;
                if (rotateTransform.Angle >= 360)
                {
                    rotateTransform.Angle = 0;
                }
            };
        }
    }
}