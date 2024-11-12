using MusicPlayApp.DLL.Model;
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
        private readonly SongRepo _songRepo = new();

        public IEnumerable<Song> GetAllSongs()
        {
            return _songRepo.GetAllSongs();
        }

        public void AddSong(Song song)
        {
            _songRepo.AddSong(song);
        }
    }
}
