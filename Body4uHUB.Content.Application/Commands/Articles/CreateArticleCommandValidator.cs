namespace Body4uHUB.Content.Application.Commands.Articles
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

    public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
    {
        public CreateArticleCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(TitleRequired)
                .MinimumLength(TitleMinLength).WithMessage(string.Format(TitleMinLengthMessage, TitleMinLength))
                .MaximumLength(TitleMaxLength).WithMessage(string.Format(TitleMaxLengthMessage, TitleMaxLength));

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .MinimumLength(ContentMinLength).WithMessage(string.Format(ContentMinLengthMessage, ContentMinLength))
                .MaximumLength(ContentMaxLength).WithMessage(string.Format(ContentMaxLengthMessage, ContentMaxLength));

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);
        }
    }
}
