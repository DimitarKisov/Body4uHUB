using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum
{
    public class CreateForumPostCommand : IRequest<Result<Guid>>
    {
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public Guid TopicId { get; set; }

        internal class CreateForumPostCommandHandler : IRequestHandler<CreateForumPostCommand, Result<Guid>>
        {
            private readonly IForumTopicRepository _forumTopicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateForumPostCommandHandler(
                IForumTopicRepository forumTopicRepository,
                IUnitOfWork unitOfWork)
            {
                _forumTopicRepository = forumTopicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<Guid>> Handle(CreateForumPostCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumTopicRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.UnprocessableEntity<Guid>(ForumTopicNotFound);
                }

                var post = ForumPost.Create(
                    request.Content,
                    request.AuthorId);

                topic.AddPost(post);

                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {

                    throw;
                }

                return Result.Success(post.Id);
            }
        }
    }
}
