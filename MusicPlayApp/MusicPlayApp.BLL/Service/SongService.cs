using MusicPlayApp.DLL.Entities;
using MusicPlayApp.DLL.Repository;
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
    }
}
