using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Queries.Forum
{
    public class GetForumTopicByIdQuery : IRequest<Result<ForumTopicDto>>
    {
        public Guid TopicId { get; set; }

        internal class GetForumTopicByIdQueryHandler : IRequestHandler<GetForumTopicByIdQuery, Result<ForumTopicDto>>
        {
            private readonly IForumRepository _topicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public GetForumTopicByIdQueryHandler(
                IForumRepository topicRepository,
                IUnitOfWork unitOfWork)
            {
                _topicRepository = topicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<ForumTopicDto>> Handle(GetForumTopicByIdQuery request, CancellationToken cancellationToken)
            {
                var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.UnprocessableEntity<ForumTopicDto>(ForumTopicNotFound);
                }

                topic.IncrementViewCount();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var forumTopicDto = new ForumTopicDto
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    AuthorId = topic.AuthorId,
                    IsLocked = topic.IsLocked,
                    ViewCount = topic.ViewCount,
                    PostCount = topic.Posts.Count,
                    CreatedAt = topic.CreatedAt,
                    ModifiedAt = topic.ModifiedAt
                };

                return Result.Success(forumTopicDto);
            }
        }
    }
}