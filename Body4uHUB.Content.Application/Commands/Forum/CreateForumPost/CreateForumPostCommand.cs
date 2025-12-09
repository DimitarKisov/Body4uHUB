using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.CreateForumPost
{
    public class CreateForumPostCommand : IRequest<Result<Guid>>
    {
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public Guid TopicId { get; set; }

        internal class CreateForumPostCommandHandler : IRequestHandler<CreateForumPostCommand, Result<Guid>>
        {
            private readonly IForumRepository _forumRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateForumPostCommandHandler(
                IForumRepository forumRepository,
                IUnitOfWork unitOfWork)
            {
                _forumRepository = forumRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<Guid>> Handle(CreateForumPostCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumRepository.GetByIdWithPostsAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.UnprocessableEntity<Guid>(ForumTopicNotFound);
                }

                var post = ForumPost.Create(
                    request.Content,
                    request.AuthorId);

                topic.AddPost(post);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(post.Id);
            }
        }
    }
}
