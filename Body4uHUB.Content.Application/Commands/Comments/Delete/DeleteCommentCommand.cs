using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

namespace Body4uHUB.Content.Application.Commands.Comments.Delete
{
    public class DeleteCommentCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }

        [JsonIgnore]
        public AuthorizationContext AuthContext { get; set; }

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
                    return Result.ResourceNotFound(ArticleNotFound);
                }

                var comment = article.Comments.FirstOrDefault(x => x.Id == CommentId.Create(request.Id));
                if (comment == null)
                {
                    return Result.ResourceNotFound(CommentNotFound);
                }

                if (!request.AuthContext.IsAdmin && comment.AuthorId != request.AuthContext.CurrentUserId)
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
