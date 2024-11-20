using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayApp.BLL.Service
{
    public class SongService
    {
        private readonly SongRepository _songRepository;

        // Inject SongRepository through constructor
        public SongService(SongRepository songRepository)
        {
            _songRepository = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
        }

        public async Task<List<Song>> GetAllSongsAsync()
        {
            return await _songRepository.GetAllSongsAsync();
        }

        public async Task<List<Song>> GetSongsByUserIdAsync(int userId)
        {
            return await _songRepository.GetSongsByUserIdAsync(userId);
        }

        public async Task AddSongAsync(Song song)
        {
            await _songRepository.AddSongAsync(song);
        }

        public async Task RemoveSongAsync(Song song)
        {
            if (song == null)
            {
                throw new ArgumentNullException(nameof(song));
            }

            await _songRepository.RemoveSongAsync(song);
        }

        public async Task UpdateSongAsync(Song song)
        {
            if (song == null)
            {
                throw new ArgumentNullException(nameof(song));
            }

            await _songRepository.UpdateSongAsync(song);
        }

        public async Task<Song> GetSongByIdAsync(int songId)
        {
            return await _songRepository.GetSongByIdAsync(songId);
        }

        public async Task<List<Song>> SearchSongsAsync(string query)
        {
            return await _songRepository.SearchSongsAsync(query);
        }
    }
}
