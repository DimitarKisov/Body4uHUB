using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.DeleteForumPost
{
    public record DeleteForumPostCommand(int TopicId, int PostId, AuthorizationContext AuthContext) : IRequest<Result>;

    internal sealed class DeleteForumPostCommandHandler : IRequestHandler<DeleteForumPostCommand, Result>
    {
        private readonly IForumRepository _forumRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteForumPostCommandHandler(
            IForumRepository forumRepository,
            IUnitOfWork unitOfWork)
        {
            _forumRepository = forumRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteForumPostCommand request, CancellationToken cancellationToken)
        {
            var topic = await _forumRepository.GetByIdWithPostsAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                return Result.ResourceNotFound(ForumTopicNotFound);
            }

            topic.DeletePost(
                request.PostId,
                request.AuthContext.CurrentUserId,
                request.AuthContext.IsAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
