using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DLL.Entities;
using System;
using System.Linq;

namespace MusicPlayApp.DLL.Repository
{
    public class UserRepo
    {
        private readonly MusicPlayerAppContext _context;

        // Inject DbContext qua constructor để quản lý vòng đời của nó
        public UserRepo(MusicPlayerAppContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy người dùng bằng Id
        public User GetUserById(int id)
        {
            return _context.Users
                           .Include(u => u.Playlists)
                           .Include(u => u.FavoriteLists)
                           .FirstOrDefault(u => u.UserId == id);
        }

        // Lấy người dùng bằng Username
        public User GetUserByUsername(string username)
        {
            if (_context == null || _context.Users == null)
            {
                throw new InvalidOperationException("Database context or Users collection is not initialized.");
            }

            return _context.Users
                           .Include(u => u.Playlists)
                           .Include(u => u.FavoriteLists)
                           .FirstOrDefault(u => u.UserName == username);
        }

        // Thêm người dùng mới
        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (existingUser != null)
                throw new InvalidOperationException("A user with this username already exists.");

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the user. Inner exception: " + ex.InnerException?.Message, ex);
            }
        }

        // Kiểm tra thông tin đăng nhập
        public bool ValidateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            return user != null;
        }
    }
}
