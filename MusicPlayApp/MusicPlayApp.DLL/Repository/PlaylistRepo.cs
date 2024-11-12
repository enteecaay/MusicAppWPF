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
        MusicPlayerAppContext _context;
        public void Create(int UserId)
        {
            _context = new();
            //_context.Playlists.Add();
        }
    }
}
