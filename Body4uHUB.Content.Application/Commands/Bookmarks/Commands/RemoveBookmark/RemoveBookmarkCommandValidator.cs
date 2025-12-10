namespace Body4uHUB.Content.Application.Commands.Bookmarks.Commands.RemoveBookmark
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

    public class RemoveBookmarkCommandValidator : AbstractValidator<RemoveBookmarkCommand>
    {
        public RemoveBookmarkCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(UserIdRequired);

            RuleFor(x => x.ArticleId)
                .NotEmpty().WithMessage(ArticleIdRequired);
        }
    }
}
