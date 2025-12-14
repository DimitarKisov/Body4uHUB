using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Services.Application.Queries.GetServiceOfferingsByTrainer
{
    public class GetServiceOfferingsByTrainerQueryValidator : AbstractValidator<GetServiceOfferingsByTrainerQuery>
    {
        public GetServiceOfferingsByTrainerQueryValidator()
        {
            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage(TrainerIdRequired);
        }
    }
}
