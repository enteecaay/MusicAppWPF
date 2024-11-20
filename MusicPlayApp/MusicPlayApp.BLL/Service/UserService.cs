using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;
using System;
using System.Threading.Tasks;

namespace MusicPlayApp.BLL.Service
{
    public class UserService
    {
        private readonly UserRepo _userRepo;

        // Inject UserRepo through constructor
        public UserService(UserRepo userRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        // Get user by Id
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepo.GetUserByIdAsync(id);
        }

        // Get user by Username
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepo.GetUserByUsernameAsync(username);
        }

        // Add new user
        public async Task AddUserAsync(User user)
        {
            await _userRepo.AddUserAsync(user);
        }

        // Validate user credentials
        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepo.GetUserByUsernameAsync(username);
            if (user == null || user.Password != password)
            {
                return false;
            }
            return true;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepo.GetUserByUsernameAsync(username);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return null;
            }
            if (user.Password != password)
            {
                Console.WriteLine("Password does not match.");
                return null;
            }
            return user;
        }
    }
}
