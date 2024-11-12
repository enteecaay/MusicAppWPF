using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Entities;

public partial class Song
{
    public int SongId { get; set; }

    public string Title { get; set; }

    public string Artist { get; set; }

    public string? Album { get; set; } //url

    public virtual ICollection<FavoriteList> FavoriteLists { get; set; } = new List<FavoriteList>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
