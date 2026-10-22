using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.EditComment;

public class EditCommentCommandValidator : AbstractValidator<EditCommentCommand>
{
    public EditCommentCommandValidator()
    {
        RuleFor(x => x.ArticleId)
            .GreaterThan(0).WithMessage(ArticleIdRequired);

        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage(CommentIdRequired);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ContentRequired)
            .Length(CommentContentMinLength, CommentContentMaxLength).WithMessage(string.Format(ContentLengthMessage, CommentContentMinLength, CommentContentMaxLength));
    }
}
