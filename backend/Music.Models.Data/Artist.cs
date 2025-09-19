using System.Collections.Immutable;

namespace Music.Models.Data;

public class Artist : BaseEntity
{
    private Artist() { }

    private Artist(string name) => Name = name;
    public static Artist Create(string name) => new(name);

    public string Name { get; init; }
    public DateOnly DateOfBirth { get; set; }

    public virtual ArtistRequest? ArtistRequest { get; set; }

    protected virtual ICollection<Song> _songs { get; private set; }
    public IImmutableList<Song> Songs => _songs
        .OrderBy(song => song.Name, StringComparer.InvariantCultureIgnoreCase)
        .ToImmutableList();

    public Song CreateSong(string name)
    {
        var song = Song.CreateWithAuthors(name, [this]);
        _songs.Add(song);
        return song;
    }

    public void AddToSong(Song song)
    {
        if (song.TryAddArtist(this))
            _songs = _songs.Any(s => s.Id == song.Id) ? throw new Exception() : _songs.Concat([ song ]).ToList();
    }

    public void RemoveFromSong(Song song)
    {
        if (song.TryRemoveArtist(this))
            _songs = _songs.All(s => s.Id != song.Id) ? throw new Exception() : _songs.Where(s => s != song).ToList();
    }
}
