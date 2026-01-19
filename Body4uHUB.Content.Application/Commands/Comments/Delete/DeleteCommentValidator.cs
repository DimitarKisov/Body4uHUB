using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Comments.Delete
{
    public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(CommentIdRequired);

            RuleFor(x => x.ArticleId)
                .GreaterThan(0).WithMessage(ArticleIdRequired);
        }
    }
}