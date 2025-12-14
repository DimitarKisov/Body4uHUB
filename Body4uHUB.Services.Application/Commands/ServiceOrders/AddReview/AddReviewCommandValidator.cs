using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.AddReview
{
    public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
    {
        public AddReviewCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage(ServiceOrderNotFound);

            RuleFor(x => x.Rating)
                .InclusiveBetween(MinRating, MaxRating).WithMessage(string.Format(ServiceRatingOutOfRange, MinRating, MaxRating));

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage(CommentRequired)
                .Length(MinCommentLength, MaxCommentLength).WithMessage(string.Format(CommentLength, MinCommentLength, MaxCommentLength));

            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage(ClientIdRequired);
        }
    }
}
