using System.Collections.Immutable;
using Music.Models.Data.Errors;
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
        RequestInformation = Data.RequestInformation.Create(requester).ToOption()
    };

    public string Name { get; init; }
    public DateOnly DateOfBirth { get; set; }

    private RequestInformation? _requestInformation { get; set; }

    public OptionType<RequestInformation> RequestInformation
    {
        get => _requestInformation.ToOption();
        set => _requestInformation = value.ToNullable();
    }

    public virtual ImmutableList<Song> Songs { get; private set; }

    public Song CreateSong(string name) => Song.CreateWithAuthors(name, [this]);

    public ResultType<Artist, ResultError> TryApproveSuggestion() =>
        RequestInformation.Match(
            reqInfo => reqInfo.Approve().Map(approved =>
            {
                RequestInformation = Option.Some(approved);
                return this;
            }),
            () => Result.Fail<Artist, ResultError>(new FailedOperationError("No request information found."))
        );

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
