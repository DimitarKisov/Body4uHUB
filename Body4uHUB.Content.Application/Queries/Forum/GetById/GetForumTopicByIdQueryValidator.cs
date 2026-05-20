using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Queries.Forum.GetById
{
    public class GetForumTopicByIdQueryValidator : AbstractValidator<GetForumTopicByIdQuery>
    {
        public GetForumTopicByIdQueryValidator()
        {
            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage(ForumTopicIdRequired);
        }
    }
}
