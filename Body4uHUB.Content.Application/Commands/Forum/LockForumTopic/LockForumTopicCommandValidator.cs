using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.LockForumTopic
{
    public class LockForumTopicCommandValidator : AbstractValidator<LockForumTopicCommand>
    {
        public LockForumTopicCommandValidator()
        {
            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage(ForumTopicIdRequired);
        }
    }
}
