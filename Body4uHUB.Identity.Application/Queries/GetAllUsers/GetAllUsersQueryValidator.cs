using FluentValidation;

using static Body4uHUB.Shared.Application.Commons.CommonConstants;

namespace Body4uHUB.Identity.Application.Queries.GetAllUsers
{
    public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
    {
        public GetAllUsersQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage(PageGreaterThanZero);

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage(PageSizeGreaterThanZero)
                .LessThanOrEqualTo(100).WithMessage(PageLessThanEqualToZero);
        }
    }
}
