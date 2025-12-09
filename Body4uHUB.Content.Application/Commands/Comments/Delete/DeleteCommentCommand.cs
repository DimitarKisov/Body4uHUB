using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

namespace Body4uHUB.Content.Application.Commands.Comments.Delete
{
    public class DeleteCommentCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }

        internal class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result>
        {
            private readonly IArticleRepository _articleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCommentCommandHandler(
                IArticleRepository articleRepository,
                IUnitOfWork unitOfWork)
            {
                _articleRepository = articleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
            {
                var article = await _articleRepository.GetByIdAsync(Domain.ValueObjects.ArticleId.Create(request.ArticleId), cancellationToken);
                if (article == null)
                {
                    return Result.UnprocessableEntity(ArticleNotFound);
                }

                var comment = article.Comments.FirstOrDefault(x => x.Id == CommentId.Create(request.Id));
                if (comment == null)
                {
                    return Result.UnprocessableEntity(CommentNotFound);
                }

                if (!request.IsAdmin && comment.AuthorId != request.CurrentUserId)
                {
                    return Result.Forbidden(CommentDeleteForbidden);
                }

                comment.MarkAsDeleted();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
