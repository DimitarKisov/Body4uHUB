using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumTopic
{
    public record EditForumTopicCommand(string Title) : IRequest<Result>
    {
        [JsonIgnore]
        public Guid TopicId{ get; init; }

        [JsonIgnore]
        public AuthorizationContext AuthContext { get; init; }

        internal class EditForumTopicCommandHandler : IRequestHandler<EditForumTopicCommand, Result>
        {
            private readonly IForumRepository _forumRepository;
            private readonly IUnitOfWork _unitOfWork;

            public EditForumTopicCommandHandler(
                IForumRepository forumRepository,
                IUnitOfWork unitOfWork)
            {
                _forumRepository = forumRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(EditForumTopicCommand request, CancellationToken cancellationToken)
            {
                var titleExists = await _forumRepository.ExistsByTitleAsync(request.Title, cancellationToken);
                if (titleExists)
                {
                    return Result.Conflict(string.Format(ForumTopicExists, request.Title));
                }

                var topic = await _forumRepository.GetByIdWithPostsAsync(request.TopicId, cancellationToken);
                if (topic == null)
                {
                    return Result.ResourceNotFound(ForumTopicNotFound);
                }

                topic.Edit(request.Title, request.AuthContext.CurrentUserId, request.AuthContext.IsAdmin);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
