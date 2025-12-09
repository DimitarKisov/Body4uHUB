using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.DeleteForumPost
{
    public class DeleteForumPostCommand : IRequest<Result>
    {
        public Guid PostId { get; set; }
        public Guid TopicId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }

        internal class DeleteForumPostCommandHandler : IRequestHandler<DeleteForumPostCommand, Result>
        {
            private readonly IForumRepository _topicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteForumPostCommandHandler(
                IForumRepository topicRepository,
                IUnitOfWork unitOfWork)
            {
                _topicRepository = topicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(DeleteForumPostCommand request, CancellationToken cancellationToken)
            {
                var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.UnprocessableEntity(ForumTopicNotFound);
                }

                var post = topic.Posts.FirstOrDefault(x => x.Id == request.PostId);
                if (post == null)
                {
                    return Result.UnprocessableEntity(ForumPostNotFound);
                }

                if (!request.IsAdmin && post.AuthorId != request.CurrentUserId)
                {
                    return Result.Forbidden(ForumPostDeleteForbidden);
                }

                post.MarkAsDeleted();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
