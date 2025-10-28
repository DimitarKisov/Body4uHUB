using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Exceptions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.EditUser
{
    public class EditUserCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        internal class EditUserCommandHandler : IRequestHandler<EditUserCommand, Unit>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;

            public EditUserCommandHandler(
                IUserRepository userRepository,
                IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(EditUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.Id);
                if (user == null)
                {
                    throw new NotFoundException(UserNotFound);
                }

                user.UpdateFirstName(request.FirstName);
                user.UpdateLastName(request.LastName);
                user.UpdateContactInfo(user.ContactInfo.Email, request.PhoneNumber);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
