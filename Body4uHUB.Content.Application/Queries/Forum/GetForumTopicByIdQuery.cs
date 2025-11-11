namespace Body4uHUB.Content.Application.Queries.Forum
{
    using Body4uHUB.Content.Application.DTOs;
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Shared.Exceptions;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

    public class GetForumTopicByIdQuery : IRequest<ForumTopicDto>
    {
        public Guid TopicId { get; set; }

        internal class GetForumTopicByIdQueryHandler : IRequestHandler<GetForumTopicByIdQuery, ForumTopicDto>
        {
            private readonly IForumTopicRepository _topicRepository;

            public GetForumTopicByIdQueryHandler(
                IForumTopicRepository topicRepository)
            {
                _topicRepository = topicRepository;
            }

            public async Task<ForumTopicDto> Handle(GetForumTopicByIdQuery request, CancellationToken cancellationToken)
            {
                var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    throw new NotFoundException(ForumTopicNotFound);
                }

                topic.IncrementViewCount();

                return new ForumTopicDto
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
            }
        }
    }
}