using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Application.Commands.Forum.EditForumTopic
{
    public class EditForumTopicCommandValidator : AbstractValidator<EditForumTopicCommand>
    {
        public EditForumTopicCommandValidator()
        {
            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage(ForumTopicIdRequired);

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(TitleRequired)
                .Length(TitleMinLength, TitleMaxLength).WithMessage(string.Format(TitleLengthMessage, TitleMinLength, TitleMaxLength));
        }
    }
}
