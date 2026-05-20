using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumTopic
{
    public record EditForumTopicCommand(int TopicId, string Title, AuthorizationContext AuthContext) : IRequest<Result>;

    internal sealed class EditForumTopicCommandHandler : IRequestHandler<EditForumTopicCommand, Result>
    {
        private readonly IForumRepository _forumRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditForumTopicCommandHandler(
            IForumRepository forumRepository,
            IUnitOfWork unitOfWork)
        {
            _forumRepository = forumRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(EditForumTopicCommand request, CancellationToken cancellationToken)
        {
            var topic = await _forumRepository.GetByIdAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                return Result.ResourceNotFound(ForumTopicNotFound);
            }

            if (!string.Equals(topic.Title, request.Title, StringComparison.Ordinal))
            {
                var titleExists = await _forumRepository.ExistsByTitleAsync(request.Title, cancellationToken);
                if (titleExists)
                {
                    return Result.Conflict(string.Format(ForumTopicExists, request.Title));
                }
            }

            topic.Edit(request.Title, request.AuthContext.CurrentUserId, request.AuthContext.IsAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
