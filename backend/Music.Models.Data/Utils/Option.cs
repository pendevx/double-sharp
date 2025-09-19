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

    public static bool IsSome<T>(this OptionType<T> @this, out OptionSomeType<T>? result)
    {
        result = @this as OptionSomeType<T>;
        return result is not null;
    }

    public static OptionType<TResult> Bind<T, TResult>(this OptionType<T> @this, Func<T, OptionType<TResult>> bind) =>
        @this is OptionSomeType<T> option ? bind(option.Value) : None<TResult>();

    public static TResult Match<T, TResult>(this OptionType<T> @this, Func<T, TResult> someAction, Func<TResult> noneAction) =>
        @this is OptionSomeType<T> option ? someAction(option.Value) : noneAction();
}

public static class AsyncOption
{
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
}
