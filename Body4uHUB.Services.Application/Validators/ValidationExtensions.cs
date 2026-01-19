using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Shared.Domain.Enumerations;

namespace Body4uHUB.Services.Application.Validators
{
    public static class ValidationExtensions
    {
        private static readonly HashSet<string> ValidServiceCategories = Enumeration.GetAll<ServiceCategory>().Select(x => x.Name).ToHashSet();

        public static bool BeValidServiceCategory(string serviceType)
        {
            return !string.IsNullOrWhiteSpace(serviceType) &&
                   ValidServiceCategories.Contains(serviceType);
        }
    }
}
