using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Forum.GetAllForumTopics
{
    public record GetAllForumTopicsQuery(int Page, int PageSize, bool IncludeDeleted = false) : IRequest<Result<PagedResult<ForumTopicDto>>>;

    internal sealed class GetAllForumTopicsQueryHandler : IRequestHandler<GetAllForumTopicsQuery, Result<PagedResult<ForumTopicDto>>>
    {
        private readonly IForumReadRepository _forumReadRepository;

        public GetAllForumTopicsQueryHandler(IForumReadRepository forumReadRepository)
        {
            _forumReadRepository = forumReadRepository;
        }

        public async Task<Result<PagedResult<ForumTopicDto>>> Handle(GetAllForumTopicsQuery request, CancellationToken cancellationToken)
        {
            var topics = await _forumReadRepository.GetAllAsync(request.Page, request.PageSize, request.IncludeDeleted, cancellationToken);

            return Result.Success(topics);
        }
    }
}
