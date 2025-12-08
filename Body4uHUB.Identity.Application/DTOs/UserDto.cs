namespace Body4uHUB.Identity.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public ICollection<RoleDto> Roles { get; set; }
    }
}
