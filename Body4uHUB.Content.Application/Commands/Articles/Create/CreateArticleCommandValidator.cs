namespace Body4uHUB.Content.Application.Commands.Articles.Create
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.Common;

    public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
    {
        public CreateArticleCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(TitleRequired)
                .Length(TitleMinLength, TitleMaxLength).WithMessage(string.Format(TitleLengthMessage, TitleMinLength, TitleMaxLength));

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ContentMinLength, ContentMaxLength).WithMessage(string.Format(ContentLengthMessage, ContentMinLength, ContentMaxLength));

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);
        }
    }
}
