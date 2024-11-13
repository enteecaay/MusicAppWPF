using MusicPlayApp.BLL.Service;
using MusicPlayApp.DLL.Repository;
using System.Windows;
using MusicPlayApp.DLL;
using MusicPlayApp.DLL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MusicPlayList
{
    public partial class LoginWindow : Window
    {
        private UserService _userService;
        private PlaylistService _playlistService = new PlaylistService();
        // Constructor for XAML to initialize LoginWindow
        public LoginWindow()
        {
            InitializeComponent();

            // Initialize MusicPlayListDbContext
            var context = new MusicPlayerAppContext();

            // Initialize UserRepo and UserService
            var userRepo = new UserRepo(context); // Initialize UserRepo with context
            _userService = new UserService(userRepo);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập cả tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Validate login credentials through UserService
                bool isValidUser = _userService.ValidateUser(username, password);

                if (isValidUser)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Open MainWindow and close LoginWindow


                    User loggedInUser = _userService.Authenticate(username, password);

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.CurrentUser = loggedInUser;
                    //mainWindow.LoadUserSongs();
                    mainWindow.Show();
                    
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập cả tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var existingUser = _userService.GetUserByUsername(username);
                if (existingUser != null)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên đăng nhập khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newUser = new MusicPlayApp.DLL.Entities.User
                {
                    UserName = username,
                    Password = password
                };

                _userService.AddUser(newUser);
                MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                User registerUser = _userService.Authenticate(username, password);
                FavoriteList favoriteList = new FavoriteList
                {
                    UserId = registerUser.UserId,
                    ListName = $"{username}"
                };
                try
                {
                    _playlistService.Create(favoriteList);
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException?.Message;
                    MessageBox.Show($"Đã xảy ra lỗi khi tạo playlist: {innerException}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UsernameTextBox.Clear();
                PasswordBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi đăng ký: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
