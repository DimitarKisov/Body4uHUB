using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public Guid Id { get; set; }

        internal class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
        {
            private readonly IUserReadRepository _userReadRepository;

            public GetUserByIdQueryHandler(IUserReadRepository userReadRepository)
            {
                _userReadRepository = userReadRepository;
            }

            public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            {
                var user = await _userReadRepository.GetByIdAsync(request.Id, cancellationToken);
                if (user == null)
                {
                    return Result.UnprocessableEntity<UserDto>(UserNotFound);
                }

                return Result.Success(user);
            }
        }
    }
}
