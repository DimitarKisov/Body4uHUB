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
    }
}
