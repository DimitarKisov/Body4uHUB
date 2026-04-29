using System;

namespace Body4uHUB.Content.Api.Models.Articles
{
    public record CreateCommentRequest(string Content, Guid? ParentCommentId);
}
