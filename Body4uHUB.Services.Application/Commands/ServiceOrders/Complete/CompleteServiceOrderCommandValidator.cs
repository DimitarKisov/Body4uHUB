using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Complete
{
    public class CompleteServiceOrderCommandValidator : AbstractValidator<CompleteServiceOrderCommand>
    {
        public CompleteServiceOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ServiceOrderIdCannotBeZeroOrNegative);
        }
    }
}
