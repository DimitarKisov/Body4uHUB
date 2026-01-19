namespace Body4uHUB.Shared.Domain.Abstractions
{
    public interface IRepository<TEntity>
        where TEntity : IAggregateRoot
    {
    }
}
