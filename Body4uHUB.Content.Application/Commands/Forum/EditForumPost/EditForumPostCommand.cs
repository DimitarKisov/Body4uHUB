using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumPost
{
    public record EditForumPostCommand(string Content) : IRequest<Result>
    {
        [JsonIgnore]
        public Guid PostId { get; init; }

        [JsonIgnore]
        public Guid TopicId { get; init; }

        [JsonIgnore]
        public AuthorizationContext AuthContext { get; init; }

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

                topic.EditPost(
                    request.PostId,
                    request.Content,
                    request.AuthContext.CurrentUserId,
                    request.AuthContext.IsAdmin);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
