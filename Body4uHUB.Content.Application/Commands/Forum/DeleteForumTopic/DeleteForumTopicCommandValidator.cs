using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.DeleteForumTopic
{
    public class DeleteForumTopicCommandValidator : AbstractValidator<DeleteForumTopicCommand>
    {
        public DeleteForumTopicCommandValidator()
        {
            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage(ForumTopicIdRequired);
        }
    }
}
