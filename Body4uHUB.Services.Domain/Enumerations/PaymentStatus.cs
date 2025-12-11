using Body4uHUB.Shared;

namespace Body4uHUB.Services.Domain.Enumerations
{
    public class PaymentStatus : Enumeration
    {
        public static readonly PaymentStatus Pending = new(1, nameof(Pending));
        public static readonly PaymentStatus Completed = new(2, nameof(Completed));
        public static readonly PaymentStatus Failed = new(3, nameof(Failed));
        public static readonly PaymentStatus Refunded = new(4, nameof(Refunded));

        private PaymentStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}
