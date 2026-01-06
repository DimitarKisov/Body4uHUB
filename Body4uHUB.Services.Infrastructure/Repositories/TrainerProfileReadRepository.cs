using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Services.Infrastructure.Persistence;
using Body4uHUB.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Services.Infrastructure.Repositories
{
    internal class TrainerProfileReadRepository : ITrainerProfileReadRepository
    {
        private readonly ServicesDbContext _dbContext;

        public TrainerProfileReadRepository(ServicesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TrainerProfileDto>> GetAllActiveAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TrainerProfiles
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(x => new TrainerProfileDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Bio = x.Bio,
                    Specializations = x.Specializations.ToList(),
                    Certifications = x.Certifications.ToList(),
                    YearsOfExperience = x.YearsOfExperience,
                    AverageRating = x.AverageRating,
                    TotalReviews = x.TotalReviews,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt,
                    ServiceOfferings = x.Services.Select(y => new ServiceOfferingDto
                    {
                        Id = y.Id,
                        ServiceName = y.Name,
                        Description = y.Description,
                        Price = y.Price.Amount,
                        DurationInMinutes = y.DurationInMinutes
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<TrainerProfileDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TrainerProfiles
                .Where(x => x.Id == id)
                .Select(x => new TrainerProfileDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Bio = x.Bio,
                    Specializations = x.Specializations.ToList(),
                    Certifications = x.Certifications.ToList(),
                    YearsOfExperience = x.YearsOfExperience,
                    AverageRating = x.AverageRating,
                    TotalReviews = x.TotalReviews,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt,
                    ServiceOfferings = x.Services.Select(y => new ServiceOfferingDto
                    {
                        Id = y.Id,
                        ServiceName = y.Name,
                        Description = y.Description,
                        Price = y.Price.Amount,
                        DurationInMinutes = y.DurationInMinutes
                    }).ToList()
                })
                .OrderByDescending(x => x.AverageRating)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<ServiceOfferingDto>> GetServiceOfferingsByTrainerIdAsync(Guid trainerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TrainerProfiles
                .Where(x => x.Id == trainerId)
                .SelectMany(x => x.Services)
                .Where(x => x.IsActive)
                .Select(x => new ServiceOfferingDto
                {
                    Id = x.Id,
                    ServiceName = x.Name,
                    Description = x.Description,
                    Price = x.Price.Amount,
                    Currency = x.Price.Currency,
                    DurationInMinutes = x.DurationInMinutes,
                    ServiceCategory = x.Category.ToString(),
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        
    }
}
