using System.ComponentModel.DataAnnotations;

namespace ProgPoe.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [RegularExpression(@"^\+27\d{9}$", ErrorMessage = "Phone number must be in the format +27123456789")]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public string Role { get; set; }
        public List<string> Roles { get; set; }
    }
}

