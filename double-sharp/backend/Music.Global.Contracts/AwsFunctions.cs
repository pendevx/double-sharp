namespace Music.Global.Contracts;

public delegate string GetSongPath(int songId);
public delegate string GetBucketName();
public delegate string CreateSongRequestPath(Guid songRequestId);
