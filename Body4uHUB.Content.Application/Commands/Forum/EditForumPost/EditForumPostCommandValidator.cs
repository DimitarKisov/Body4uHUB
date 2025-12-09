using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumPostConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumPost
{
    public class EditForumPostCommandValidator : AbstractValidator<EditForumPostCommand>
    {
        public EditForumPostCommandValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage(ForumPostIdRequired);

            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage(ForumTopicIdRequired);

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ContentRequired)
                .Length(ContentMinLength, ContentMaxLength).WithMessage(ContentLengthMessage);
        }
    }
}
