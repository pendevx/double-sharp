using Music.Models.Data.Utils;

namespace Music.Models.Data.Errors;

public sealed class NotFoundError : ResultError;

public sealed class FailedOperationError(string Reason) : ResultError;
