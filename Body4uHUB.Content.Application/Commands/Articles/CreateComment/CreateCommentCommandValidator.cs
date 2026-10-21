namespace Body4uHUB.Content.Application.Commands.Articles.CreateComment
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(CommentContentMinLength, CommentContentMaxLength).WithMessage(string.Format(ContentLengthMessage, CommentContentMinLength, CommentContentMaxLength));

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);

            RuleFor(x => x.ArticleId)
                .GreaterThan(0).WithMessage(ArticleIdRequired);
        }
    }
}
