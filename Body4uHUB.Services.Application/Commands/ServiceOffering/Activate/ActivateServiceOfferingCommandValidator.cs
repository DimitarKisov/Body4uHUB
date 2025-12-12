using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Activate
{
    public class ActivateServiceOfferingCommandValidator : AbstractValidator<ActivateServiceOfferingCommand>
    {
        public ActivateServiceOfferingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ServiceOfferingIdRequired);

            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage(TrainerIdRequired);
        }
    }
}
