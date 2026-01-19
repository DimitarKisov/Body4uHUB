namespace Body4uHUB.Shared.Domain.Abstractions
{
    /// <summary>
    /// Interface for entities that track modification time
    /// </summary>
    public interface IModifiableEntity
    {
        DateTime? ModifiedAt { get; }
        void SetModifiedAt();
    }
}
