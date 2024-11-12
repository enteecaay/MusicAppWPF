using MusicPlayApp.DLL.Model;
using MusicPlayApp.DLL.Repository;
using System.Collections.Generic;

namespace MusicPlayApp.BLL.Service
{
    public class SongService
    {
        private readonly SongRepo _songRepo;

        // Inject SongRepo qua constructor
        public SongService(SongRepo songRepo)
        {
            _songRepo = songRepo;
        }

        // Lấy tất cả bài hát
        public IEnumerable<Song> GetAllSongs()
        {
            return _songRepo.GetAllSongs();
        }

        // Thêm bài hát mới
        public void AddSong(Song song)
        {
            _songRepo.AddSong(song);
        }

        // Cập nhật bài hát
        public void UpdateSong(Song song)
        {
            _songRepo.UpdateSong(song);
        }

        // Xóa bài hát theo ID
        public void DeleteSong(int id)
        {
            _songRepo.DeleteSong(id);
        }

        // Lấy bài hát theo ID
        public Song GetSongById(int id)
        {
            return _songRepo.GetSongById(id);
        }
    }
}
