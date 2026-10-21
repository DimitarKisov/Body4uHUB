using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Shared.Infrastructure.Persistence
{
    public static class DbUpdateExceptionExtensions
    {
        // SQL Server error numbers for duplicate key in a unique index / unique constraint.
        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

        public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
        {
            return exception.InnerException is SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation };
        }
    }
}
