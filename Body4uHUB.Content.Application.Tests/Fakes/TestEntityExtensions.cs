using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Content.Application.Tests.Fakes;

internal static class TestEntityExtensions
{
    // Ids of identity columns are assigned by the database, so tests set them explicitly.
    public static T WithId<T, TId>(this T entity, TId id)
        where T : Entity<TId>
        where TId : notnull
    {
        typeof(Entity<TId>).GetProperty(nameof(Entity<TId>.Id))!.SetValue(entity, id);
        return entity;
    }
}
