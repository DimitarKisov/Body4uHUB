using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;

namespace Body4uHUB.Identity.Domain.Models;

public class Role : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    // For EF Core
    private Role() { }

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
        Guard.ForStringLength<InvalidRoleException>(name, MinRoleNameLength, MaxRoleNameLength, nameof(Name));
    }
}
