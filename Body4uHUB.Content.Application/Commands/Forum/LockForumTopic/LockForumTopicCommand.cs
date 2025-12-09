using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.LockForumTopic
{
    public class LockForumTopicCommand : IRequest<Result>
    {
        public Guid TopicId { get; set; }

        internal class LockForumTopicCommandHandler : IRequestHandler<LockForumTopicCommand, Result>
        {
            private readonly IForumTopicRepository _forumTopicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public LockForumTopicCommandHandler(
                IForumTopicRepository forumTopicRepository,
                IUnitOfWork unitOfWork)
            {
                _forumTopicRepository = forumTopicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(LockForumTopicCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumTopicRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.UnprocessableEntity(ForumTopicNotFound);
                }

                topic.Lock();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
