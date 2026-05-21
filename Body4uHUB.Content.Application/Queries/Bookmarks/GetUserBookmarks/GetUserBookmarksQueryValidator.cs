using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Shared.Application.Commons.CommonConstants;

namespace Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks
{
    public class GetUserBookmarksQueryValidator : AbstractValidator<GetUserBookmarksQuery>
    {
        public GetUserBookmarksQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(UserIdRequired);

            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage(PageGreaterThanZero);

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage(PageSizeGreaterThanZero)
                .LessThanOrEqualTo(100).WithMessage(PageLessThanEqualToZero);
        }
    }
}
