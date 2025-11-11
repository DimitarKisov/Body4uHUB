namespace Body4uHUB.Content.Application.Commands.Comments.Delete
{
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Shared;
    using Body4uHUB.Shared.Exceptions;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

    public class DeleteCommentCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }

        internal class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
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

            public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
            {
                var article = await _articleRepository.GetByIdAsync(Domain.ValueObjects.ArticleId.Create(request.ArticleId), cancellationToken);
                if (article == null)
                {
                    throw new NotFoundException(ArticleNotFound);
                }

                var comment = article.Comments.FirstOrDefault(x => x.Id == CommentId.Create(request.Id));
                if (comment == null)
                {
                    throw new NotFoundException(CommentNotFound);
                }

                comment.MarkAsDeleted();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
