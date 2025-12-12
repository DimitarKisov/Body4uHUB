using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Create
{
    public class CreateServiceOrderCommandValidator : AbstractValidator<CreateServiceOrderCommand>
    {
        public CreateServiceOrderCommandValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage(ClientIdRequired);

            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage(TrainerIdRequired);

            RuleFor(x => x.ServiceOfferingId)
                .GreaterThan(0).WithMessage(ServiceOfferingIdRequired);

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage(string.Format(NoteMaxLengthMessage, NotesMaxLength));
        }
    }
}
