namespace Body4uHUB.Content.Application.Commands.Articles.Edit
{
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Shared;
    using Body4uHUB.Shared.Exceptions;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

    public class EditArticleCommand : IRequest<Unit>
    {
        public ArticleId Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        internal class EditArticleCommandHandler : IRequestHandler<EditArticleCommand, Unit>
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

            public async Task<Unit> Handle(EditArticleCommand request, CancellationToken cancellationToken)
            {
                var article = await _articleRepository.GetByIdAsync(request.Id, cancellationToken);
                if (article == null)
                {
                    throw new NotFoundException(ArticleNotFound);
                }

                article.UpdateTitle(request.Title);
                article.UpdateContent(request.Content);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
