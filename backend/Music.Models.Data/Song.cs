using System.Collections.Immutable;

namespace Music.Models.Data;

public class Song : BaseEntity
{
    protected Song() { }

    private Song(string name, ICollection<Artist> artists) => (Name, _artists) = (name, artists);
    internal static Song Create(string name) => new(name, []);
    internal static Song CreateWithAuthors(string name, ICollection<Artist> artists) => new(name, artists);

    public string Name { get; init; }

    protected virtual ICollection<Artist> _artists { get; private set; }
    public ImmutableList<Artist> Artists => _artists.ToImmutableList();

    internal bool TryAddArtist(Artist artist)
    {
        if (Artists.Contains(artist)) return false;

        _artists.Add(artist);
        return true;
    }

    internal bool TryRemoveArtist(Artist artist)
    {
        if (!Artists.Contains(artist)) return false;

        _artists.Remove(artist);
        return true;
    }
}
