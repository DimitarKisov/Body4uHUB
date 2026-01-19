using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Domain.Models
{
    public class Article : AggregateRoot<ArticleId>
    {
        private readonly List<Comment> _comments = new();

        public string Title { get; private set; }
        public string Content { get; private set; }
        public ArticleStatus Status { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public int ViewCount { get; private set; }
        public Guid AuthorId { get; private set; }
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private Article()
            : base(default!)
        { 
        }

        private Article(string title, string content, Guid authorId)
            : base(default!)
        {
            Title = title;
            Content = content;
            Status = ArticleStatus.Draft;
            ViewCount = 0;
            AuthorId = authorId;
        }

        public static Article Create(string title, string content, Guid authorId)
        {
            Validate(title, content, authorId);

            return new Article(title, content, authorId);
        }

        public void UpdateTitle(string title)
        {
            ValidateTitle(title);
            Title = title;
        }

        public void UpdateContent(string content)
        {
            ValidateContent(content);
            Content = content;
        }

        public void Publish()
        {
            if (Status == ArticleStatus.Published)
            {
                throw new InvalidArticleException(ArticleAlreadyPublished);
            }

            Status = ArticleStatus.Published;
            PublishedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != ArticleStatus.Published)
            {
                throw new InvalidArticleException(ArticleNotPublished);
            }

            Status = ArticleStatus.Archived;
            PublishedAt = null;
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }

        public void AddComment(Comment comment)
        {
            _comments.Add(comment);
        }

        private static void Validate(string title, string content, Guid authorId)
        {
            ValidateTitle(title);
            ValidateContent(content);
            ValidateAuthorId(authorId);
        }

        private static void ValidateTitle(string title)
        {
            Guard.AgainstEmptyString<InvalidArticleException>(title, nameof(Title));
            Guard.ForStringLength<InvalidArticleException>(title, TitleMinLength, TitleMaxLength, nameof(Title));
        }

        private static void ValidateContent(string content)
        {
            Guard.AgainstEmptyString<InvalidArticleException>(content, nameof(Content));
            Guard.ForStringLength<InvalidArticleException>(content, ContentMinLength, ContentMaxLength, nameof(Content));
        }

        private static void ValidateAuthorId(Guid authorId)
        {
            Guard.AgainstEmptyString<InvalidArticleException>(authorId.ToString(), nameof(AuthorId));
        }
    }
}
