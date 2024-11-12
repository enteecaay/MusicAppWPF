using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DLL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.DLL.Repository
{
    public class SongRepository
    {
        private MusicPlayerAppContext _context;

        public async Task<List<Song>> GetAllSongsAsync()
        {
            return await _context.Songs
                .Include(s => s.FavoriteLists)
                .Include(s => s.Playlists)
                .ThenInclude(p => p.User)
                .ToListAsync();
        }

        public async Task<List<Song>>? GetSongsByUserIdAsync(int userId)
        {
            return await _context.Songs
                .Include(s => s.FavoriteLists)
                .Include(s => s.Playlists)
                .ThenInclude(p => p.User)
                .Where(s => s.Playlists.Any(p => p.UserId == userId))
                .ToListAsync();

        }



    }
}
