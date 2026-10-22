namespace Body4uHUB.Content.Application.Commands.Articles.Publish
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

    public class PublishArticleCommandValidator : AbstractValidator<PublishArticleCommand>
    {
        public PublishArticleCommandValidator()
        {
            RuleFor(x => x.ArticleId)
                .GreaterThan(0).WithMessage(ArticleIdRequired);
        }
    }
}
