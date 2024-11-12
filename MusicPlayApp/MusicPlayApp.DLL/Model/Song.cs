using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Model;

public partial class Song
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Artist { get; set; } = null!;

    public string? Album { get; set; }

    public string Url { get; set; } = null!;

    public virtual ICollection<FavoriteMusic> FavoriteMusics { get; set; } = new List<FavoriteMusic>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
