namespace Body4uHUB.Content.Application.Commands.Articles.PublishArticle
{
    using Body4uHUB.Content.Application.Commands.Articles.Publish;
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

    public class PublishArticleValidator : AbstractValidator<PublishArticleCommand>
    {
        public PublishArticleValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ArticleIdRequired);
        }
    }
}
