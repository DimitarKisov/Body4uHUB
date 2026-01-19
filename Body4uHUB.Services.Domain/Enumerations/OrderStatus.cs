using Body4uHUB.Shared.Domain.Enumerations;

namespace Body4uHUB.Services.Domain.Enumerations
{
    public class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Pending = new(1, nameof(Pending));
        public static readonly OrderStatus Confirmed = new(2, nameof(Confirmed));
        public static readonly OrderStatus Completed = new(3, nameof(Completed));
        public static readonly OrderStatus Cancelled = new(4, nameof(Cancelled));

        private OrderStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}
