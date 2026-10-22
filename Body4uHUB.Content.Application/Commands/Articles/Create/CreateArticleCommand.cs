using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

namespace Body4uHUB.Content.Application.Commands.Articles.Create;

public record CreateArticleCommand(string Title, string Content, Guid AuthorId) : IRequest<Result<CreateArticleResponse>>;

internal sealed class CreateArticleCommandHandler(
    IArticleRepository articleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateArticleCommand, Result<CreateArticleResponse>>
{
    private readonly IArticleRepository _articleRepository = articleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CreateArticleResponse>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = Article.Create(
            request.Title,
            request.Content,
            request.AuthorId);

        _articleRepository.Add(article);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateArticleResponse(article.Id));
    }
}
