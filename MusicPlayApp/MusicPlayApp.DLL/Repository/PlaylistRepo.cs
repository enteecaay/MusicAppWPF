using MusicPlayApp.DLL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.DLL.Repository
{
    public class PlaylistRepo
    {
        private MusicPlayerAppContext _context;
        public void Create(Playlist obj)
        {
            _context = new MusicPlayerAppContext();
            _context.Playlists.Add(obj);
            _context.SaveChanges();
        }
    }
}
