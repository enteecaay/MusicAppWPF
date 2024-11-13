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
            // Kiểm tra xem bài hát đã tồn tại trong danh sách yêu thích của người dùng chưa
            var existingFavorite = await _context.FavoriteLists
                                                 .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

            if (existingFavorite != null)
            {
                // Nếu bài hát đã có trong danh sách yêu thích, ném lỗi
                throw new InvalidOperationException("This song is already in your favorite list.");
            }

            // Nếu chưa có, tiếp tục thêm bài hát vào danh sách yêu thích
            var favorite = new FavoriteList
            {
                UserId = userId,
                SongId = songId,
                ListName = listName
            };

            _context.FavoriteLists.Add(favorite);
            await _context.SaveChangesAsync();
        }

        // Kiểm tra xem bài hát đã có trong danh sách yêu thích của người dùng chưa
        public async Task<FavoriteList> GetFavoriteByUserIdAndSongIdAsync(int userId, int songId)
        {
            return await _context.FavoriteLists
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);
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
