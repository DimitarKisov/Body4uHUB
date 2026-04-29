using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.CreateForumPost
{
    public record CreateForumPostCommand(string Content) : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid AuthorId { get; init; }

        [JsonIgnore]
        public Guid TopicId { get; init; }

        internal class CreateForumPostCommandHandler : IRequestHandler<CreateForumPostCommand, Result<Guid>>
        {
            private readonly IForumRepository _forumRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateForumPostCommandHandler(
                IForumRepository forumRepository,
                IUnitOfWork unitOfWork)
            {
                _forumRepository = forumRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<Guid>> Handle(CreateForumPostCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumRepository.GetByIdWithPostsAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.ResourceNotFound<Guid>(ForumTopicNotFound);
                }

                var postId = topic.AddPost(request.Content, request.AuthorId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(postId);
            }
        }
    }
}
