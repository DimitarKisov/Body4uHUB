using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks
{
    public record GetUserBookmarksQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<PagedResult<GetUserBookmarksResponse>>>;

    internal sealed class GetUserBookmarksQueryHandler : IRequestHandler<GetUserBookmarksQuery, Result<PagedResult<GetUserBookmarksResponse>>>
    {
        private readonly IBookmarkReadRepository _bookmarkReadRepository;

        public GetUserBookmarksQueryHandler(IBookmarkReadRepository bookmarkReadRepository)
        {
            _bookmarkReadRepository = bookmarkReadRepository;
        }

        public async Task<Result<PagedResult<GetUserBookmarksResponse>>> Handle(
            GetUserBookmarksQuery request,
            CancellationToken cancellationToken)
        {
            var bookmarks = await _bookmarkReadRepository.GetByUserIdAsync(request.UserId, request.Page, request.PageSize, cancellationToken);

            return Result.Success(bookmarks);
        }
    }
}
