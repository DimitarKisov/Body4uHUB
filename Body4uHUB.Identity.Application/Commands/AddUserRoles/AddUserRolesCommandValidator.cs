using FluentValidation;

namespace Body4uHUB.Identity.Application.Commands.AddUserRoles
{
    public class AddUserRolesCommandValidator : AbstractValidator<AddUserRolesCommand>
    {
        public AddUserRolesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.RoleIds)
                .NotEmpty().WithMessage("At least one role must be specified.")
                .Must(roles => roles != null && roles.Any()).WithMessage("Role list cannot be empty.");

            RuleForEach(x => x.RoleIds)
                .NotEmpty().WithMessage("Role ids cannot be empty.");
        }
    }
}
