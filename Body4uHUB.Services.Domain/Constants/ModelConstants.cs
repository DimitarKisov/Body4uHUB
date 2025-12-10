namespace Body4uHUB.Services.Domain.Constants
{
    public class ModelConstants
    {
        public class TrainerProfile
        {
            public const int BioMinLength = 50;
            public const int BioMaxLength = 2000;
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
    }
}
