using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.DeleteForumTopic
{
    public class DeleteForumTopicCommand : IRequest<Result>
    {
        public Guid TopicId { get; set; }

        internal class DeleteForumTopicCommandHandler : IRequestHandler<DeleteForumTopicCommand, Result>
        {
            private readonly IForumRepository _forumTopicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteForumTopicCommandHandler(
                IForumRepository forumTopicRepository,
                IUnitOfWork unitOfWork)
            {
                _forumTopicRepository = forumTopicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(DeleteForumTopicCommand request, CancellationToken cancellationToken)
            {
                var topic = await _forumTopicRepository.GetByIdAsync(request.TopicId);
                if (topic == null)
                {
                    return Result.UnprocessableEntity(ForumTopicNotFound);
                }

                topic.MarkAsDeleted();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
