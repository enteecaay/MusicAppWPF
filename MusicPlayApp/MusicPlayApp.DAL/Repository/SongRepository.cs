using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicPlayApp.DAL.Entities;

namespace MusicPlayApp.DAL.Repository
{
    public class SongRepository
    {
        private const string FileName = "songs.json";

        public async Task<List<Song>> GetAllSongsAsync()
        {
            return await JsonDatabase.ReadAsync<Song>(FileName);
        }

        public async Task<List<Song>> GetSongsByUserIdAsync(int userId)
        {
            var songs = await JsonDatabase.ReadAsync<Song>(FileName);
            var playlists = await JsonDatabase.ReadAsync<Playlist>("playlists.json");

            return songs.Where(s => playlists.Any(p => p.UserId == userId && p.SongIds.Contains(s.SongId ?? 0))).ToList();
        }


        private int GetNextId(List<Song> songs)
        {
            return songs.Any() ? (songs.Max(s => s.SongId) ?? 0) + 1 : 1;
        }
        public async Task AddSongAsync(Song song)
        {
            song.SongId = GetNextId(await JsonDatabase.ReadAsync<Song>(FileName));
            var songs = await JsonDatabase.ReadAsync<Song>(FileName);
            songs.Add(song);
            await JsonDatabase.WriteAsync(FileName, songs);
        }

        public async Task UpdateSongAsync(Song song)
        {
            var songs = await JsonDatabase.ReadAsync<Song>(FileName);
            var existingSong = songs.FirstOrDefault(s => s.SongId == song.SongId);
            if (existingSong != null)
            {
                songs.Remove(existingSong);
                songs.Add(song);
                await JsonDatabase.WriteAsync(FileName, songs);
            }
        }
        public async Task RemoveSongAsync(Song song)
        {
            var songs = await JsonDatabase.ReadAsync<Song>(FileName);
            var existingSong = songs.FirstOrDefault(s => s.SongId == song.SongId);
            if (existingSong != null)
            {
                songs.Remove(existingSong);
                await JsonDatabase.WriteAsync(FileName, songs);
            }
        }
        public async Task<Song> GetSongByIdAsync(int id)
        {
            var songs = await JsonDatabase.ReadAsync<Song>(FileName);
            return songs.FirstOrDefault(s => s.SongId == id);
        }

        public async Task<List<Song>> SearchSongsAsync(string query)
        {
            var songs = await JsonDatabase.ReadAsync<Song>(FileName);
            return songs.Where(s => s.Title.ToLower().Contains(query) || s.Artist.ToLower().Contains(query)).ToList();
        }

    }
}
