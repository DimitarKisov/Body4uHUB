namespace Body4uHUB.Content.Application.Commands.Comments.Create
{
    using Body4uHUB.Content.Domain.Models;
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.Common;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

    public class CreateCommentCommandValidator : AbstractValidator<Comment>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ContentMinLength, ContentMaxLength).WithMessage(ContentLengthMessage);

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);

            RuleFor(x => x.ArticleId)
                .NotEmpty().WithMessage(ArticleIdRequired);
        }
    }
}
