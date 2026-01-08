using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumTopic
{
    public class EditForumTopicCommand : IRequest<Result>
    {
        public Guid TopicId { get; set; }
        public string Title { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }

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

                if (!request.IsAdmin && topic.AuthorId != request.CurrentUserId)
                {
                    return Result.Forbidden(ForumTopicEditForbidden);
                }
                
                topic.UpdateTitle(request.Title);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
