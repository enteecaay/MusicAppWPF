using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DLL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.DLL.Repository
{
    public class UserRepo
    {
        private MusicPlayListDbContext _context;

        public User GetUserById(int id)
        {
            _context = new();
            return _context.Users.Include(u => u.Playlists).Include(u => u.FavoriteMusics).FirstOrDefault(u => u.Id == id);
        }
        public User GetUserByUsername(string username)
        {
            _context = new();
            if (_context == null || _context.Users == null)
            {
                throw new InvalidOperationException("Database context or Users collection is not initialized.");
            }

            return _context.Users
                           .Include(u => u.Playlists)
                           .Include(u => u.FavoriteMusics)
                           .FirstOrDefault(u => u.Username == username);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
