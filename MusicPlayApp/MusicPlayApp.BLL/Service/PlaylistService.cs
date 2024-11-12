using MusicPlayApp.DLL.Entities;
using MusicPlayApp.DLL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.BLL.Service
{
    public class PlaylistService
    {
        private PlaylistRepo _playlistRepo = new();
        public void Create(Playlist playlist)
        {
            _playlistRepo.Create(playlist);
        }
    }
}
