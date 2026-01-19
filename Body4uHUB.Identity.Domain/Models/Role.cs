using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

namespace Body4uHUB.Identity.Domain.Models
{
    public class Role : AggregateRoot<Guid>
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
