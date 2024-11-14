🎶 MusicAppWPF 🎬

Overview 📖
MusicAppWPF is a Windows-based music and video player application built with WPF, structured on a three-tier architecture for better separation of concerns and maintainability. It allows users to enjoy a seamless media playback experience with features like playlist management, favorite lists, volume control, and more.

Features 🌟
🎵 Play and Pause Music/Video: Smooth playback of audio and video files with essential controls like play, pause, next, previous, and stop.
🔀 Shuffle and Sequential Modes: Toggle between shuffle and sequential playback for a personalized listening experience.
🔊 Volume Control: Adjust the volume level with an easy-to-use slider.
❤️ Favorites Management: Add or remove songs and videos to/from your favorites list.
🖥️ Full-Screen Mode: Enjoy full-screen video playback with a double-click toggle.
💿 Disk Animation: Real-time rotating CD icon animation for audio playback.
🔐 User Authentication: Requires user login for secure access to features.
Project Structure 🏗️
This application follows a three-tier architecture:

Presentation Layer (UI - WPF Application) 🖌️

Manages the user interface and handles user interactions.
Utilizes WPF components like ListBox, Button, and Slider to offer a responsive and user-friendly design.
Business Logic Layer (BLL) ⚙️

Implements the core logic for managing playlists, favorites, and playback modes.
Contains services like SongService and FavoriteService to handle operations related to media management.
Data Access Layer (DAL) 🗄️

Handles data storage, retrieval, and updates.
Uses classes such as Song and User to map database entities and perform CRUD operations.
Key Files & Code Structure 📂
MainWindow.xaml.cs: Main event handler file, managing play, pause, deletion, and song selection. It also initializes the app and verifies user authentication.
SongService and FavoriteService: Core business logic for managing songs and favorites, interfacing with the DAL for data retrieval and modification.
Entities (Song, User): Data structures representing the application's core models, aligning with the database schema.
Installation 🛠️
Clone the Repository 📥
bash
Copy code
git clone https://github.com/yourusername/MusicAppWPF.git
Open the Project in Visual Studio.
Restore NuGet Packages to resolve dependencies.
Build and Run the application to start using it.
Usage 🎧
Launch the Application.
Log In if prompted to access full features.
Import Music/Video files to your playlist.
Playback Controls: Use the play, pause, skip, and stop controls to manage playback.
Favorites: Add songs/videos to your favorites list by selecting them and clicking "Add to Favorite."
Shuffle/Sequential: Switch between playback modes for a customized experience.
Future Enhancements 🚀
🌐 Additional Format Support: Expand compatibility with more media formats.
🔍 Search Functionality: Add search within playlists and favorites for quick access.
🔒 Role-Based Authentication: Implement enhanced authentication features with different user roles.
