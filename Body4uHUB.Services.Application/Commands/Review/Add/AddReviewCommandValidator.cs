using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.Review.Add
{
    public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
    {
        public AddReviewCommandValidator()
        {
            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage(TrainerProfileNotFound);

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage(ServiceOfferingNotFound);

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage(ServiceOrderNotFound);

            RuleFor(x => x.Rating)
                .InclusiveBetween(MinRating, MaxRating).WithMessage(string.Format(ServiceRatingOutOfRange, MinRating, MaxRating));

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage(CommentRequired)
                .Length(MinCommentLength, MaxCommentLength).WithMessage(string.Format(CommentLength, MinCommentLength, MaxCommentLength));
        }
    }
}
