using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Services.Application.Queries.TrainerProfile.GetTrainerProfile
{
    public class GetTrainerProfileByIdQueryValidator : AbstractValidator<GetTrainerProfileByIdQuery>
    {
        public GetTrainerProfileByIdQueryValidator()
        {
            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage(TrainerIdRequired);
        }
    }
}
