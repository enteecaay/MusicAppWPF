using MusicPlayApp.DLL.Entities;
using MusicPlayApp.DLL.Repository;
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
