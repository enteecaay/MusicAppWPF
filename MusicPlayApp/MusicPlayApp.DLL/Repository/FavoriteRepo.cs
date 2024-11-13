using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DLL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayApp.DLL.Repository
{
    public class FavoriteRepo
    {
        private MusicPlayerAppContext _context;

        public FavoriteRepo()
        {
            _context = new MusicPlayerAppContext();
        }

        public async Task AddFavoriteAsync(int userId, int songId, string listName)
        {
            var favorite = new FavoriteList
            {
                UserId = userId,
                SongId = songId,
                ListName = listName
            };

            _context.FavoriteLists.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Song>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _context.FavoriteLists
                .Include(f => f.Song)
                .Where(f => f.UserId == userId)
                .Select(f => f.Song) // Lấy bài hát liên quan
                .ToListAsync();
        }

        public async Task RemoveFavoriteAsync(int userId, int songId)
        {
            // Tìm mục yêu thích dựa trên UserId và SongId
            var favorite = await _context.FavoriteLists
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

            if (favorite != null)
            {
                // Xóa mục khỏi FavoriteLists
                _context.FavoriteLists.Remove(favorite);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
            }
        }
    }
}
