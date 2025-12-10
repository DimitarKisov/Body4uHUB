using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Commands.Bookmarks.Queries.GetUserBookmarks
{
    public class GetUserBookmarksQuery : IRequest<Result<IEnumerable<BookmarkDto>>>
    {
        public Guid UserId { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;

        internal class GetUserBookmarksQueryHandler
            : IRequestHandler<GetUserBookmarksQuery, Result<IEnumerable<BookmarkDto>>>
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
}
