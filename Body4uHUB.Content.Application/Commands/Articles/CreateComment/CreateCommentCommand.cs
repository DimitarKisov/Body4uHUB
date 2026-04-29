using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.CreateComment
{
    public record CreateCommentCommand(string Content, int ArticleId, Guid AuthorId, Guid? ParentCommentId) : IRequest<Result<CreateCommentResponse>>;

    internal sealed class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CreateCommentResponse>>
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

        public async Task<Result<CreateCommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var article = await _articleRepository.GetByNumberAsync(request.ArticleId, cancellationToken);
            if (article is null)
            {
                return Result.ResourceNotFound<CreateCommentResponse>(ArticleNotFound);
            }

            var commentId = article.AddComment(request.Content, request.AuthorId, request.ParentCommentId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateCommentResponse(commentId));
        }
    }
}
