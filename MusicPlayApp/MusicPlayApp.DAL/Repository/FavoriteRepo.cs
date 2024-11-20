using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayApp.DAL.Repository
{
    public class FavoriteRepo
    {
        private const string FileName = "favoriteLists.json";
        //private int GetNextId(List<FavoriteList> favoritelists)
        //{
        //    return favoritelists.Any() ? (favoritelists.Max(f => f.FavoriteListId) ?? 0) + 1 : 1;
        //}
        public async Task AddFavoriteAsync(int userId, int songId, string listName)
        {
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);

            var existingFavorite = favoriteLists.FirstOrDefault(f => f.UserId == userId && f.SongId == songId);
            if (existingFavorite != null)
            {
                throw new InvalidOperationException("This song is already in your favorite list.");
            }

            var favorite = new FavoriteList
            {
                UserId = userId,
                SongId = songId,
                ListName = listName
            };

            favoriteLists.Add(favorite);
            await JsonDatabase.WriteAsync(FileName, favoriteLists);
        }

        // Kiểm tra xem bài hát đã có trong danh sách yêu thích của người dùng chưa
        public async Task<FavoriteList> GetFavoriteByUserIdAndSongIdAsync(int userId, int songId)
        {
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);
            return favoriteLists.FirstOrDefault(f => f.UserId == userId && f.SongId == songId);
        }


        public async Task<List<int?>> GetFavoritesByUserIdAsync(int userId)
        {
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);
            return favoriteLists
                .Where(f => f.UserId == userId)
                .Select(f => f.SongId)
                .ToList();
        }

        public async Task RemoveFavoriteAsync(int userId, int songId)
        {

            // Tìm mục yêu thích dựa trên UserId và SongId
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);
            var favorite = favoriteLists.FirstOrDefault(f => f.UserId == userId && f.SongId == songId);

            if (favorite != null)
            {
                // Xóa mục khỏi FavoriteLists
                favoriteLists.Remove(favorite);
                await JsonDatabase.WriteAsync(FileName, favoriteLists);
                // Lưu thay đổi vào cơ sở dữ liệu
            }
        }
    }
}
