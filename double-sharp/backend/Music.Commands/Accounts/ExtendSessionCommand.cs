namespace Music.Commands.Accounts;

public record ExtendSessionCommand(Guid Token, int ExtensionSeconds);
