using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Shared;

namespace Body4uHUB.Identity.Domain.Models
{
    public class Role : Entity<Guid>, IAggregateRoot
    {
        public string Name { get; private set; }

        // For EF Core
        private Role() : base(Guid.NewGuid()) { }

        internal Role(Guid id, string name)
            : base(id)
        {
            Name = name;
        }

        public static Role Create(string name)
        {
            Validate(name);

            return new Role(Guid.NewGuid(), name);
        }

        private static void Validate(string name)
        {
            Guard.AgainstEmptyString<InvalidRoleException>(name, nameof(Name));
        }
    }
}
