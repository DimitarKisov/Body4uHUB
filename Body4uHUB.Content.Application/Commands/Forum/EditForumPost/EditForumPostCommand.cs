using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumPostConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumPost
{
    public class EditForumPostCommand : IRequest<Result>
    {
        public Guid PostId { get; set; }
        public Guid TopicId { get; set; }
        public string Content { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }

        internal class EditForumPostCommandHandler : IRequestHandler<EditForumPostCommand, Result>
        {
            private readonly IForumTopicRepository _forumTopicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public EditForumPostCommandHandler(
                IForumTopicRepository forumTopicRepository,
                IUnitOfWork unitOfWork)
            {
                _forumTopicRepository = forumTopicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(EditForumPostCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumTopicRepository.GetByIdAsync(request.TopicId, cancellationToken);
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
                    return Result.Forbidden(ForumPostEditForbidden);
                }

                post.UpdateContent(request.Content);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
