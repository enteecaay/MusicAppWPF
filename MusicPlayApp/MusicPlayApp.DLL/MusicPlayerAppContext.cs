using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MusicPlayApp.DLL.Entities;

namespace MusicPlayApp.DLL;

public partial class MusicPlayerAppContext : DbContext
{
    public MusicPlayerAppContext()
    {
    }

    public MusicPlayerAppContext(DbContextOptions<MusicPlayerAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FavoriteList> FavoriteLists { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI;uid=sa;pwd=12345;database= MusicPlayerApp;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FavoriteList>(entity =>
        {
            entity.HasKey(e => e.FavoriteListId).HasName("PK__Favorite__007A2DF668BF85D4");

            entity.Property(e => e.ListName).HasMaxLength(100);

            entity.HasOne(d => d.Song).WithMany(p => p.FavoriteLists)
                .HasForeignKey(d => d.SongId)
                .HasConstraintName("FK__FavoriteL__SongI__403A8C7D");

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__FavoriteL__UserI__3F466844");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("PK__Playlist__B30167A05122256A");

            entity.Property(e => e.PlaylistName).HasMaxLength(100);

            entity.HasOne(d => d.Song).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.SongId)
                .HasConstraintName("FK__Playlists__SongI__3C69FB99");

            entity.HasOne(d => d.User).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Playlists__UserI__3B75D760");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PK__Songs__12E3D697540FC9C7");

            entity.Property(e => e.Album).HasMaxLength(100);
            entity.Property(e => e.Artist).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CE259796E");

            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
