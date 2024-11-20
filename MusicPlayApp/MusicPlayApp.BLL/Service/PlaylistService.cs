using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;

namespace MusicPlayApp.BLL.Service
{
    public class PlaylistService
    {
        private readonly PlaylistRepo _playlistRepo;

        public PlaylistService(PlaylistRepo playlistRepo)
        {
            _playlistRepo = playlistRepo ?? throw new ArgumentNullException(nameof(playlistRepo));
        }

        public async Task CreateAsync(Playlist playlist)
        {
            await _playlistRepo.CreateAsync(playlist);
        }

        public async Task<List<Playlist>> GetPlaylistsByUserIdAsync(int userId)
        {
            return await _playlistRepo.GetPlaylistsByUserIdAsync(userId);
        }

        public async Task AddSongToPlaylistAsync(int songId, int playlistId)
        {
            await _playlistRepo.AddSongToPlaylistAsync(songId, playlistId);
        }

        public async Task RemoveSongFromPlaylistAsync(int songId, int playlistId)
        {
            await _playlistRepo.RemoveSongFromPlaylistAsync(songId, playlistId);
        }
    }
}