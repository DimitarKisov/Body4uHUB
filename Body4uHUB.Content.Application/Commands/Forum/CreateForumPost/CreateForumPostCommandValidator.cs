namespace Body4uHUB.Content.Application.Commands.Forum.CreateForumPost
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumPostConstants;

    public class CreateForumPostCommandValidator : AbstractValidator<CreateForumPostCommand>
    {
        public CreateForumPostCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ContentMinLength, ContentMaxLength).WithMessage(ContentLengthMessage);

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);

            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage(ForumTopicIdRequired);
        }
    }
}
