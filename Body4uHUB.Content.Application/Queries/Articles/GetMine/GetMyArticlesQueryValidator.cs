using FluentValidation;

using static Body4uHUB.Shared.Application.Commons.CommonConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Queries.Articles.GetMine;

public class GetMyArticlesQueryValidator : AbstractValidator<GetMyArticlesQuery>
{
    public GetMyArticlesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage(PageGreaterThanZero);

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage(PageSizeGreaterThanZero)
            .LessThanOrEqualTo(100).WithMessage(PageLessThanEqualToZero);

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage(AuthorIdRequired);
    }
}
