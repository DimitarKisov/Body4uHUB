namespace Body4uHUB.Content.Application.Commands.Forum
{
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Shared;
    using Body4uHUB.Shared.Exceptions;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

    public class CreateForumPostCommand : IRequest<Guid>
    {
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public Guid TopicId { get; set; }

        internal class CreateForumPostCommandHandler : IRequestHandler<CreateForumPostCommand, Guid>
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

            public async Task<Guid> Handle(CreateForumPostCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumTopicRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    throw new NotFoundException(ForumTopicNotFound);
                }

                var post = ForumPost.Create(
                    request.Content,
                    request.AuthorId);

                topic.AddPost(post);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return post.Id;
            }
        }
    }
}
