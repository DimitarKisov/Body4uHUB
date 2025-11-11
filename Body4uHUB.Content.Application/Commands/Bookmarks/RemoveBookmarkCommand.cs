namespace Body4uHUB.Content.Application.Commands.Bookmarks
{
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Shared;
    using Body4uHUB.Shared.Exceptions;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.BookmarkConstants;

    public class RemoveBookmarkCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }

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
                var bookmark = await _bookmarkRepository.GetByUserAndArticleAsync(
                    request.UserId,
                    request.ArticleId,
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