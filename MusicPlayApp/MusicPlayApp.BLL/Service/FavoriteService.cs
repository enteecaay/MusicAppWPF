using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayApp.BLL.Service
{
    public class FavoriteService
    {
        private FavoriteRepo _favoriteRepo;

        public FavoriteService()
        {
            _favoriteRepo = new FavoriteRepo();
        }

        public async Task AddFavoriteAsync(int userId, int songId, string listName)
        {
            await _favoriteRepo.AddFavoriteAsync(userId, songId, listName);
        }
        // Kiểm tra xem bài hát đã có trong danh sách yêu thích của người dùng chưa
        public async Task<bool> CheckIfFavoriteExistsAsync(int userId, int songId)
        {
            var favorite = await _favoriteRepo.GetFavoriteByUserIdAndSongIdAsync(userId, songId);
            return favorite != null;
        }
        public async Task<List<Song>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _favoriteRepo.GetFavoritesByUserIdAsync(userId);
        }

        public async Task RemoveFavoriteAsync(int userId, int songId)
        {
            FavoriteRepo favoriteRepo = new FavoriteRepo();
            await favoriteRepo.RemoveFavoriteAsync(userId, songId);
        }

    }
}
