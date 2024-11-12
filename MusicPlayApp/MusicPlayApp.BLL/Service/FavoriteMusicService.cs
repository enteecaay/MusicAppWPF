using MusicPlayApp.DLL.Model;
using MusicPlayApp.DLL.Repository;
using System.Collections.Generic;

namespace MusicPlayApp.BLL.Service
{
    public class FavoriteMusicService
    {
        private readonly FavoriteMusicsRepo _favoriteMusicsRepo;

        // Inject FavoriteMusicsRepo thông qua constructor
        public FavoriteMusicService(FavoriteMusicsRepo favoriteMusicsRepo)
        {
            _favoriteMusicsRepo = favoriteMusicsRepo;
        }

        // Lấy danh sách bài hát yêu thích của một người dùng
        public IEnumerable<Song> GetFavoriteSongsByUserId(int userId)
        {
            return _favoriteMusicsRepo.GetFavoriteSongsByUserId(userId);
        }

        // Thêm một bài hát vào danh sách yêu thích
        public void AddToFavorite(int userId, int songId)
        {
            _favoriteMusicsRepo.AddToFavorite(userId, songId);
        }

        // Xóa một bài hát khỏi danh sách yêu thích
        public void RemoveFromFavorite(int userId, int songId)
        {
            _favoriteMusicsRepo.RemoveFromFavorite(userId, songId);
        }

        // Kiểm tra xem một bài hát có trong danh sách yêu thích của người dùng không
        public bool IsFavorite(int userId, int songId)
        {
            return _favoriteMusicsRepo.IsFavorite(userId, songId);
        }
    }
}
