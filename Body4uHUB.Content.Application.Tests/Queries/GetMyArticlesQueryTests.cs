using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles.GetMine;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Shared.Application;
using Moq;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Tests.Queries;

[TestFixture]
public class GetMyArticlesQueryTests
{
    private Mock<IArticleReadRepository> _articleReadRepository;
    private GetMyArticlesQueryHandler _handler;
    private GetMyArticlesQueryValidator _validator;
    private Guid _authorId;

    [SetUp]
    public void Setup()
    {
        _articleReadRepository = new Mock<IArticleReadRepository>();
        _handler = new GetMyArticlesQueryHandler(_articleReadRepository.Object);
        _validator = new GetMyArticlesQueryValidator();
        _authorId = Guid.NewGuid();
    }

    [Test]
    public async Task Handle_ShouldReturnTheAuthorsArticlesFromTheRepository()
    {
        var draft = new ArticleSummaryDto(1, "Title", ArticleCategory.Training.Name, 1, _authorId, ArticleStatus.Draft.Name, null, 0, DateTime.UtcNow, null);
        var page = new PagedResult<ArticleSummaryDto>([draft], 1, 2, 5);

        _articleReadRepository
            .Setup(x => x.GetMyArticlesAsync(_authorId, 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        var result = await _handler.Handle(new GetMyArticlesQuery(_authorId, 2, 5), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.SameAs(page));
    }

    [Test]
    public void Validate_ShouldRejectEmptyAuthorId()
    {
        var result = _validator.Validate(new GetMyArticlesQuery(Guid.Empty, 1, 10));

        Assert.That(result.Errors.Select(e => e.ErrorMessage), Does.Contain(AuthorIdRequired));
    }

    [TestCase(0, 10)]
    [TestCase(1, 0)]
    [TestCase(1, 101)]
    public void Validate_ShouldRejectInvalidPaging(int page, int pageSize)
    {
        var result = _validator.Validate(new GetMyArticlesQuery(_authorId, page, pageSize));

        Assert.That(result.IsValid, Is.False);
    }
}
