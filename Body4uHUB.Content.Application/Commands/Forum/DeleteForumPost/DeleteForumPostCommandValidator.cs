using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.DeleteForumPost
{
    public class DeleteForumPostCommandValidator : AbstractValidator<DeleteForumPostCommand>
    {
        public DeleteForumPostCommandValidator()
        {
            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage(ForumPostIdRequired);

            RuleFor(x => x.TopicId)
                .GreaterThan(0).WithMessage(ForumTopicIdRequired);
        }
    }
}
