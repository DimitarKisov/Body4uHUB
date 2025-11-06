namespace Body4uHUB.Content.Application.Commands.Articles.Edit
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

    public class EditArticleCommandValidator : AbstractValidator<EditArticleCommand>
    {
        public EditArticleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ArticleIdRequired);

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(TitleRequired)
                .Length(TitleMinLength, TitleMaxLength).WithMessage(string.Format(TitleLengthMessage, TitleMinLength, TitleMaxLength));

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ContentMinLength, ContentMaxLength).WithMessage(string.Format(ContentLengthMessage, ContentMinLength, ContentMaxLength));
        }
    }
}
