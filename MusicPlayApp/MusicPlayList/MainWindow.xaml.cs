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
        private List<string> playlist = new List<string>();
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
            //LoadUserSongs();

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
            
        }


        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtText.Text = playlistListBox.SelectedItem.ToString();
            mediaPlayer.Source = new Uri(paths[playlistListBox.SelectedIndex]);
            mediaPlayer.Play();
            timer.Start();

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
            if (FavoriteListBox.SelectedItem != null)
            {
                FavoriteListBox.Items.Remove(FavoriteListBox.SelectedItem);
            }
        }

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void playlistListBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public async void LoadUserSongs()
        {
            var songs = await _songService.GetSongsByUserIdAsync(CurrentUser.UserId);
            foreach (var song in songs)
            {
                playlist.Add(song.Title);
                playlistListBox.Items.Add(song.Title);
            }
            if(songs == null)
            {
                return;
            }
        }
    }
}