namespace Body4uHUB.Content.Application.Commands.Bookmarks
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

    public class AddBookmarkCommandValidator : AbstractValidator<AddBookmarkCommand>
    {
        public AddBookmarkCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(UserIdRequired);

            RuleFor(x => x.ArticleId)
                .NotEmpty().WithMessage(ArticleIdRequired);
        }
    }
}
