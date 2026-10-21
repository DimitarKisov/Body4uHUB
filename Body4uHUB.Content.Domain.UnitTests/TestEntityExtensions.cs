using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Content.Domain.UnitTests
{
    internal static class TestEntityExtensions
    {
        // Ids of ForumPost (identity column) are assigned by the database, so tests set them explicitly.
        public static T WithId<T, TId>(this T entity, TId id)
            where T : Entity<TId>
            where TId : notnull
        {
            typeof(Entity<TId>).GetProperty(nameof(Entity<TId>.Id))!.SetValue(entity, id);
            return entity;
        }
    }
}
