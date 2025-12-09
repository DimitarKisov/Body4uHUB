using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.UnlockForumTopic
{
    public class UnlockForumTopicCommand : IRequest<Result>
    {
        public Guid TopicId { get; set; }

        internal class UnlockForumTopicCommandHandler : IRequestHandler<UnlockForumTopicCommand, Result>
        {
            private readonly IForumTopicRepository _topicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UnlockForumTopicCommandHandler(
                IForumTopicRepository topicRepository,
                IUnitOfWork unitOfWork)
            {
                _topicRepository = topicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UnlockForumTopicCommand request, CancellationToken cancellationToken)
            {
                var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.UnprocessableEntity(ForumTopicNotFound);
                }

                topic.Unlock();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
