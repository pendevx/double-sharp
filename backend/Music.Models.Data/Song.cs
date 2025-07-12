namespace Music.Models.Data;

public class Song : BaseEntity
{
    private Song(string name, string mimeType) =>
        (Name, MimeType) = (name, mimeType);

    public string Name { get; init; }
    public string MimeType { get; init; }

    public static Song Create(string name, string mimeType) =>
        new(name, mimeType);
}
