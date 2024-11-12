using MusicPlayApp.DLL.Model;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayApp.DLL.Repository
{
    public class SongRepo
    {
        private readonly MusicPlayListDbContext _context;

        // Inject DbContext thông qua constructor
        public SongRepo(MusicPlayListDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả bài hát
        public IEnumerable<Song> GetAllSongs()
        {
            return _context.Songs.ToList();
        }

        // Thêm bài hát mới
        public void AddSong(Song song)
        {
            _context.Songs.Add(song);
            _context.SaveChanges();
        }

        // Lấy bài hát theo ID
        public Song GetSongById(int id)
        {
            return _context.Songs.Find(id);
        }

        // Xóa bài hát theo ID
        public void DeleteSong(int id)
        {
            var song = _context.Songs.Find(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
                _context.SaveChanges();
            }
        }

        public void UpdateSong(Song song)
        {
            var existingSong = _context.Songs.Find(song.Id);
            if (existingSong != null)
            {
                existingSong.Title = song.Title;
                existingSong.Artist = song.Artist;
                existingSong.Album = song.Album;
                existingSong.Url = song.Url;

                _context.SaveChanges();
            }
        }

    }
}
