using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Edit
{
    public class EditArticleCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public AuthorizationContext AuthContext { get; set; }

        internal class EditArticleCommandHandler : IRequestHandler<EditArticleCommand, Result>
        {
            private readonly IArticleRepository _articleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public EditArticleCommandHandler(
                IArticleRepository articleRepository,
                IUnitOfWork unitOfWork)
            {
                _articleRepository = articleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(EditArticleCommand request, CancellationToken cancellationToken)
            {
                var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);
                if (article == null)
                {
                    return Result.ResourceNotFound(ArticleNotFound);
                }

                if (!request.AuthContext.IsAdmin && article.AuthorId != request.AuthContext.CurrentUserId)
                {
                    return Result.Forbidden(ArticleEditForbidden);
                }

                article.UpdateTitle(request.Title);
                article.UpdateContent(request.Content);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
