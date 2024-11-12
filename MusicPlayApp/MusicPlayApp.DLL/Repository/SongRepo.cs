using MusicPlayApp.DLL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.DLL.Repository
{
    public class SongRepo
    {
        private MusicPlayListDbContext _context;

        public IEnumerable<Song> GetAllSongs()
        {
            _context = new MusicPlayListDbContext();
            return _context.Songs.ToList();
        }

        public void AddSong(Song song)
        {
            _context = new MusicPlayListDbContext();
            _context.Songs.Add(song);
            _context.SaveChanges();
        }
    }
}
