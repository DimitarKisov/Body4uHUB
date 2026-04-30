using FluentValidation;

using static Body4uHUB.Shared.Application.Commons.CommonConstants;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAll
{
    public class GetAllArticlesQueryValidator : AbstractValidator<GetAllArticlesQuery>
    {
        public GetAllArticlesQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage(PageGreaterThanZero);

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage(PageSizeGreaterThanZero)
                .LessThanOrEqualTo(100).WithMessage(PageLessThanEqualToZero);
        }
    }
}
