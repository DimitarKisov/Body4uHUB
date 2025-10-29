namespace Body4uHUB.Content.Domain.Constants
{
    public class ModelConstants
    {
        public class Article
        {
            public const int ContentMinLength = 50;
            public const int ContentMaxLength = 5000;
            public const int TitleMinLength = 10;
            public const int TitleMaxLength = 100;

            public const string ArticleAlreadyPublished = "Article is already published.";
            public const string ArticleNotPublished = "Article is not published.";
        }
    }
}
