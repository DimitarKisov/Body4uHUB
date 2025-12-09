using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Archive
{
    public class ArchiveArticleCommandValidator : AbstractValidator<ArchiveArticleCommand>
    {
        public ArchiveArticleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ArticleIdRequired);
        }
    }
}
