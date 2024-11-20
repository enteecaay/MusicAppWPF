using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayApp.DAL.Repository
{
    public class UserRepo
    {
        private const string FileName = "users.json";

        // Lấy người dùng bằng Id

        private int GetNextId(List<User> users)
        {
            return users.Any() ? users.Max(u => u.UserId) + 1 : 1;
        }


        public async Task<List<User>> GetAllUsersAsync()
        {
            return await JsonDatabase.ReadAsync<User>(FileName);
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            var users = await JsonDatabase.ReadAsync<User>(FileName);
            return users.FirstOrDefault(u => u.UserId == id);
        }

        // Lấy người dùng bằng Username
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var users = await JsonDatabase.ReadAsync<User>(FileName);
            return users.FirstOrDefault(u => u.UserName == username);
        }

        // Thêm người dùng mới

        public async Task AddUserAsync(User user)
        {
            user.UserId = GetNextId(await JsonDatabase.ReadAsync<User>(FileName));
            var users = await JsonDatabase.ReadAsync<User>(FileName);
            var existingUser = users.FirstOrDefault(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this username already exists.");
            }
            users.Add(user);
            await JsonDatabase.WriteAsync(FileName, users);
        }

        // Kiểm tra thông tin đăng nhập
        public bool ValidateUser(string username, string password)
        {
            var users = JsonDatabase.ReadAsync<User>(FileName).Result;
            return users.Any(u => u.UserName == username && u.Password == password);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var users = await JsonDatabase.ReadAsync<User>(FileName);
            return users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower() && u.Password == password);
        }
    }
}
