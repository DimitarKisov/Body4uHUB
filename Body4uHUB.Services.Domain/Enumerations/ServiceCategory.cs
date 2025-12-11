using Body4uHUB.Shared;

namespace Body4uHUB.Services.Domain.Enumerations
{
    public class ServiceCategory : Enumeration
    {
        public static readonly ServiceCategory PersonalTraining = new(1, nameof(PersonalTraining));
        public static readonly ServiceCategory NutritionPlan = new(2, nameof(NutritionPlan));
        public static readonly ServiceCategory OnlineConsultation = new(3, nameof(OnlineConsultation));
        public static readonly ServiceCategory GroupTraining = new(4, nameof(GroupTraining));
        public static readonly ServiceCategory SpecializedProgram = new(5, nameof(SpecializedProgram));
        public static readonly ServiceCategory Coaching = new(6, nameof(Coaching));
        public static readonly ServiceCategory WorkoutPlan = new(7, nameof(WorkoutPlan));

        private ServiceCategory(int id, string name)
            : base(id, name)
        {
        }
    }
}
