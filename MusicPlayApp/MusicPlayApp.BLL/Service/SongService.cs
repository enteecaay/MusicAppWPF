using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.BLL.Service
{
    public class SongService
    {
        private SongRepository _songRepository = new SongRepository();

        public async Task<List<Song>> GetAllSongsAsync()
        {
            return await _songRepository.GetAllSongsAsync();
        }
        public async Task<List<Song>> GetSongsByUserIdAsync(int userId)
        {
            return await _songRepository.GetSongsByUserIdAsync(userId);
        }

        public void AddSong(Song song)
        {
            _songRepository.AddSong(song);
        }

        public void RemoveSong(Song song)
        {
            if (song == null)
            {
                throw new ArgumentNullException(nameof(song), "The song cannot be null.");
            }

            try
            {
                _songRepository.Remove(song);
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý lỗi nếu cần
                throw new Exception($"An error occurred while removing the song: {ex.Message}", ex);
            }
        }
        public void Update(Song song)
        {
            _songRepository.Update(song);
        }


    }
}
