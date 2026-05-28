using System.ComponentModel.DataAnnotations;

namespace AccountManagement.DTO
{
    public class RegisterRequestDto
    {
        // Identity part
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string[] Roles { get; set; }

        // Client part
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime Birthdate { get; set; }

        public string Phone { get; set; }
    }
}