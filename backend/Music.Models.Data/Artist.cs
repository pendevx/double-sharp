using System.Collections.Immutable;
using Music.Models.Data.Utils;

namespace Music.Models.Data;

public class Artist : BaseEntity
{
    protected Artist() { }

    protected Artist(string name) => Name = name;
    public static Artist SuggestNew(string name, DateOnly dateOfBirth, Account requester) => new()
    {
        Name = name,
        DateOfBirth = dateOfBirth,
        _requestInformation = Data.RequestInformation.Create(requester)
    };

    public string Name { get; init; }
    public DateOnly DateOfBirth { get; set; }

    protected RequestInformation? _requestInformation { get; private init; }
    public OptionType<RequestInformation> RequestInformation => _requestInformation.ToOption();

    public virtual ImmutableList<Song> Songs { get; private set; }

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
