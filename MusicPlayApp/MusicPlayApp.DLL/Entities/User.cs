using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public virtual ICollection<FavoriteList> FavoriteLists { get; set; } = new List<FavoriteList>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
