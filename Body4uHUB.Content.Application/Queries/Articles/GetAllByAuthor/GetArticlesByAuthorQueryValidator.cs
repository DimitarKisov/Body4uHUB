using FluentValidation;

using static Body4uHUB.Shared.Application.Commons.CommonConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor
{
    public class GetArticlesByAuthorQueryValidator : AbstractValidator<GetArticlesByAuthorQuery>
    {
        public GetArticlesByAuthorQueryValidator()
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
}
