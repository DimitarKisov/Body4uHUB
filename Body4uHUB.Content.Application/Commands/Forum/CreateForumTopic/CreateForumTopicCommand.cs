using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.CreateForumTopic
{
    public class CreateForumTopicCommand : IRequest<Result<Guid>>
    {
        public string Title { get; set; }
        public Guid AuthorId { get; set; }

        internal class CreateForumTopicCommandHandler : IRequestHandler<CreateForumTopicCommand, Result<Guid>>
        {
            private readonly IForumTopicRepository _topicRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateForumTopicCommandHandler(
                IForumTopicRepository topicRepository,
                IUnitOfWork unitOfWork)
            {
                _topicRepository = topicRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<Guid>> Handle(CreateForumTopicCommand request, CancellationToken cancellationToken)
            {
                var titleExists = await _topicRepository.ExistsByTitleAsync(request.Title, cancellationToken);
                if (titleExists)
                {
                    return Result.Conflict<Guid>(string.Format(ForumTopicExists, request.Title));
                }

                var topic = ForumTopic.Create(
                    request.Title,
                    request.AuthorId);

                _topicRepository.Add(topic);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(topic.Id);
            }
        }
    }
}
