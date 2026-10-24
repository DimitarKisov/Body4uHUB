using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Shared.Domain.Enumerations;
using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Edit;

public class EditArticleCommandValidator : AbstractValidator<EditArticleCommand>
{
    public EditArticleCommandValidator()
    {
        RuleFor(x => x.ArticleId)
            .GreaterThan(0).WithMessage(ArticleIdRequired);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(TitleRequired)
            .Length(TitleMinLength, TitleMaxLength).WithMessage(string.Format(TitleLengthMessage, TitleMinLength, TitleMaxLength));

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ContentRequired)
            .Length(ContentMinLength, ContentMaxLength).WithMessage(string.Format(ContentLengthMessage, ContentMinLength, ContentMaxLength));

        var categories = Enumeration.GetAll<ArticleCategory>().Select(x => x.Name).ToList();

        RuleFor(x => x.Category)
            .Must(categories.Contains).WithMessage(string.Format(CategoryInvalid, string.Join(", ", categories)));
    }
}
