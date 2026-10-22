using Body4uHUB.Content.Application.Abstractions;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Queries.Forum.GetById;

public record GetForumTopicByIdQuery(int TopicId, string VisitorIp) : IRequest<Result<GetForumTopicByIdResponse>>;

internal sealed class GetForumTopicByIdQueryHandler(
    IForumRepository forumRepository,
    IForumReadRepository forumReadRepository,
    IViewTracker viewTracker)
    : IRequestHandler<GetForumTopicByIdQuery, Result<GetForumTopicByIdResponse>>
{
    private readonly IForumRepository _forumRepository = forumRepository;
    private readonly IForumReadRepository _forumReadRepository = forumReadRepository;
    private readonly IViewTracker _viewTracker = viewTracker;

    public async Task<Result<GetForumTopicByIdResponse>> Handle(GetForumTopicByIdQuery request, CancellationToken cancellationToken)
    {
        var topic = await _forumReadRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic == null)
        {
            return Result.ResourceNotFound<GetForumTopicByIdResponse>(ForumTopicNotFound);
        }

        if (await _viewTracker.TryRegisterViewAsync(ViewTarget.ForumTopic, request.TopicId, request.VisitorIp, cancellationToken))
        {
            var success = await _forumRepository.IncrementViewCountAsync(request.TopicId, cancellationToken);
            if (!success)
            {
                return Result.ResourceNotFound<GetForumTopicByIdResponse>(ForumTopicNotFound);
            }

            topic = topic with { ViewCount = topic.ViewCount + 1 };
        }

        return Result.Success(topic);
    }
}
