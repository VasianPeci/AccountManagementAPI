using System.ComponentModel.DataAnnotations;

namespace AccountManagement.DTO
{
    public class UpdateClientRequestDto
    {
        // Identity fields
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string? Password { get; set; }

        public string[]? Roles { get; set; }

        // Client fields
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "First name is too long")]
        public string? FirstName { get; set; }

        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Last name is too long")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }
    }
}