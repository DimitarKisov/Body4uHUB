using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderByClients
{
    public class GetOrdersByClientQueryValidator : AbstractValidator<GetOrdersByClientQuery>
    {
        public GetOrdersByClientQueryValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage(ClientIdRequired);
        }
    }
}
