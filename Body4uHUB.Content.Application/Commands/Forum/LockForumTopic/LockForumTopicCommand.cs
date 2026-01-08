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
            private readonly IForumRepository _forumRepository;
            private readonly IUnitOfWork _unitOfWork;

            public LockForumTopicCommandHandler(
                IForumRepository forumRepository,
                IUnitOfWork unitOfWork)
            {
                _forumRepository = forumRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(LockForumTopicCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.ResourceNotFound(ForumTopicNotFound);
                }

                topic.Lock();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
