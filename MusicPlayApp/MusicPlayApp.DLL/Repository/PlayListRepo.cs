using MusicPlayApp.DLL.Model;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayApp.DLL.Repository
{
    public class PlayListRepo
    {
        private readonly MusicPlayListDbContext _context;

        // Inject DbContext qua constructor
        public PlayListRepo(MusicPlayListDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả playlist của một người dùng cụ thể
        public IEnumerable<Playlist> GetPlaylistsByUserId(int userId)
        {
            return _context.Playlists.Where(p => p.UserId == userId).ToList();
        }

        // Lấy một playlist theo Id
        public Playlist GetPlaylistById(int id)
        {
            return _context.Playlists.Find(id);
        }

        // Thêm mới playlist
        public void AddPlaylist(Playlist playlist)
        {
            _context.Playlists.Add(playlist);
            _context.SaveChanges();
        }

        // Cập nhật thông tin playlist
        public void UpdatePlaylist(Playlist playlist)
        {
            var existingPlaylist = _context.Playlists.Find(playlist.Id);
            if (existingPlaylist != null)
            {
                existingPlaylist.Name = playlist.Name;
                existingPlaylist.UserId = playlist.UserId;
                _context.SaveChanges();
            }
        }

        // Xóa một playlist theo Id
        public void DeletePlaylist(int id)
        {
            var playlist = _context.Playlists.Find(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                _context.SaveChanges();
            }
        }

        // Lấy tất cả bài hát trong một playlist
        public IEnumerable<Song> GetSongsInPlaylist(int playlistId)
        {
            return _context.PlaylistSongs
                .Where(ps => ps.PlaylistId == playlistId)
                .Select(ps => ps.Song)
                .ToList();
        }

        // Thêm bài hát vào playlist
        public void AddSongToPlaylist(int playlistId, int songId)
        {
            // Kiểm tra nếu bài hát đã có trong playlist
            var existingEntry = _context.PlaylistSongs
                .FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (existingEntry == null)
            {
                var playlistSong = new PlaylistSong
                {
                    PlaylistId = playlistId,
                    SongId = songId
                };

                _context.PlaylistSongs.Add(playlistSong);
                _context.SaveChanges();
            }
        }

        // Xóa bài hát khỏi playlist
        public void RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var playlistSong = _context.PlaylistSongs
                .FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistSong != null)
            {
                _context.PlaylistSongs.Remove(playlistSong);
                _context.SaveChanges();
            }
        }
    }
}
