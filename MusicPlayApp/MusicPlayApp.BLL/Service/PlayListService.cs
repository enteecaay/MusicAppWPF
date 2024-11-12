using MusicPlayApp.DLL.Model;
using MusicPlayApp.DLL.Repository;
using System.Collections.Generic;

namespace MusicPlayApp.BLL.Service
{
    public class PlayListService
    {
        private readonly PlayListRepo _playlistRepo;

        // Inject PlayListRepo thông qua constructor
        public PlayListService(PlayListRepo playlistRepo)
        {
            _playlistRepo = playlistRepo;
        }

        // Lấy tất cả playlist của người dùng
        public IEnumerable<Playlist> GetPlaylistsByUserId(int userId)
        {
            return _playlistRepo.GetPlaylistsByUserId(userId);
        }

        // Lấy một playlist theo Id
        public Playlist GetPlaylistById(int id)
        {
            return _playlistRepo.GetPlaylistById(id);
        }

        // Thêm mới một playlist
        public void AddPlaylist(Playlist playlist)
        {
            _playlistRepo.AddPlaylist(playlist);
        }

        // Cập nhật thông tin playlist
        public void UpdatePlaylist(Playlist playlist)
        {
            _playlistRepo.UpdatePlaylist(playlist);
        }

        // Xóa một playlist theo Id
        public void DeletePlaylist(int id)
        {
            _playlistRepo.DeletePlaylist(id);
        }

        // Lấy tất cả bài hát trong một playlist
        public IEnumerable<Song> GetSongsInPlaylist(int playlistId)
        {
            return _playlistRepo.GetSongsInPlaylist(playlistId);
        }

        // Thêm bài hát vào playlist
        public void AddSongToPlaylist(int playlistId, int songId)
        {
            _playlistRepo.AddSongToPlaylist(playlistId, songId);
        }

        // Xóa bài hát khỏi playlist
        public void RemoveSongFromPlaylist(int playlistId, int songId)
        {
            _playlistRepo.RemoveSongFromPlaylist(playlistId, songId);
        }
    }
}
