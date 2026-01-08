using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumPostConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumPost
{
    public class EditForumPostCommand : IRequest<Result>
    {
        public Guid PostId { get; set; }
        public Guid TopicId { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public AuthorizationContext AuthContext { get; set; }

        internal class EditForumPostCommandHandler : IRequestHandler<EditForumPostCommand, Result>
        {
            private readonly IForumRepository _forumRepository;
            private readonly IUnitOfWork _unitOfWork;

            public EditForumPostCommandHandler(
                IForumRepository forumRepository,
                IUnitOfWork unitOfWork)
            {
                _forumRepository = forumRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(EditForumPostCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumRepository.GetByIdWithPostsAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.ResourceNotFound(ForumTopicNotFound);
                }

                var post = topic.Posts.FirstOrDefault(x => x.Id == request.PostId);
                if (post == null)
                {
                    return Result.ResourceNotFound(ForumPostNotFound);
                }

                if (!request.AuthContext.IsAdmin && post.AuthorId != request.AuthContext.CurrentUserId)
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
