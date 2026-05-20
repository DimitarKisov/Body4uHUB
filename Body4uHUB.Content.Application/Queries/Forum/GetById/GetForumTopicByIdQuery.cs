using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Queries.Forum.GetById
{
    public record GetForumTopicByIdQuery(int TopicId) : IRequest<Result<GetForumTopicByIdResponse>>;

    internal sealed class GetForumTopicByIdQueryHandler : IRequestHandler<GetForumTopicByIdQuery, Result<GetForumTopicByIdResponse>>
    {
        private readonly IForumRepository _forumRepository;
        private readonly IForumReadRepository _forumReadRepository;

        public GetForumTopicByIdQueryHandler(
            IForumRepository forumRepository,
            IForumReadRepository forumReadRepository)
        {
            _forumRepository = forumRepository;
            _forumReadRepository = forumReadRepository;
        }

        public async Task<Result<GetForumTopicByIdResponse>> Handle(GetForumTopicByIdQuery request, CancellationToken cancellationToken)
        {
            var topic = await _forumReadRepository.GetByIdAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                return Result.ResourceNotFound<GetForumTopicByIdResponse>(ForumTopicNotFound);
            }

            var success = await _forumRepository.IncrementViewCountAsync(request.TopicId, cancellationToken);
            if (!success)
            {
                return Result.ResourceNotFound<GetForumTopicByIdResponse>(ForumTopicNotFound);
            }

            return Result.Success(topic);
        }
    }
}
