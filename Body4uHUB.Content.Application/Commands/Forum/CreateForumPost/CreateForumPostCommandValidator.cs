namespace Body4uHUB.Content.Application.Commands.Forum.CreateForumPost
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

    public class CreateForumPostCommandValidator : AbstractValidator<CreateForumPostCommand>
    {
        public CreateForumPostCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ForumPostContentMinLength, ForumPostContentMaxLength).WithMessage(string.Format(ContentLengthMessage, ForumPostContentMinLength, ForumPostContentMaxLength));

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);

            RuleFor(x => x.TopicId)
                .GreaterThan(0).WithMessage(ForumTopicIdRequired);
        }
    }
}
