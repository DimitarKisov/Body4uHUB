using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Delete
{
    public class DeleteArticleCommandValidator : AbstractValidator<DeleteArticleCommand>
    {
        public DeleteArticleCommandValidator()
        {
            RuleFor(x => x.ArticleId)
                .GreaterThan(0).WithMessage(ArticleIdRequired);
        }
    }
}
