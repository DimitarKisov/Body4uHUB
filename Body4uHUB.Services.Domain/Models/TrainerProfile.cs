using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Exceptions;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Guards;
using Body4uHUB.Shared.Domain.Base;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;

namespace Body4uHUB.Services.Domain.Models
{
    public class TrainerProfile : AggregateRoot<Guid>
    {
        private readonly List<string> _specializations = new();
        private readonly List<string> _certifications = new();
        private readonly List<ServiceOffering> _services = new();

        public Guid UserId { get; private set; }
        public string Bio { get; private set; }
        public int YearsOfExperience { get; private set; }
        public decimal AverageRating { get; private set; }
        public int TotalReviews { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<string> Specializations => _specializations.AsReadOnly();
        public IReadOnlyCollection<string> Certifications => _certifications.AsReadOnly();
        public IReadOnlyCollection<ServiceOffering> Services => _services.AsReadOnly();

        private TrainerProfile()
            : base(Guid.NewGuid())
        {
        }

        private TrainerProfile(Guid userId, string bio, int yearsOfExperience)
            : base(Guid.NewGuid())
        {
            UserId = userId;
            Bio = bio;
            YearsOfExperience = yearsOfExperience;
            AverageRating = 0;
            TotalReviews = 0;
            IsActive = true;
        }

        public static TrainerProfile Create(Guid userId, string bio, int yearsOfExperience)
        {
            Validate(userId, bio, yearsOfExperience);

            return new TrainerProfile(userId, bio, yearsOfExperience);
        }

        public void UpdateBio(string bio)
        {
            ValidateBio(bio);
            Bio = bio;
        }

        public void UpdateYearsOfExperience(int yearsOfExperience)
        {
            ValidateYearsOfExperience(yearsOfExperience);
            YearsOfExperience = yearsOfExperience;
        }

        public void UpdateRating()
        {
            var servicesWithReviews = _services
                .Where(x => x.Reviews.Count != 0)
                .ToList();

            if (servicesWithReviews.Any())
            {
                AverageRating = servicesWithReviews.Average(x => x.AverageRating);
                TotalReviews = servicesWithReviews.Sum(x => x.Reviews.Count);
            }
            else
            {
                AverageRating = 0;
                TotalReviews = 0;
            }
        }

        public void ActivateProfile()
        {
            IsActive = true;
        }

        public void DeactivateProfile()
        {
            IsActive = false;
        }

        public void AddSpecialization(string specialization)
        {
            ValidateSpecialization(specialization);

            if (_specializations.Contains(specialization))
            {
                return;
            }

            _specializations.Add(specialization);
        }

        public void RemoveSpecialization(string specialization)
        {
            _specializations.Remove(specialization);
        }

        public void ClearSpecializations()
        {
            _specializations.Clear();
        }

        public void AddCertification(string certification)
        {
            ValidateCertification(certification);

            if (_certifications.Contains(certification))
            {
                return;
            }

            _certifications.Add(certification);
        }

        public void RemoveCertification(string certification)
        {
            _certifications.Remove(certification);
        }

        public void ClearCertifications()
        {
            _certifications.Clear();
        }

        public int AddService(
            string title,
            string description,
            Money price,
            int durationMinutes,
            ServiceCategory category,
            bool isActive,
            int maxParticipants,
            bool isOnline,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (_services.Any(x => x.Name.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidServiceOfferingException(ServiceOfferingAlreadyExists);
            }

            var service = ServiceOffering.Create(
                title,
                description,
                price,
                durationMinutes,
                category,
                true,
                maxParticipants,
                isOnline,
                startDate,
                endDate);

            _services.Add(service);

            return service.Id;
        }

        public void UpdateServiceDetails(
            int serviceId,
            string name,
            string description,
            decimal price,
            int durationMinutes,
            Guid requesterId,
            bool isAdmin)
        {
            EnsureCanBeModifiedBy(requesterId, isAdmin);

            var service = GetService(serviceId);
            var money = Money.Create(price, service.Price.Currency);

            service.UpdateName(name);
            service.UpdateDescription(description);
            service.UpdatePrice(money);
            service.UpdateDurationInMinutes(durationMinutes);
        }

        private void EnsureCanBeModifiedBy(Guid requesterId, bool isAdmin)
        {
            if (!isAdmin && UserId != requesterId)
            {
                throw new DomainAuthorizationException(TrainerProfileForbidden);
            }
        }

        public void ActivateService(int id)
        {
            var service = GetService(id);
            service.Activate();
        }

        public void DeactivateService(int id)
        {
            var service = GetService(id);
            service.Deactivate();
        }

        public void RemoveService(int id)
        {
            var service = GetService(id);
            _services.Remove(service);
        }

        public ServiceOffering GetService(int id)
        {
            var service = _services.FirstOrDefault(x => x.Id == id);
            if (service == null)
            {
                throw new InvalidServiceOfferingException(ServiceOfferingNotFound);
            }

            return service;
        }

        private static void Validate(Guid userId, string bio, int yearsOfExperience)
        {
            ValidateUserId(userId);
            ValidateBio(bio);
            ValidateYearsOfExperience(yearsOfExperience);
        }

        private static void ValidateUserId(Guid userId)
        {
            Guard.AgainstEmptyGuid<InvalidTrainerProfileException>(userId, nameof(UserId));
        }

        private static void ValidateBio(string bio)
        {
            Guard.AgainstEmptyString<InvalidTrainerProfileException>(bio, nameof(Bio));
            Guard.ForStringLength<InvalidTrainerProfileException>(bio, BioMinLength, BioMaxLength, nameof(Bio));
        }

        private static void ValidateYearsOfExperience(int yearsOfExperience)
        {
            Guard.AgainstNegative<InvalidTrainerProfileException>(yearsOfExperience, nameof(YearsOfExperience));
        }

        private static void ValidateRating(int rating)
        {
            Guard.AgainstOutOfRange<InvalidTrainerProfileException>(rating, RatingLowerLimit, RatingUpperLimit, nameof(rating));
        }

        private static void ValidateSpecialization(string specialization)
        {
            Guard.AgainstEmptyString<InvalidTrainerProfileException>(specialization, nameof(specialization));
            Guard.ForStringLength<InvalidTrainerProfileException>(specialization, SpecializationMinLength, SpecializationMaxLength, nameof(specialization));
        }

        private static void ValidateCertification(string certification)
        {
            Guard.AgainstEmptyString<InvalidTrainerProfileException>(certification, nameof(certification));
            Guard.ForStringLength<InvalidTrainerProfileException>(certification, CertificationMinLength, CertificationMaxLength, nameof(certification));
        }
    }
}
