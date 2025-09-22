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

    public ImmutableList<Song> Songs { get; private set; }

    public Song CreateSong(string name) => Song.CreateWithAuthors(name, [this]);

    public Artist AddSong(Song song)
    {
        Songs = Songs.Add(song);
        return this;
    }

    public Artist RemoveFromSong(Song song)
    {
        Songs = Songs.Where(s => s.Id != song.Id).ToImmutableList();
        return this;
    }
}
