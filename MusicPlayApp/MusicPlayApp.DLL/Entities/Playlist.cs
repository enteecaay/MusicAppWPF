using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Entities;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public int UserId { get; set; }

    public int SongId { get; set; }

    public string PlaylistName { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
