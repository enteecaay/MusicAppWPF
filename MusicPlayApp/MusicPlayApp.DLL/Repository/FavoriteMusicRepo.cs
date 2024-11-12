using MusicPlayApp.DLL.Model;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayApp.DLL.Repository
{
    public class FavoriteMusicsRepo
    {
        private readonly MusicPlayListDbContext _context;

        // Inject DbContext qua constructor
        public FavoriteMusicsRepo(MusicPlayListDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách bài hát yêu thích của một người dùng
        public IEnumerable<Song> GetFavoriteSongsByUserId(int userId)
        {
            return _context.FavoriteMusics
                .Where(f => f.UserId == userId)
                .Select(f => f.Song)
                .ToList();
        }

        // Thêm một bài hát vào danh sách yêu thích
        public void AddToFavorite(int userId, int songId)
        {
            // Kiểm tra nếu bài hát đã có trong danh sách yêu thích
            var existingFavorite = _context.FavoriteMusics
                .FirstOrDefault(f => f.UserId == userId && f.SongId == songId);

            if (existingFavorite == null)
            {
                var favoriteMusic = new FavoriteMusic
                {
                    UserId = userId,
                    SongId = songId
                };

                _context.FavoriteMusics.Add(favoriteMusic);
                _context.SaveChanges();
            }
        }

        // Xóa một bài hát khỏi danh sách yêu thích
        public void RemoveFromFavorite(int userId, int songId)
        {
            var favoriteMusic = _context.FavoriteMusics
                .FirstOrDefault(f => f.UserId == userId && f.SongId == songId);

            if (favoriteMusic != null)
            {
                _context.FavoriteMusics.Remove(favoriteMusic);
                _context.SaveChanges();
            }
        }

        // Kiểm tra xem một bài hát có trong danh sách yêu thích của người dùng không
        public bool IsFavorite(int userId, int songId)
        {
            return _context.FavoriteMusics
                .Any(f => f.UserId == userId && f.SongId == songId);
        }
    }
}
