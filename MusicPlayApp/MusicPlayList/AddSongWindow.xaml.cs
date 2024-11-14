using Microsoft.Win32;
using MusicPlayApp.BLL.Service;
using MusicPlayApp.DAL.Entities;
using System.Windows;

namespace MusicPlayList
{
    public partial class AddSongWindow : Window
    {
        private SongService _songService = new();
        public Song SelectedOne { get; set; }
        public List<Song> SelectedSongs { get; set; } = new List<Song>(); 

        public AddSongWindow()
        {
            InitializeComponent();
            if(SelectedOne != null)
            {
                TitleTextBox.Text = SelectedOne.Title;
                ArtistTextBox.Text = SelectedOne.Artist;
                AlbumTextBox.Text = SelectedOne.Album;
            }
            SongsListBox.Visibility = Visibility.Hidden;

        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            // Retrieve the input values
            string title = TitleTextBox.Text;
            string artist = ArtistTextBox.Text;
            string albumFilePath = AlbumTextBox.Text;

            if (SelectedOne != null)
            {
                // Update the song details
                SelectedOne.Title = TitleTextBox.Text;
                SelectedOne.Artist = ArtistTextBox.Text;
                SelectedOne.Album = AlbumTextBox.Text;

                _songService.Update(SelectedOne);
            }
            else if (SelectedSongs.Count > 1)
            {
                    foreach (var songs in SelectedSongs)
                    {
                        _songService.AddSong(songs);
                    }
            }
            else
            {
                // Create a new Song object
                var newSong = new Song
                {
                    Title = title,
                    Artist = artist,
                    Album = albumFilePath
                };
                _songService.AddSong(newSong);
            }

            // Close the window
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without saving
            this.DialogResult = false;
            this.Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select multiple album file paths
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.mp4;*.wav;*.flac|All Files|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var newSong = new Song
                    {
                        Title = System.IO.Path.GetFileNameWithoutExtension(fileName),
                        Artist = "Unknown Artist",
                        Album = fileName
                    };
                    SelectedSongs.Add(newSong);
                }

                // Update the UI to reflect the selected songs
                // For example, you can display the selected songs in a ListBox
                SongsListBox.ItemsSource = null;
                SongsListBox.ItemsSource = SelectedSongs.Select(song => song.Title).ToList();
                if (SelectedSongs.Count > 1)
                {
                    TitleLabel.Visibility = Visibility.Hidden;
                    TitleTextBox.Text = null;
                    TitleTextBox.Visibility = Visibility.Hidden;
                    ArtistLabel.Visibility = Visibility.Hidden;
                    ArtistTextBox.Text = null;
                    ArtistTextBox.Visibility = Visibility.Hidden;
                    SongsListBox.Visibility = Visibility.Visible;
                }
                else
                {
                    AlbumTextBox.Text = SelectedSongs.First().Album;
                }
            }
        }
    }
}
