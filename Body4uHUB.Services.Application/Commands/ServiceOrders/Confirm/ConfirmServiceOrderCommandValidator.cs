using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Confirm
{
    public class ConfirmServiceOrderCommandValidator : AbstractValidator<ConfirmServiceOrderCommand>
    {
        public ConfirmServiceOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ServiceOrderIdCannotBeZeroOrNegative);
        }
    }
}
