using Body4uHUB.Services.Domain.Exceptions;
using Body4uHUB.Shared;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.TrainerProfile;

namespace Body4uHUB.Services.Domain.Models
{
    public class TrainerProfile : Entity<Guid>, IAggregateRoot
    {
        private readonly List<string> _specializations = new();
        private readonly List<string> _certifications = new();

        public Guid UserId { get; private set; }
        public string Bio { get; private set; }
        public int YearsOfExperience { get; private set; }
        public decimal AverageRating { get; private set; }
        public int TotalReviews { get; private set; }

        public IReadOnlyCollection<string> Specializations => _specializations.AsReadOnly();
        public IReadOnlyCollection<string> Certifications => _certifications.AsReadOnly();

        private TrainerProfile()
            : base(Guid.Empty)
        {
        }

        private TrainerProfile(Guid userId, string bio, int yearsOfExperience)
            : base(Guid.Empty)
        {
            UserId = userId;
            Bio = bio;
            YearsOfExperience = yearsOfExperience;
            AverageRating = 0;
            TotalReviews = 0;
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

        public void UpdateRating(int rating)
        {
            ValidateRating(rating);

            TotalReviews++;
            AverageRating = ((AverageRating * (TotalReviews - 1)) + rating) / TotalReviews;
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
            Guard.AgainsOutOfRange<InvalidTrainerProfileException>(rating, RatingLowerLimit, RatingUpperLimit, nameof(rating));
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
