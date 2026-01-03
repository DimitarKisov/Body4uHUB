using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Exceptions;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;

namespace Body4uHUB.Services.Domain.Models
{
    public class ServiceOffering : Entity<ServiceOfferingId>
    {
        private readonly List<Review> _reviews = new();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Money Price { get; private set; }
        public int? DurationInMinutes { get; private set; }
        public ServiceCategory Category { get; private set; }
        public bool IsActive { get; private set; }
        public int MaxParticipants { get; private set; }
        public bool IsOnline { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public decimal AverageRating { get; private set; }
        public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

        private ServiceOffering()
            : base(default!)
        {
        }

        private ServiceOffering(
            string name,
            string description,
            Money price,
            int? durationInMinutes,
            ServiceCategory category,
            bool isActive,
            int maxParticipants,
            bool isOnline,
            DateTime? startDate,
            DateTime? endDate)
            : base(default!)
        {
            Name = name;
            Description = description;
            Price = price;
            DurationInMinutes = durationInMinutes;
            Category = category;
            IsActive = isActive;
            MaxParticipants = maxParticipants;
            IsOnline = isOnline;
            StartDate = startDate;
            EndDate = endDate;
            AverageRating = 0;
        }

        public static ServiceOffering Create(
            string name,
            string description,
            Money price,
            int? durationInMinutes,
            ServiceCategory category,
            bool isActive,
            int maxParticipants,
            bool isOnline,
            DateTime? startDate,
            DateTime? endDate)
        {
            Validate(name, description, durationInMinutes, maxParticipants, startDate, endDate);

            return new ServiceOffering(
                name,
                description,
                price,
                durationInMinutes,
                category,
                isActive,
                maxParticipants,
                isOnline,
                startDate,
                endDate);
        }

        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            ValidateDescription(description);
            Description = description;
        }

        public void UpdatePrice(Money price)
        {
            Price = price;
        }

        public void UpdateDurationInMinutes(int? durationInMinutes)
        {
            ValidateDurationInMinutes(durationInMinutes);
            DurationInMinutes = durationInMinutes;
        }

        public void UpdateCategory(ServiceCategory category)
        {
            Category = category;
        }

        public void UpdateMaxParticipants(int maxParticipants)
        {
            ValidateMaxParticipants(maxParticipants);
            MaxParticipants = maxParticipants;
        }

        public void UpdateStartDate(DateTime? startDate)
        {
            ValidateStartDate(startDate);
            StartDate = startDate;
        }

        public void UpdateEndDate(DateTime? endDate)
        {
            ValidateEndDate(endDate);
            EndDate = endDate;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void SetOnlineMode(bool isOnline)
        {
            IsOnline = isOnline;
        }

        public void AddReview(Guid clientId, ServiceOrderId orderId, int rating, string comment)
        {

            var review = Review.Create(clientId, orderId, rating, comment);
            _reviews.Add(review);

            RecalculateAverageRating();
        }

        private static void Validate(string name, string description, int? durationInMinutes, int maxParticipants, DateTime? startDate, DateTime? endDate)
        {
            ValidateName(name);
            ValidateDescription(description);
            ValidateDurationInMinutes(durationInMinutes);
            ValidateMaxParticipants(maxParticipants);
            ValidateStartDate(startDate);
            ValidateEndDate(endDate);
        }

        private static void ValidateName(string name)
        {
            Guard.AgainstEmptyString<InvalidServiceOfferingException>(name, nameof(Name));
            Guard.ForStringLength<InvalidServiceOfferingException>(name, NameMinLength, NameMaxLength, nameof(Name));
        }

        private static void ValidateDescription(string description)
        {
            Guard.AgainstEmptyString<InvalidServiceOfferingException>(description, nameof(Description));
            Guard.ForStringLength<InvalidServiceOfferingException>(description, DescriptionMinLength, DescriptionMaxLength, nameof(Description));
        }

        private static void ValidateDurationInMinutes(int? durationInMinutes)
        {
            if (durationInMinutes.HasValue)
            {
                Guard.AgainstOutOfRange<InvalidServiceOfferingException>(durationInMinutes.Value, MinDurationMinutes, MaxDurationMinutes, nameof(DurationInMinutes));
            }
        }

        private static void ValidateMaxParticipants(int maxParticipants)
        {
            Guard.AgainstNegative<InvalidServiceOfferingException>(maxParticipants, nameof(MaxParticipants));
        }

        private static void ValidateStartDate(DateTime? startDate)
        {
            if (startDate.HasValue)
            {
                Guard.AgainstPastDate<InvalidServiceOfferingException>(startDate.Value, nameof(StartDate));
            }
        }

        private static void ValidateEndDate(DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                Guard.AgainstMoreThanOneYearInFuture<InvalidServiceOfferingException>(endDate.Value, nameof(EndDate));
            }
        }

        private void RecalculateAverageRating()
        {
            if (_reviews.Any())
            {
                AverageRating = (decimal)_reviews.Average(r => r.Rating);
            }
            else
            {
                AverageRating = 0;
            }
        }
    }
}
