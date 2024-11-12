using MusicPlayApp.DLL.Entities;
using MusicPlayApp.DLL.Entities;
using MusicPlayApp.DLL.Repository;
using System;

namespace MusicPlayApp.BLL.Service
{
    public class UserService
    {
        private readonly UserRepo _userRepo;

        // Inject UserRepo thông qua constructor
        public UserService(UserRepo userRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        // Lấy người dùng bằng Id
        public User GetUserById(int id)
        {
            return _userRepo.GetUserById(id);
        }

        // Lấy người dùng bằng Username
        public User GetUserByUsername(string username)
        {
            return _userRepo.GetUserByUsername(username);
        }

        // Thêm người dùng mới
        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            _userRepo.AddUser(user);
        }

        // Kiểm tra thông tin đăng nhập
        public bool ValidateUser(string username, string password)
        {
            return _userRepo.ValidateUser(username, password);
        }

        public User Authenticate(string username, string password)
        {
            return _userRepo.Authenticate(username, password);
        }
    }
}
