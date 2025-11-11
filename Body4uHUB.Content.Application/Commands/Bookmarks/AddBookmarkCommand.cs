namespace Body4uHUB.Content.Application.Commands.Bookmarks
{
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Shared;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.BookmarkConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

    public class AddBookmarkCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public ArticleId ArticleId { get; set; }

        internal class AddBookmarkCommandHandler : IRequestHandler<AddBookmarkCommand, Guid>
        {
            private readonly IBookmarkRepository _bookmarkRepository;
            private readonly IArticleRepository _articleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public AddBookmarkCommandHandler(
                IBookmarkRepository bookmarkRepository,
                IArticleRepository articleRepository,
                IUnitOfWork unitOfWork)
            {
                _bookmarkRepository = bookmarkRepository;
                _articleRepository = articleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Guid> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
            {
                var articleExists = await _articleRepository.ExistsByIdAsync(request.ArticleId, cancellationToken);
                if (!articleExists)
                {
                    throw new InvalidOperationException(ArticleNotFound);
                }

                var bookmarkExists = await _bookmarkRepository.ExistsAsync(request.UserId, request.ArticleId, cancellationToken);
                if (bookmarkExists)
                {
                    throw new InvalidOperationException(BookmarkAlreadyExists);
                }

                var bookmark = Bookmark.Create(request.UserId, request.ArticleId);
                _bookmarkRepository.Add(bookmark);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return bookmark.Id;
            }
        }
    }
}