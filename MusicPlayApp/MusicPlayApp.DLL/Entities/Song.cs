﻿using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Entities;

public partial class Song
{
    public int SongId { get; set; }

    public string Title { get; set; } = null!;

    public string Artist { get; set; } = null!;

    public string? Album { get; set; }

    public virtual ICollection<FavoriteList> FavoriteLists { get; set; } = new List<FavoriteList>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
