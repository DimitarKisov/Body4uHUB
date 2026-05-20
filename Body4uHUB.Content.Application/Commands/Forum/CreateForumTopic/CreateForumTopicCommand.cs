using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.CreateForumTopic
{
    public record CreateForumTopicCommand(string Title, Guid AuthorId) : IRequest<Result<CreateForumTopicResponse>>;

    internal sealed class CreateForumTopicCommandHandler : IRequestHandler<CreateForumTopicCommand, Result<CreateForumTopicResponse>>
    {
        private readonly IForumRepository _forumRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateForumTopicCommandHandler(
            IForumRepository forumRepository,
            IUnitOfWork unitOfWork)
        {
            _forumRepository = forumRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateForumTopicResponse>> Handle(CreateForumTopicCommand request, CancellationToken cancellationToken)
        {
            var titleExists = await _forumRepository.ExistsByTitleAsync(request.Title, cancellationToken);
            if (titleExists)
            {
                return Result.Conflict<CreateForumTopicResponse>(string.Format(ForumTopicExists, request.Title));
            }

            var topic = ForumTopic.Create(
                request.Title,
                request.AuthorId);

            _forumRepository.Add(topic);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateForumTopicResponse(topic.Id));
        }
    }
}
