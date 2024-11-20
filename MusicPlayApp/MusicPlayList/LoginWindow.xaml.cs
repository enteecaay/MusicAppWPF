using MusicPlayApp.BLL.Service;
using MusicPlayApp.DAL.Repository;
using System.Windows;
using MusicPlayApp.DAL;
using MusicPlayApp.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MusicPlayList
{
    public partial class LoginWindow : Window
    {
        private UserService _userService;
        // Constructor for XAML to initialize LoginWindow
        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService(new UserRepo());
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập cả tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Gọi hàm Validate để kiểm tra tài khoản
                bool isValidUser = await _userService.ValidateUserAsync(username, password);

                if (isValidUser)
                {
                    User loggedInUser = await _userService.AuthenticateAsync(username, password);
                    // Gọi hàm Authenticate để lấy thông tin người dùng

                    if (loggedInUser != null)
                    {
                        // Khởi tạo MainWindow và truyền thông tin người dùng
                        MainWindow mainWindow = new MainWindow(loggedInUser);
                        mainWindow.CurrentUser = loggedInUser;
                        mainWindow.Show();

                        // Đóng LoginWindow
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Đã xảy ra lỗi khi lấy thông tin người dùng. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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


        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
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
                var existingUser = await _userService.GetUserByUsernameAsync(username);
                if (existingUser != null)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên đăng nhập khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newUser = new MusicPlayApp.DAL.Entities.User
                {
                    UserName = username,
                    Password = password
                };

                await _userService.AddUserAsync(newUser);
                MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                User registerUser = await _userService.AuthenticateAsync(username, password);
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
