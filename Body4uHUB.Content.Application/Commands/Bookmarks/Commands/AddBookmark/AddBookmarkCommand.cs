using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.BookmarkConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;



namespace Body4uHUB.Content.Application.Commands.Bookmarks.Commands.AddBookmark
{
    public class AddBookmarkCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }

        internal class AddBookmarkCommandHandler : IRequestHandler<AddBookmarkCommand, Result<Guid>>
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

            public async Task<Result<Guid>> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
            {
                var articleId = Domain.ValueObjects.ArticleId.Create(request.ArticleId);

                var articleExists = await _articleRepository.ExistsByIdAsync(articleId, cancellationToken);
                if (!articleExists)
                {
                    return Result.UnprocessableEntity<Guid>(ArticleNotFound);
                }

                var bookmarkExists = await _bookmarkRepository.ExistsAsync(request.UserId, articleId, cancellationToken);
                if (bookmarkExists)
                {
                    return Result.UnprocessableEntity<Guid>(BookmarkAlreadyExists);
                }

                var bookmark = Bookmark.Create(request.UserId, articleId);

                _bookmarkRepository.Add(bookmark);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(bookmark.Id);
            }
        }
    }
}