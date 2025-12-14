using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderById
{
    public class GetServiceOrderByIdQueryValidator : AbstractValidator<GetServiceOrderByIdQuery>
    {
        public GetServiceOrderByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ServiceOrderIdCannotBeZeroOrNegative);
        }
    }
}
