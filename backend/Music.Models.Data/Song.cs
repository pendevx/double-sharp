using System.Collections.Immutable;

namespace Music.Models.Data;

public class Song : BaseEntity
{
    protected Song() { }

    private Song(string name, ImmutableList<Artist> artists) => (Name, Artists) = (name, artists);
    internal static Song Create(string name) => new(name, []);
    internal static Song CreateWithAuthors(string name, ImmutableList<Artist> artists) => new(name, artists);

    public string Name { get; init; }
    public ImmutableList<Artist> Artists { get; set; }
}
