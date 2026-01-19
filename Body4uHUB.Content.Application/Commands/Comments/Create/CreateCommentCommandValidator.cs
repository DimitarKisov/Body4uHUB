namespace Body4uHUB.Content.Application.Commands.Comments.Create
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ContentMinLength, ContentMaxLength).WithMessage(ContentLengthMessage);

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);

            RuleFor(x => x.ArticleId)
                .GreaterThan(0).WithMessage("ArticleId must be greater than 0.");
        }
    }
}
