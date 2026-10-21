using Body4uHUB.Content.Application.Abstractions;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Content.Infrastructure.Settings;
using Body4uHUB.Shared.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Body4uHUB.Content.Infrastructure.Services
{
    internal sealed class ViewTracker(
        ContentDbContext dbContext,
        TimeProvider timeProvider,
        IOptions<ViewTrackingSettings> options)
        : IViewTracker
    {
        private static readonly TimeZoneInfo ViewDayTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia");

        private readonly ContentDbContext _dbContext = dbContext;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly byte[] _hashKey = Encoding.UTF8.GetBytes(options.Value.HashKey);

        public static DateOnly Today(TimeProvider timeProvider)
        {
            var now = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), ViewDayTimeZone);
            return DateOnly.FromDateTime(now.DateTime);
        }

        public async Task<bool> TryRegisterViewAsync(ViewTarget target, int targetId, string visitorIp, CancellationToken cancellationToken)
        {
            var viewDate = Today(_timeProvider);
            var visitorHash = HashVisitor(visitorIp, viewDate);
            var targetType = (byte)target;

            try
            {
                // A repeated view is the common case, so it is skipped by NOT EXISTS instead of failing on the key.
                var inserted = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO ContentViews (TargetType, TargetId, VisitorHash, ViewDate)
                    SELECT {targetType}, {targetId}, {visitorHash}, {viewDate}
                    WHERE NOT EXISTS (
                        SELECT 1 FROM ContentViews
                        WHERE TargetType = {targetType} AND TargetId = {targetId} AND VisitorHash = {visitorHash} AND ViewDate = {viewDate})
                    """, cancellationToken);

                return inserted > 0;
            }
            catch (SqlException ex) when (ex.IsUniqueConstraintViolation())
            {
                // A concurrent request from the same visitor inserted the row first.
                return false;
            }
        }

        // The date is part of the hash, so the same visitor cannot be linked across days.
        private byte[] HashVisitor(string visitorIp, DateOnly viewDate)
        {
            var value = $"{visitorIp ?? "unknown"}|{viewDate:yyyy-MM-dd}";
            return HMACSHA256.HashData(_hashKey, Encoding.UTF8.GetBytes(value));
        }
    }
}
