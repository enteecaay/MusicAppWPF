using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DLL.Model;
using System;
using System.Linq;

namespace MusicPlayApp.DLL.Repository
{
    public class UserRepo
    {
        private readonly MusicPlayListDbContext _context;

        // Inject DbContext qua constructor để quản lý vòng đời của nó
        public UserRepo(MusicPlayListDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy người dùng bằng Id
        public User GetUserById(int id)
        {
            return _context.Users
                           .Include(u => u.Playlists)
                           .Include(u => u.FavoriteMusics)
                           .FirstOrDefault(u => u.Id == id);
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
                           .Include(u => u.FavoriteMusics)
                           .FirstOrDefault(u => u.Username == username);
        }

        // Thêm người dùng mới
        public void AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // Kiểm tra thông tin đăng nhập
        public bool ValidateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            return user != null;
        }
    }
}
