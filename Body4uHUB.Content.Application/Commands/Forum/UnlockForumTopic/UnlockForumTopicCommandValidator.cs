using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.UnlockForumTopic
{
    public class UnlockForumTopicCommandValidator : AbstractValidator<UnlockForumTopicCommand>
    {
        public UnlockForumTopicCommandValidator()
        {
            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage(ForumTopicIdRequired);
        }
    }
}
