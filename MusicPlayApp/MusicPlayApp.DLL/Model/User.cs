using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Model;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<FavoriteMusic> FavoriteMusics { get; set; } = new List<FavoriteMusic>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
