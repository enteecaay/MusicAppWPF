using System;
using System.Collections.Generic;

namespace MusicPlayApp.DAL.Entities;

public partial class Playlist
{
    public int PlaylistId { get; set; }
    public int UserId { get; set; }
    public string PlaylistName { get; set; }
    public List<int> SongIds { get; set; } = new List<int>();
}
