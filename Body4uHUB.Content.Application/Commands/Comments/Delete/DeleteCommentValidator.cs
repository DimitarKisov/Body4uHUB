namespace Body4uHUB.Content.Application.Commands.Comments.Delete
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

    public class DeleteCommentValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(CommentIdRequired);

            RuleFor(x => x.ArticleId)
                .NotEmpty().WithMessage(ArticleIdRequired);
        }
    }
}
