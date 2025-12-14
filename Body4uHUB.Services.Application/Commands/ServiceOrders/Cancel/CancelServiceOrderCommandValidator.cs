using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Cancel
{
    public class CancelServiceOrderCommandValidator : AbstractValidator<CancelServiceOrderCommand>
    {
        public CancelServiceOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ServiceOrderIdCannotBeZeroOrNegative);
        }
    }
}
