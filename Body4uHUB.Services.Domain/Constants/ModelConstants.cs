namespace Body4uHUB.Services.Domain.Constants
{
    public class ModelConstants
    {
        public class TrainerProfileConstants
        {
            public const int BioMinLength = 50;
            public const int BioMaxLength = 2000;
            public const int MaxMoneyAmount = 5000;
            public const int MinMoneyAmount = 0;
            public const int SpecializationMinLength = 3;
            public const int SpecializationMaxLength = 100;
            public const int CertificationMinLength = 5;
            public const int CertificationMaxLength = 200;
            public const int RatingUpperLimit = 5;
            public const int RatingLowerLimit = 1;
            

            //public const string BioRequired = "Biography is required";
            //public const string BioLengthMessage = "Biography must be between {0} and {1} characters";
            //public const string SpecializationRequired = "At least one specialization is required";
            //public const string SpecializationLengthMessage = "Specialization must be between {0} and {1} characters";
            //public const string TrainerProfileNotFound = "Trainer profile not found";
            //public const string TrainerProfileAlreadyExists = "Trainer profile already exists for this user";
            //public const string YearsOfExperienceInvalid = "Years of experience must be greater than or equal to 0";
        }

        public class ReviewConstants
        {
            public const int MinRating = 1;
            public const int MaxRating = 5;
            public const int MinCommentLength = 5;
            public const int MaxCommentLength = 1000;

            public const string ReviewIdCannotBeZeroOrNegative = "Review ID must be greater than 0.";
        }

        public class ServiceOfferingConstants
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 100;
            public const int DescriptionMinLength = 20;
            public const int DescriptionMaxLength = 2000;
            public const decimal PriceMinValue = 0.01m;
            public const decimal PriceMaxValue = 10000m;
            public const int MinDurationMinutes = 15;
            public const int MaxDurationMinutes = 480;

            public const string ServiceAlreadyExists = "Service with this title already exists for this trainer";
            public const string ServiceOfferingIdCannotBeZeroOrNegative = "ServiceOffering ID must be greater than 0.";
            public const string ServiceNotFound = "Service offering not found";
        }

        public class ServiceOrderConstants
        {
            public const int NotesMinLength = 0;
            public const int NotesMaxLength = 1000;

            public const string OrderNotCompleted = "Cannot add a review to an order that is not completed.";
            public const string CannotMarkPaymentRefunded = "Payment can only be marked as refunded if it is currently pending.";
            public const string CannotMarkPaymentFailed = "Payment can only be marked as failed if it is not already completed or failed.";
            public const string CannotMarkPaymentCompleted = "Payment can only be marked as completed if it is currently pending.";
            public const string CannotReviewOwnService = "Trainers cannot review their own services.";
            public const string OrderCannotBeCancelled = "The order cannot be cancelled within 24 hours of the scheduled time.";
            public const string OrderAlreadyCancelled = "The order has already been cancelled.";
            public const string OnlyClientCanReview = "Only the client who placed the order can add a review.";
            public const string CannotConfirmNonPendingOrder = "Only pending orders can be confirmed.";
            public const string CannotCompleteNonConfirmedOrder = "Only confirmed orders can be completed.";
            public const string CannotCancelCompletedOrder = "Completed orders cannot be cancelled.";
            public const string ServiceOrderIdCannotBeZeroOrNegative = "ServiceOrder ID must be greater than 0.";
            public const string ReviewAlreadyExists = "A review has already been added for this order.";
            public const string ReviewNotFound = "Review not found for this order.";
        }
    }
}
