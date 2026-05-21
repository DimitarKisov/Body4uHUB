using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.BookmarkConstants;

namespace Body4uHUB.Content.Application.Commands.Bookmarks.AddBookmark
{
    public record AddBookmarkCommand(Guid UserId, int ArticleId) : IRequest<Result<AddBookmarkResponse>>;

    internal sealed class AddBookmarkCommandHandler : IRequestHandler<AddBookmarkCommand, Result<AddBookmarkResponse>>
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

        public async Task<Result<AddBookmarkResponse>> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
        {
            var articleId = await _articleRepository.GetArticleIdByNumberAsync(request.ArticleId, cancellationToken);
            if (articleId <= 0)
            {
                return Result.ResourceNotFound<AddBookmarkResponse>(ArticleNotFound);
            }

            var bookmarkExists = await _bookmarkRepository.ExistsAsync(request.UserId, articleId, cancellationToken);
            if (bookmarkExists)
            {
                return Result.Conflict<AddBookmarkResponse>(BookmarkAlreadyExists);
            }

            var bookmark = Bookmark.Create(request.UserId, articleId);

            _bookmarkRepository.Add(bookmark);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AddBookmarkResponse(bookmark.Id));
        }
    }
}
