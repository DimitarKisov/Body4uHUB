using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumPost
{
    public class EditForumPostCommandValidator : AbstractValidator<EditForumPostCommand>
    {
        public EditForumPostCommandValidator()
        {
            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage(ForumPostIdRequired);

            RuleFor(x => x.TopicId)
                .GreaterThan(0).WithMessage(ForumTopicIdRequired);

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ForumPostContentMinLength, ForumPostContentMaxLength).WithMessage(string.Format(ContentLengthMessage, ForumPostContentMinLength, ForumPostContentMaxLength));
        }
    }
}
