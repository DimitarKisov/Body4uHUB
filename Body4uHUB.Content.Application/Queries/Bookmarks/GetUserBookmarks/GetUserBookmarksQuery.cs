using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks
{
    public record GetUserBookmarksQuery(Guid UserId, int Skip = 0, int Take = 10) : IRequest<Result<IEnumerable<BookmarkDto>>>;

    internal sealed class GetUserBookmarksQueryHandler : IRequestHandler<GetUserBookmarksQuery, Result<IEnumerable<BookmarkDto>>>
    {
        private readonly IBookmarkReadRepository _bookmarkReadRepository;

        public GetUserBookmarksQueryHandler(IBookmarkReadRepository bookmarkReadRepository)
        {
            _bookmarkReadRepository = bookmarkReadRepository;
        }

        public async Task<Result<IEnumerable<BookmarkDto>>> Handle(
            GetUserBookmarksQuery request,
            CancellationToken cancellationToken)
        {
            var bookmarks = await _bookmarkReadRepository.GetByUserIdAsync(request.UserId, request.Skip, request.Take, cancellationToken);

            return Result.Success(bookmarks);
        }
    }
}
