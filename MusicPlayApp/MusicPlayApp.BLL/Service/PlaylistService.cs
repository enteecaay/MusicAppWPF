using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;
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
        private SongRepository _songRepository = new();
        public void Create(FavoriteList favoriteList)
        {
            _playlistRepo.Create(favoriteList);
        }
    

        public FavoriteList GetFavoriteListByUserIdService(int userId)
        {
            return _playlistRepo.GetFavoriteListByUserId(userId);
        }

        public void AddSongToFavoriteListService(int songId, int FavoriteListId)
        {

        }
    }

}
