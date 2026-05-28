namespace AccountManagement.DTO
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string[] Roles { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime Birthdate { get; set; }
        public string Phone { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
