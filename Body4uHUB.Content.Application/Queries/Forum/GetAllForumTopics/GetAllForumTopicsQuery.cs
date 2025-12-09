using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Forum.GetAllForumTopics
{
    public class GetAllForumTopicsQuery : IRequest<Result<IEnumerable<ForumTopicDto>>>
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
        public bool IncludeDeleted { get; set; } = false;

        internal class GetAllForumTopicsQueryHandler : IRequestHandler<GetAllForumTopicsQuery, Result<IEnumerable<ForumTopicDto>>>
        {
            private readonly IForumReadRepository _forumReadRepository;

            public GetAllForumTopicsQueryHandler(IForumReadRepository forumReadRepository)
            {
                _forumReadRepository = forumReadRepository;
            }

            public async Task<Result<IEnumerable<ForumTopicDto>>> Handle(GetAllForumTopicsQuery request, CancellationToken cancellationToken)
            {
                var topics = await _forumReadRepository.GetAllAsync(request.Skip, request.Take, cancellationToken);

                return Result.Success(topics);
            }
        }
    }
}
