namespace Music.Models.Data.Utils;

public abstract record OptionType<T>;
public sealed record OptionSomeType<T>(T Value) : OptionType<T>;
public sealed record OptionNoneType<T> : OptionType<T>;

public static class Option
{
    public static OptionSomeType<T> Some<T>(T value) =>
        value is null
            ? throw new ArgumentNullException(nameof(value), "Cannot wrap null in Some.")
            : new OptionSomeType<T>(value);
    public static OptionNoneType<T> None<T>() => new();

    public static OptionType<T> ToOption<T>(this T? @this) => @this is null ? None<T>() : Some(@this);

    public static T? ToNullable<T>(this OptionType<T> @this) =>
        @this switch
        {
            OptionSomeType<T> some => some.Value,
            OptionNoneType<T> => default,
            _ => throw new InvalidOperationException()
        };

    public static bool IsSome<T>(this OptionType<T> @this, out OptionSomeType<T>? result)
    {
        result = @this as OptionSomeType<T>;
        return result is not null;
    }

    public static OptionType<TResult> Map<T, TResult>(this OptionType<T> @this, Func<T, TResult> map) =>
        @this.IsSome(out var option) ? map(option!.Value).ToOption() : None<TResult>();

    public static OptionType<TResult> Bind<T, TResult>(this OptionType<T> @this, Func<T, OptionType<TResult>> bind) =>
        @this.IsSome(out var option) ? bind(option!.Value) : None<TResult>();

    public static TResult Match<T, TResult>(this OptionType<T> @this, Func<T, TResult> some, Func<TResult> none) =>
        @this.IsSome(out var option) ? some(option!.Value) : none();

    public static OptionType<T> IfSome<T>(this OptionType<T> @this, Action<T> action)
    {
        if (@this.IsSome(out var some)) action(some!.Value);
        return @this;
    }
}

public static class AsyncOption
{
    public static async Task<OptionType<TResult>> MapAsync<T, TResult>(
        this OptionType<T> @this, Func<T, Task<TResult>> map) =>
        @this.IsSome(out var option) ? (await map(option!.Value)).ToOption() : Option.None<TResult>();

    public static async Task<OptionType<TResult>> MapAsync<T, TResult>(
        this Task<OptionType<T>> @this, Func<T, TResult> map) =>
        (await @this).IsSome(out var option) ? map(option!.Value).ToOption() : Option.None<TResult>();

    public static async Task<OptionType<TResult>> MapAsync<T, TResult>(
        this Task<OptionType<T>> @this, Func<T, Task<TResult>> map) =>
        (await @this).IsSome(out var option) ? (await map(option!.Value)).ToOption() : Option.None<TResult>();

    public static async Task<TResult> MatchAsync<T, TResult>(
        this OptionType<T> @this, Func<T, TResult> some, Func<Task<TResult>> none) =>
        @this.IsSome(out var option) ? some(option!.Value) : await none();

    public static async Task<TResult> MatchAsync<T, TResult>(
        this OptionType<T> @this, Func<T, Task<TResult>> some, Func<TResult> none) =>
        @this.IsSome(out var option) ? await some(option!.Value) : none();

    public static async Task<TResult> MatchAsync<T, TResult>(
        this OptionType<T> @this, Func<T, Task<TResult>> some, Func<Task<TResult>> none) =>
        @this.IsSome(out var option) ? await some(option!.Value) : await none();

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<OptionType<T>> @this, Func<T, TResult> some, Func<Task<TResult>> none) =>
        (await @this).IsSome(out var option) ? some(option!.Value) : await none();

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<OptionType<T>> @this, Func<T, Task<TResult>> some, Func<TResult> none) =>
        (await @this).IsSome(out var option) ? await some(option!.Value) : none();

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<OptionType<T>> @this, Func<T, Task<TResult>> some, Func<Task<TResult>> none) =>
        await (await @this).MatchAsync(some, none);

    public static async Task<OptionType<T>> IfSomeAsync<T>(this OptionType<T> @this, Func<T, Task> action)
    {
        if (@this.IsSome(out var some)) await action(some!.Value);
        return @this;
    }

    public static async Task<OptionType<T>> IfSomeAsync<T>(this Task<OptionType<T>> @this, Action<T> action)
    {
        var awaited = await @this;
        if (awaited.IsSome(out var some)) action(some!.Value);
        return awaited;
    }

    public static async Task<OptionType<T>> IfSomeAsync<T>(this Task<OptionType<T>> @this, Func<T, Task> action)
    {
        var awaited = await @this;
        if (awaited.IsSome(out var some)) await action(some!.Value);
        return awaited;
    }
}
