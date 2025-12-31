using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.CreateTrainer
{
    public class CreateTrainerAccountCommandValidator : AbstractValidator<CreateTrainerAccountCommand>
    {
        public CreateTrainerAccountCommandValidator()
        {
            RuleFor(x=>x.UserId)
                .NotEmpty().WithMessage(UserIdRequired);
        }
    }
}
