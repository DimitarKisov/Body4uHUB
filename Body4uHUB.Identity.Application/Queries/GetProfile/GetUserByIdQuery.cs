using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Queries.GetProfile
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public Guid Id { get; set; }

        internal class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
        {
            private readonly IUserRepository _userRepository;

            public GetUserByIdQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
                if (user == null)
                {
                    return Result.UnprocessableEntity<UserDto>(UserNotFound);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.ContactInfo.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.ContactInfo.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    IsEmailConfirmed = user.IsEmailConfirmed
                };

                return Result.Success(userDto);
            }
        }
    }
}
