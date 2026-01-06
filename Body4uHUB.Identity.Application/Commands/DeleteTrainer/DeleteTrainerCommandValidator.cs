using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.DeleteTrainer
{
    public class DeleteTrainerCommandValidator : AbstractValidator<DeleteTrainerCommand>
    {
        public DeleteTrainerCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(UserIdRequired);
        }
    }
}
