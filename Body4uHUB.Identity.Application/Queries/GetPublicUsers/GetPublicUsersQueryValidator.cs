using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Queries.GetPublicUsers
{
    public class GetPublicUsersQueryValidator : AbstractValidator<GetPublicUsersQuery>
    {
        public GetPublicUsersQueryValidator()
        {
            RuleFor(x => x.Ids)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(PublicUserIdsRequired)
                // A page repeats the same author, so the limit counts distinct ids.
                .Must(ids => ids.Distinct().Count() <= MaxPublicUserIds).WithMessage(string.Format(PublicUserIdsLimit, MaxPublicUserIds));

            RuleForEach(x => x.Ids)
                .NotEmpty().WithMessage(UserIdRequired);
        }
    }
}
