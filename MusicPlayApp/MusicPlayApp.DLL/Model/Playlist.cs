using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Model
{
    public partial class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        // Các bài hát trong playlist thông qua PlaylistSongs
        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
        public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>(); // Thêm thuộc tính này
    }
}
