using System;
using System.Collections.Generic;

namespace MusicPlayApp.DLL.Entities;

public partial class FavoriteList
{
    public int FavoriteListId { get; set; }

    public int UserId { get; set; }

    public int SongId { get; set; }

    public string ListName { get; set;}

    public virtual Song Song { get; set; }

    public virtual User User { get; set; }
}
