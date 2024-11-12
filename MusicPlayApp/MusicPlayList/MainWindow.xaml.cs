using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MusicPlayApp.BLL.Service;
using MusicPlayApp.DLL.Repository;
using MusicPlayApp.DLL.Model;
using MusicPlayApp.DLL;

namespace MusicPlayList
{
    public partial class MainWindow : Window
    {
        private UserService _userService;
        private SongService _songService;
        private PlayListService _playListService;
        private FavoriteMusicService _favoriteMusicService;
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
            _playListService = _serviceProvider.GetRequiredService<PlayListService>();
            _favoriteMusicService = _serviceProvider.GetRequiredService<FavoriteMusicService>();

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
            services.AddScoped<PlayListRepo>();
            services.AddScoped<PlayListService>();
            services.AddScoped<FavoriteMusicsRepo>();
            services.AddScoped<FavoriteMusicService>();
        }

        private void LoadUserData()
        {
            var user = _userService.GetUserByUsername("exampleUser");
            if (user != null)
            {
                var playlists = _playListService.GetPlaylistsByUserId(user.Id);
                foreach (var playlist in playlists)
                {
                    playlistListBox.Items.Add(playlist.Name);
                }

                var favoriteSongs = _favoriteMusicService.GetFavoriteSongsByUserId(user.Id);
                foreach (var song in favoriteSongs)
                {
                    FavoriteListBox.Items.Add(song.Title);
                }
            }
        }

        private void AddToPlaylist(string filePath)
        {
            playlist.Add(filePath);
            playlistListBox.Items.Add(System.IO.Path.GetFileName(filePath));
        }

        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                txtText.Text = playlistListBox.SelectedItem.ToString();
                mediaPlayer.Source = new Uri(paths[playlistListBox.SelectedIndex]);
                mediaPlayer.Play();
                timer.Start();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            timer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
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
                mediaPlayer.Volume = volumeSlider.Value / 100;
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

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.mp4;*.wav;*.wma|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                files = openFileDialog.FileNames;
                paths = openFileDialog.FileNames;

                foreach (var file in files)
                {
                    var song = new Song
                    {
                        Title = Path.GetFileNameWithoutExtension(file),
                        Url = file,
                        Artist = "Unknown", // Giả định nghệ sĩ nếu không có thông tin
                        Album = "Unknown"
                    };

                    _songService.AddSong(song); // Lưu vào cơ sở dữ liệu
                    playlistListBox.Items.Add(song.Title);
                }
            }
        }

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            if (playlistListBox.SelectedItem != null)
            {
                string selectedSong = playlistListBox.SelectedItem.ToString();
                var song = _songService.GetAllSongs().FirstOrDefault(s => s.Title == selectedSong);

                if (song != null)
                {
                    var user = _userService.GetUserByUsername("exampleUser");
                    if (!_favoriteMusicService.IsFavorite(user.Id, song.Id))
                    {
                        _favoriteMusicService.AddToFavorite(user.Id, song.Id);
                        FavoriteListBox.Items.Add(selectedSong);
                    }
                    else
                    {
                        MessageBox.Show("Bài hát đã có trong danh sách yêu thích.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bài hát để thêm vào danh sách yêu thích.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FavoriteListBox.SelectedItem != null)
            {
                string selectedSong = FavoriteListBox.SelectedItem.ToString();
                var song = _songService.GetAllSongs().FirstOrDefault(s => s.Title == selectedSong);
                var user = _userService.GetUserByUsername("exampleUser");

                if (song != null)
                {
                    _favoriteMusicService.RemoveFromFavorite(user.Id, song.Id);
                    FavoriteListBox.Items.Remove(FavoriteListBox.SelectedItem);
                }
            }
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            FavoriteListBox.Items.Clear();
            var user = _userService.GetUserByUsername("exampleUser");
            var favoriteSongs = _favoriteMusicService.GetFavoriteSongsByUserId(user.Id);

            foreach (var song in favoriteSongs)
            {
                FavoriteListBox.Items.Add(song.Title);
            }
            MessageBox.Show("Danh sách yêu thích đã được tải từ cơ sở dữ liệu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
