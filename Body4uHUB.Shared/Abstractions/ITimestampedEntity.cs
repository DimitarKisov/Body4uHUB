namespace Body4uHUB.Shared.Domain.Abstractions
{
    /// <summary>
    /// Entity whose creation and modification times are set by the persistence layer on save.
    /// </summary>
    public interface ITimestampedEntity
    {
        DateTime CreatedAt { get; }
        DateTime? ModifiedAt { get; }
        void SetCreatedAt(DateTime utcNow);
        void SetModifiedAt(DateTime utcNow);
    }
}
