using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Comments.Create
{
    public class CreateCommentCommand : IRequest<Result<Guid>>
    {
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public int ArticleId { get; set; }
        public Guid? ParentCommentId { get; set; }

        internal class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
        {
            private readonly IArticleRepository _articleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateCommentCommandHandler(
                IArticleRepository articleRepository,
                IUnitOfWork unitOfWork)
            {
                _articleRepository = articleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
            {
                var article = await _articleRepository.GetByNumberAsync(request.ArticleId, cancellationToken);
                if (article == null)
                {
                    return Result.ResourceNotFound<Guid>(ArticleNotFound);
                }

                var comment = Comment.Create(
                    request.Content,
                    request.AuthorId,
                    request.ParentCommentId.HasValue ? request.ParentCommentId.Value : null);

                article.AddComment(comment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(comment.Id);
            }
        }
    }
}
