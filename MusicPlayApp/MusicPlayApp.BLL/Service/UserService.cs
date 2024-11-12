using MusicPlayApp.DLL.Model;
using MusicPlayApp.DLL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayApp.BLL.Service
{
    public class UserService
    {
        private UserRepo _userrepo = new UserRepo();

        public User getUserById(int id)
        {
            return _userrepo.GetUserById(id);
        }

        public User GetUserByUsername(string username)
        {
            return _userrepo.GetUserByUsername(username);
        }

        public void AddUser(User user)
        {
            _userrepo.AddUser(user);
        }
    }
}
