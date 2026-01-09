namespace Music.CommandHandlers
{
    public interface IBaseCommandHandler<in TCommand, out TResult>
    {
        public TResult Execute(TCommand command);
    }

    public interface IBaseCommandHandler<in TCommand>
    {
        public void Execute(TCommand command);
    }

    public interface IBaseCommandHandler
    {
        public void Execute();
    }
}
