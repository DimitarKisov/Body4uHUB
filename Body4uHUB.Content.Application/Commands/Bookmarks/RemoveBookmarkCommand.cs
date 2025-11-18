using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Exceptions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.BookmarkConstants;

namespace Body4uHUB.Content.Application.Commands.Bookmarks
{
    public class RemoveBookmarkCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }

        internal class RemoveBookmarkCommandHandler : IRequestHandler<RemoveBookmarkCommand, Unit>
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

            public async Task<Unit> Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
            {
                var articleId = Domain.ValueObjects.ArticleId.Create(request.ArticleId);

                var bookmark = await _bookmarkRepository.GetByUserAndArticleAsync(
                    request.UserId,
                    articleId,
                    cancellationToken);

                if (bookmark == null)
                {
                    throw new NotFoundException(BookmarkNotFound);
                }

                _bookmarkRepository.Remove(bookmark);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}