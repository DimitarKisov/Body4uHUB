namespace Body4uHUB.Content.Application.Commands.Forum.CreateForumTopic
{
    using FluentValidation;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;
    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

    public class CreateForumTopicCommandValidator : AbstractValidator<CreateForumTopicCommand>
    {
        public CreateForumTopicCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(TitleRequired)
                .Length(TitleMinLength, TitleMaxLength).WithMessage(TitleLengthMessage);

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage(AuthorIdRequired);
        }
    }
}
