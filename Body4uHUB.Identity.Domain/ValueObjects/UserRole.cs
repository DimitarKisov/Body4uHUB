using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Identity.Domain.ValueObjects;

// Role is its own aggregate, so User references it by Id instead of holding the Role object.
public class UserRole : ValueObject
{
    public Guid RoleId { get; private set; }

    internal UserRole(Guid roleId)
    {
        RoleId = roleId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RoleId;
    }
}
