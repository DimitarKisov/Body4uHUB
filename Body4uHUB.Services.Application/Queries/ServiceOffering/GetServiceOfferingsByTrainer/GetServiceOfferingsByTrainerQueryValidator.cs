using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Services.Application.Queries.ServiceOffering.GetServiceOfferingsByTrainer
{
    public class GetServiceOfferingsByTrainerQueryValidator : AbstractValidator<GetServiceOfferingsByTrainerQuery>
    {
        public GetServiceOfferingsByTrainerQueryValidator()
        {
            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage(TrainerIdRequired);

            RuleFor(x => x.Skip)
                .GreaterThanOrEqualTo(0)
                .WithMessage(SkipInvalid);

            RuleFor(x => x.Take)
                .InclusiveBetween(1, 100)
                .WithMessage(TakeInvalid);
        }
    }
}
