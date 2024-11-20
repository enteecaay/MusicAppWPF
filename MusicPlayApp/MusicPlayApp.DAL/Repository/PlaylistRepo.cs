using MusicPlayApp.DAL.Entities;

namespace MusicPlayApp.DAL.Repository
{
    public class PlaylistRepo
    {
        private const string FileName = "playlists.json";

        public async Task CreateAsync(Playlist playlist)
        {
            var playlists = await JsonDatabase.ReadAsync<Playlist>(FileName);
            playlists.Add(playlist);
            await JsonDatabase.WriteAsync(FileName, playlists);
        }

        public async Task AddSongToPlaylistAsync(int songId, int playlistId)
        {
            var playlists = await JsonDatabase.ReadAsync<Playlist>(FileName);
            var playlist = playlists.FirstOrDefault(p => p.PlaylistId == playlistId);
            if (playlist != null)
            {
                playlist.SongIds.Add(songId);
                await JsonDatabase.WriteAsync(FileName, playlists);
            }
        }

        public async Task<List<Playlist>> GetPlaylistsByUserIdAsync(int userId)
        {
            var playlists = await JsonDatabase.ReadAsync<Playlist>(FileName);
            return playlists.Where(p => p.UserId == userId).ToList();
        }

        public async Task RemoveSongFromPlaylistAsync(int songId, int playlistId)
        {
            var playlists = await JsonDatabase.ReadAsync<Playlist>(FileName);
            var playlist = playlists.FirstOrDefault(p => p.PlaylistId == playlistId);
            if (playlist != null)
            {
                playlist.SongIds.Remove(songId);
                await JsonDatabase.WriteAsync(FileName, playlists);
            }
        }
    }
}