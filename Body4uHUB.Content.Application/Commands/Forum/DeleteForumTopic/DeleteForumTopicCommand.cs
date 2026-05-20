using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.DeleteForumTopic
{
    public record DeleteForumTopicCommand(int TopicId, AuthorizationContext AuthContext) : IRequest<Result>;

    internal sealed class DeleteForumTopicCommandHandler : IRequestHandler<DeleteForumTopicCommand, Result>
    {
        private readonly IForumRepository _forumRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteForumTopicCommandHandler(
            IForumRepository forumRepository,
            IUnitOfWork unitOfWork)
        {
            _forumRepository = forumRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteForumTopicCommand request, CancellationToken cancellationToken)
        {
            var topic = await _forumRepository.GetByIdAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                return Result.ResourceNotFound(ForumTopicNotFound);
            }

            topic.Delete(request.AuthContext.CurrentUserId, request.AuthContext.IsAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
