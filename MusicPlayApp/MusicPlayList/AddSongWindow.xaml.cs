using Microsoft.Win32;
using MusicPlayApp.DLL.Entities;
using System.Windows;

namespace MusicPlayList
{
    public partial class AddSongWindow : Window
    {
        public AddSongWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the input values
            string title = TitleTextBox.Text;
            string artist = ArtistTextBox.Text;
            string albumFilePath = AlbumTextBox.Text;

            // Create a new Song object
            var newSong = new Song
            {
                Title = title,
                Artist = artist,
                Album = albumFilePath
            };

            // Save the new song to the database (implement the logic as needed)
            // Example: _songService.AddSong(newSong);

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
            // Open a file dialog to select the album file path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.flac|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                AlbumTextBox.Text = openFileDialog.FileName;
            }
        }
    }
}
