using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Bookmarks.RemoveBookmark;

public class RemoveBookmarkCommandValidator : AbstractValidator<RemoveBookmarkCommand>
{
    public RemoveBookmarkCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(UserIdRequired);

        RuleFor(x => x.ArticleId)
            .GreaterThan(0).WithMessage(ArticleIdRequired);
    }
}
