using MusicPlayApp.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.DAL.Repository
{
    public class PlaylistRepo
    {
        private MusicPlayerAppContext _context;
        public void Create(FavoriteList obj)
        {
            _context = new MusicPlayerAppContext();
            _context.FavoriteLists.Add(obj);
            _context.SaveChanges();
        }

        public void AddSongToFavoriteList(int songId, int FavoriteListId)
        {
            _context = new MusicPlayerAppContext();
            var playlist = _context.Playlists.Find(FavoriteListId);
            var song = _context.Songs.Find(songId);
            playlist.Song = song;
            _context.SaveChanges();
        }
        public FavoriteList GetFavoriteListByUserId(int userId)
        {
            _context = new MusicPlayerAppContext();
            return _context.FavoriteLists.FirstOrDefault(f => f.UserId == userId);
        }
    }
}
