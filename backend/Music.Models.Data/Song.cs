namespace Music.Models.Data;

public class Song : BaseEntity
{
    protected Song() { }

    private Song(string name) => Name = name;

    public string Name { get; init; }

    public static Song Create(string name) =>
        new(name);
}
