using MusicPlayApp.BLL.Service;
using MusicPlayApp.DLL.Repository;
using System.Windows;
using MusicPlayApp.DLL;

namespace MusicPlayList
{
    public partial class LoginWindow : Window
    {
        private UserService _userService;

        // Constructor mặc định để XAML có thể khởi tạo LoginWindow
        public LoginWindow()
        {
            InitializeComponent();

            // Khởi tạo MusicPlayListDbContext
            var context = new MusicPlayListDbContext(); // Lưu ý: Bạn cần chỉnh sửa Connection String nếu cần

            // Khởi tạo UserRepo và UserService
            var userRepo = new UserRepo(context); // Khởi tạo UserRepo với context
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
                // Kiểm tra thông tin đăng nhập qua UserService
                bool isValidUser = _userService.ValidateUser(username, password);

                if (isValidUser)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở MainWindow và đóng LoginWindow
                    MainWindow mainWindow = new MainWindow();
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

                var newUser = new MusicPlayApp.DLL.Model.User
                {
                    Username = username,
                    Password = password
                };

                _userService.AddUser(newUser);
                MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

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
