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

            public const string ArticleIdCannotBeZeroOrNegative = "Article ID must be greater than 0.";

            public const string ArticleAlreadyPublished = "Article is already published.";
            public const string ArticleNotPublished = "Article is not published.";
        }

        public class Bookmark
        {
            public const string BookmarkAlreadyExists = "Bookmark already exists.";
            public const string BookmarkDoesNotExist = "Bookmark does not exist.";
            public const string BookmarkIdCannotBeZeroOrNegative = "Bookmark ID must be greater than 0.";
        }

        public class Comment
        {
            public const int ContentMinLength = 2;
            public const int ContentMaxLength = 1000;

            public const string CommentIdCannotBeZeroOrNegative = "Comment ID must be greater than 0.";

            public const string CommentAlreadyDeleted = "Comment is already deleted.";
        }

        public class ForumPost
        {
            public const int ContentMinLength = 5;
            public const int ContentMaxLength = 2000;

            public const string PostDeleted = "Cannot update content of a deleted forum post.";
        }

        public class ForumTopic
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 100;

            public const string ForumTopicLocked = "Forum topic is already locked.";
            public const string ForumTopicUnlocked = "Forum topic is already unlocked.";

            public const string ForumTopicIdCannotBeZeroOrNegative = "Forum Topic ID must be greater than 0.";
        }
    }
}
