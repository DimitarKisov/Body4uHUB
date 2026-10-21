using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Exceptions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumTopic
{
    public record EditForumTopicCommand(int TopicId, string Title, AuthorizationContext AuthContext) : IRequest<Result>;

    internal sealed class EditForumTopicCommandHandler(
        IForumRepository forumRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<EditForumTopicCommand, Result>
    {
        private readonly IForumRepository _forumRepository = forumRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(EditForumTopicCommand request, CancellationToken cancellationToken)
        {
            var topic = await _forumRepository.GetByIdAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                return Result.ResourceNotFound(ForumTopicNotFound);
            }

            var titleExists = await _forumRepository.ExistsByTitleAsync(request.Title, request.TopicId, cancellationToken);
            if (titleExists)
            {
                return Result.Conflict(string.Format(ForumTopicExists, request.Title));
            }

            topic.Edit(request.Title, request.AuthContext.CurrentUserId, request.AuthContext.IsAdmin);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintViolationException)
            {
                // A concurrent request with the same title passed the ExistsByTitleAsync check first.
                return Result.Conflict(string.Format(ForumTopicExists, request.Title));
            }

            return Result.Success();
        }
    }
}
