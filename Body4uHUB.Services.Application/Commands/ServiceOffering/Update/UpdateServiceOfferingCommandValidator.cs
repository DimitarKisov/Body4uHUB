using Body4uHUB.Services.Application.Validators;
using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Shared;
using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Update
{
    public class UpdateServiceOfferingCommandValidator : AbstractValidator<UpdateServiceOfferingCommand>
    {
        public UpdateServiceOfferingCommandValidator()
        {
            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage(TrainerIdRequired);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(NameRequired)
                .Length(NameMinLength, NameMaxLength).WithMessage(string.Format(NameLength, NameMinLength, NameMaxLength));

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(DescriptionRequired)
                .Length(DescriptionMinLength, DescriptionMaxLength).WithMessage(string.Format(DescriptionLength, DescriptionMinLength, DescriptionMaxLength));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(MinPrice);

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage(CurrencyRequired);

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(MinDurationMinutes).WithMessage(string.Format(MinDuration, MinDurationMinutes));

            RuleFor(x => x.ServiceType)
                .NotEmpty().WithMessage(ServiceTypeRequired)
                .Must(ValidationExtensions.BeValidServiceCategory).WithMessage(string.Format(ServiceTypeInvalid, string.Join(", ", Enumeration.GetAll<ServiceCategory>().Select(x => x.Name).ToHashSet())));
        }
    }
}
