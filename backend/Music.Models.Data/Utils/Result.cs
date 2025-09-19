namespace Music.Models.Data.Utils;

public abstract record ResultType<TValue, TError>();
public sealed record SuccessResultType<TValue, TError>(TValue Value) : ResultType<TValue, TError>;
public sealed record FailedResultType<TValue, TError>(TError Error) : ResultType<TValue, TError>;

public static class Result
{
    public static SuccessResultType<TValue, TError> Ok<TValue, TError>(TValue value) => new(value);
    public static FailedResultType<TValue, TError> Fail<TValue, TError>(TError error) => new(error);

    public static ResultType<TNextValue, TError> Map<TValue, TNextValue, TError>(
        this ResultType<TValue, TError> @this, Func<TValue, TNextValue> map) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => Ok<TNextValue, TError>(map(success.Value)),
            FailedResultType<TValue, TError> failed => Fail<TNextValue, TError>(failed.Error),
            _ => throw new Exception()
        };

    public static ResultType<TNextValue, TError> Bind<TValue, TNextValue, TError>(
        this ResultType<TValue, TError> @this, Func<TValue, ResultType<TNextValue, TError>> bind) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => bind(success.Value),
            FailedResultType<TValue, TError> failed => Fail<TNextValue, TError>(failed.Error),
            _ => throw new Exception()
        };

    public static ResultType<TValue, TNextError> MapError<TValue, TError, TNextError>(
        this ResultType<TValue, TError> @this, Func<TError, TNextError> map) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => Ok<TValue, TNextError>(success.Value),
            FailedResultType<TValue, TError> failed => Fail<TValue, TNextError>(map(failed.Error)),
            _ => throw new Exception()
        };

    public static TResult Match<TValue, TError, TResult>(
        this ResultType<TValue, TError> @this, Func<TValue, TResult> ok, Func<TError, TResult> fail) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => ok(success.Value),
            FailedResultType<TValue, TError> failed => fail(failed.Error),
            _ => throw new Exception()
        };
}

public static class AsyncResult
{
    public static async Task<ResultType<TNextValue, TError>> MapAsync<TValue, TNextValue, TError>(
        this ResultType<TValue, TError> @this, Func<TValue, Task<TNextValue>> map) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => Result.Ok<TNextValue, TError>(await map(success.Value)),
            FailedResultType<TValue, TError> failed => Result.Fail<TNextValue, TError>(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<ResultType<TNextValue, TError>> MapAsync<TValue, TNextValue, TError>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, TNextValue> map) =>
        await @this switch
        {
            SuccessResultType<TValue, TError> success => Result.Ok<TNextValue, TError>(map(success.Value)),
            FailedResultType<TValue, TError> failed => Result.Fail<TNextValue, TError>(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<ResultType<TNextValue, TError>> MapAsync<TValue, TNextValue, TError>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, Task<TNextValue>> map) =>
        await (await @this).MapAsync(map);

    public static async Task<ResultType<TNextValue, TError>> BindAsync<TValue, TNextValue, TError>(
        this ResultType<TValue, TError> @this, Func<TValue, Task<ResultType<TNextValue, TError>>> bind) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => await bind(success.Value),
            FailedResultType<TValue, TError> failed => Result.Fail<TNextValue, TError>(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<ResultType<TNextValue, TError>> BindAsync<TValue, TNextValue, TError>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, ResultType<TNextValue, TError>> bind) =>
        await @this switch
        {
            SuccessResultType<TValue, TError> success => bind(success.Value),
            FailedResultType<TValue, TError> failed => Result.Fail<TNextValue, TError>(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<ResultType<TNextValue, TError>> BindAsync<TValue, TNextValue, TError>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, Task<ResultType<TNextValue, TError>>> bind) =>
        await (await @this).BindAsync(bind);

    public static Task<ResultType<TValue, TNextError>> MapErrorAsync<TValue, TError, TNextError>(
        this ResultType<TValue, TError> @this, Func<TError, TNextError> map) =>
        Task.FromResult<ResultType<TValue, TNextError>>(@this switch
        {
            SuccessResultType<TValue, TError> success => Result.Ok<TValue, TNextError>(success.Value),
            FailedResultType<TValue, TError> failed => Result.Fail<TValue, TNextError>(map(failed.Error)),
            _ => throw new Exception()
        });

    public static async Task<ResultType<TValue, TNextError>> MapErrorAsync<TValue, TError, TNextError>(
        this ResultType<TValue, TError> @this, Func<TError, Task<TNextError>> map) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => Result.Ok<TValue, TNextError>(success.Value),
            FailedResultType<TValue, TError> failed => Result.Fail<TValue, TNextError>(await map(failed.Error)),
            _ => throw new Exception()
        };

    public static async Task<ResultType<TValue, TNextError>> MapErrorAsync<TValue, TError, TNextError>(
        this Task<ResultType<TValue, TError>> @this, Func<TError, TNextError> map) =>
        await @this switch
        {
            SuccessResultType<TValue, TError> success => Result.Ok<TValue, TNextError>(success.Value),
            FailedResultType<TValue, TError> failed => Result.Fail<TValue, TNextError>(map(failed.Error)),
            _ => throw new Exception()
        };

    public static async Task<ResultType<TValue, TNextError>> MapErrorAsync<TValue, TError, TNextError>(
        this Task<ResultType<TValue, TError>> @this, Func<TError, Task<TNextError>> map) =>
        await (await @this).MapErrorAsync(map);

    public static async Task<TResult> MatchAsync<TValue, TError, TResult>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, TResult> ok, Func<TError, TResult> fail) =>
        await @this switch
        {
            SuccessResultType<TValue, TError> success => ok(success.Value),
            FailedResultType<TValue, TError> failed => fail(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<TResult> MatchAsync<TValue, TError, TResult>(
        this ResultType<TValue, TError> @this, Func<TValue, Task<TResult>> ok, Func<TError, TResult> fail) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => await ok(success.Value),
            FailedResultType<TValue, TError> failed => fail(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<TResult> MatchAsync<TValue, TError, TResult>(
        this ResultType<TValue, TError> @this, Func<TValue, TResult> ok, Func<TError, Task<TResult>> fail) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => ok(success.Value),
            FailedResultType<TValue, TError> failed => await fail(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<TResult> MatchAsync<TValue, TError, TResult>(
        this ResultType<TValue, TError> @this, Func<TValue, Task<TResult>> ok, Func<TError, Task<TResult>> fail) =>
        @this switch
        {
            SuccessResultType<TValue, TError> success => await ok(success.Value),
            FailedResultType<TValue, TError> failed => await fail(failed.Error),
            _ => throw new Exception()
        };

    public static async Task<TResult> MatchAsync<TValue, TError, TResult>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, Task<TResult>> ok, Func<TError, TResult> fail) =>
        await (await @this).MatchAsync(ok, fail);

    public static async Task<TResult> MatchAsync<TValue, TError, TResult>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, TResult> ok, Func<TError, Task<TResult>> fail) =>
        await (await @this).MatchAsync(ok, fail);

    public static async Task<TResult> MatchAsync<TValue, TError, TResult>(
        this Task<ResultType<TValue, TError>> @this, Func<TValue, Task<TResult>> ok,
        Func<TError, Task<TResult>> fail) =>
        await (await @this).MatchAsync(ok, fail);
}

public static class OptionToResult
{
    public static ResultType<TValue, TError> ToResult<TValue, TError>(this OptionType<TValue> @this, TError error) =>
        @this.Match<TValue, ResultType<TValue, TError>>(
            Result.Ok<TValue, TError>,
            () => Result.Fail<TValue, TError>(error)
        );
}
