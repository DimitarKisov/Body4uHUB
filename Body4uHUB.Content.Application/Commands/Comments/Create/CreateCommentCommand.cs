namespace Body4uHUB.Content.Application.Commands.Comments.Create
{
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Shared;
    using Body4uHUB.Shared.Exceptions;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

    public class CreateCommentCommand : IRequest<CommentId>
    {
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public ArticleId ArticleId { get; set; }
        public CommentId ParentCommentId { get; set; }

        internal class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentId>
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

            public async Task<CommentId> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
            {
                var article = await _articleRepository.GetByIdAsync(request.ArticleId, cancellationToken);
                if (article == null)
                {
                    throw new NotFoundException(ArticleNotFound);
                }

                var comment = Comment.Create(
                    request.Content,
                    request.AuthorId,
                    request.ArticleId,
                    request.ParentCommentId);

                article.AddComment(comment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return comment.Id;
            }
        }
    }
}
