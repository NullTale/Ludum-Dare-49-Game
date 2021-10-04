namespace CoreLib
{
    public class ResponsibilityChain<THandler, TContext> : GlobalList<THandler>, IContextHandler<TContext>
        where THandler : IContextHandler<TContext>
    {
        public void Handle(TContext context)
        {
            foreach (var handler in Content)
                handler.Handle(context);
        }
    }
}