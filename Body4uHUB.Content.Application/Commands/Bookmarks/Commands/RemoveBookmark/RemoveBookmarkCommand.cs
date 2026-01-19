using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.BookmarkConstants;

namespace Body4uHUB.Content.Application.Commands.Bookmarks.Commands.RemoveBookmark
{
    public class RemoveBookmarkCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }

        internal class RemoveBookmarkCommandHandler : IRequestHandler<RemoveBookmarkCommand, Result>
        {
            private readonly IBookmarkRepository _bookmarkRepository;
            private readonly IUnitOfWork _unitOfWork;

            public RemoveBookmarkCommandHandler(
                IBookmarkRepository bookmarkRepository,
                IUnitOfWork unitOfWork)
            {
                _bookmarkRepository = bookmarkRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
            {
                var bookmark = await _bookmarkRepository.GetByUserAndArticleAsync(
                    request.UserId,
                    request.ArticleId,
                    cancellationToken);

                if (bookmark == null)
                {
                    return Result.ResourceNotFound(BookmarkNotFound);
                }

                _bookmarkRepository.Remove(bookmark);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}