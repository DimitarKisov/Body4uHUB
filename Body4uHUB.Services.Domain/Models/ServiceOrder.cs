using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Exceptions;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Domain.Models
{
    public class ServiceOrder : Entity<ServiceOrderId>, IAggregateRoot
    {
        private Review _review;

        public Guid ClientId { get; private set; }
        public Guid TrainerId { get; private set; }
        public ServiceOfferingId ServiceOfferingId { get; private set; }
        public OrderStatus Status { get; private set; }
        public Money TotalPrice { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public string Notes { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public Review Review => _review;
        public bool HasReview => _review != null;

        private ServiceOrder()
            : base(default!)
        {
        }

        private ServiceOrder(Guid clientId,
            Guid trainerId,
            ServiceOfferingId serviceId,
            OrderStatus status,
            Money totalPrice,
            PaymentStatus paymentStatus,
            string notes)
            : base(default!)
        {
            ClientId = clientId;
            TrainerId = trainerId;
            ServiceOfferingId = serviceId;
            Status = status;
            TotalPrice = totalPrice;
            PaymentStatus = paymentStatus;
            Notes = notes;
        }

        public static ServiceOrder Create(
            Guid clientId,
            Guid trainerId,
            ServiceOfferingId serviceId,
            OrderStatus status,
            Money totalPrice,
            PaymentStatus paymentStatus,
            string notes)
        {
            Validate(clientId, trainerId, notes);

            return new ServiceOrder(
                clientId,
                trainerId,
                serviceId,
                status,
                totalPrice,
                paymentStatus,
                notes);
        }

        public void UpdateNotes(string notes)
        {
            ValidateNotes(notes);
            Notes = notes;
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidServiceOrderException(CannotConfirmNonPendingOrder);
            }

            Status = OrderStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Cancelled)
            {
                throw new InvalidServiceOrderException(OrderAlreadyCancelled);
            }

            if (Status == OrderStatus.Completed)
            {
                throw new InvalidServiceOrderException(CannotCancelCompletedOrder);
            }

            if (CreatedAt.Subtract(DateTime.UtcNow).TotalHours < 24)
            {
                throw new InvalidServiceOrderException(OrderCannotBeCancelled);
            }

            Status = OrderStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Confirmed)
            {
                throw new InvalidServiceOrderException(CannotCompleteNonConfirmedOrder);
            }

            Status = OrderStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkPaymentCompleted()
        {
            if (PaymentStatus != PaymentStatus.Pending)
            {
                throw new InvalidServiceOrderException(CannotMarkPaymentCompleted);
            }

            PaymentStatus = PaymentStatus.Completed;
        }

        public void MarkPaymentFailed()
        {
            if (PaymentStatus == PaymentStatus.Completed || PaymentStatus == PaymentStatus.Completed)
            {
                throw new InvalidServiceOrderException(CannotMarkPaymentFailed);
            }

            PaymentStatus = PaymentStatus.Failed;
        }

        public void MarkPaymentRefunded()
        {
            if (PaymentStatus != PaymentStatus.Pending)
            {
                throw new InvalidServiceOrderException(CannotMarkPaymentRefunded);
            }

            PaymentStatus = PaymentStatus.Refunded;
        }

        public void AddReview(int rating, string comment, Guid clientId)
        {
            if (Status != OrderStatus.Completed)
            {
                throw new InvalidReviewException(OrderNotCompleted);
            }

            if (HasReview)
            {
                throw new InvalidServiceOrderException(ReviewAlreadyExists);
            }

            if (ClientId != clientId)
            {
                throw new InvalidReviewException(OnlyClientCanReview);
            }

            if (ClientId == TrainerId)
            {
                throw new InvalidReviewException(CannotReviewOwnService);
            }

            var review = Review.Create(rating, comment);

            _review = review;
        }

        public void UpdateReview(int rating, string comment, Guid clientId)
        {
            if (_review == null)
            {
                throw new InvalidReviewException(ReviewNotFound);
            }

            if (ClientId != clientId)
            {
                throw new InvalidReviewException(OnlyClientCanReview);
            }

            _review.Update(rating, comment);
        }

        private static void Validate(
            Guid userId,
            Guid trainerId,
            string notes)
        {
            ValidateClientId(userId);
            ValidateTrainerId(trainerId);
            ValidateNotes(notes);
        }

        private static void ValidateClientId(Guid clientId)
        {
            Guard.AgainstDefault<InvalidServiceOrderException, Guid>(clientId, nameof(ClientId));
        }

        private static void ValidateTrainerId(Guid trainerId)
        {
            Guard.AgainstDefault<InvalidServiceOrderException, Guid>(trainerId, nameof(TrainerId));
        }

        private static void ValidateNotes(string notes)
        {
            Guard.AgainstEmptyString<InvalidServiceOrderException>(notes, nameof(Notes));
            Guard.ForStringLength<InvalidServiceOrderException>(notes, NotesMinLength, NotesMaxLength, nameof(Notes));
        }
    }
}
