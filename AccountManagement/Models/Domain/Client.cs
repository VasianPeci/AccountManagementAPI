using AccountManagement.Models.Identity;

namespace AccountManagement.Models.Domain
{
    public class Client
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime Birthdate { get; set; }
        public string Phone { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        // Link to Identity user
        public string UserId { get; set; }
    }
}