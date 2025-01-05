namespace Music.QueryHandlers
{
    public interface IBaseQueryHandler<in TQuery, out TResult>
    {
        public TResult Execute(TQuery query);
    }

    public interface IBaseQueryHandler<out TResult>
    {
        public TResult Execute();
    }
}
