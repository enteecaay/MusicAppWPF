using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.DAL.Repository
{
    public class SongRepository
    {
        private MusicPlayerAppContext _context;

        public async Task<List<Song>> GetAllSongsAsync()
        {
            _context = new MusicPlayerAppContext();
            return await _context.Songs
                .Include(s => s.FavoriteLists)
                .Include(s => s.Playlists)
                .ThenInclude(p => p.User)
                .ToListAsync();
        }

        public async Task<List<Song>> GetSongsByUserIdAsync(int userId)
        {
            _context = new MusicPlayerAppContext();
            return await _context.Songs
                .Include(s => s.FavoriteLists)
                .Include(s => s.Playlists)
                .ThenInclude(p => p.User)
                .Where(s => s.Playlists.Any(p => p.UserId == userId))
                .ToListAsync();

        }

        public void AddSong(Song song)
        {
            _context = new();
            _context.Songs.Add(song);
            _context.SaveChanges();
        }

        public void Update(Song song)
        {
            _context = new();
            _context.Songs.Update(song);
            _context.SaveChanges();
        }

        public void Remove(Song song)
        {
            if (song == null)
            {
                throw new ArgumentNullException(nameof(song), "The song cannot be null.");
            }

            _context = new();

            // Xóa các mục liên quan trong bảng FavoriteLists
            var relatedFavorites = _context.FavoriteLists.Where(f => f.SongId == song.SongId).ToList();
            _context.FavoriteLists.RemoveRange(relatedFavorites);

            // Xóa bài hát
            var existingSong = _context.Songs.Find(song.SongId);
            if (existingSong != null)
            {
                _context.Songs.Remove(existingSong);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("The song does not exist in the database.");
            }
        }





    }
}
