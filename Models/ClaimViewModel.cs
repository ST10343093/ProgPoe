using System.ComponentModel.DataAnnotations;

namespace ProgPoe.Models
{
    public class ClaimViewModel
    {
        [Required(ErrorMessage = "Hours Worked is required.")]
        [Range(1, 100, ErrorMessage = "Hours Worked must be between 1 and 100.")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly Rate is required.")]
        [Range(50, 1000, ErrorMessage = "Hourly Rate must be between 50 and 1000.")]
        public decimal HourlyRate { get; set; }

        [MaxLength(500, ErrorMessage = "Notes can't exceed 500 characters.")]
        public string Notes { get; set; }

        [Display(Name = "Supporting Documents")]
        public List<IFormFile> SupportingDocuments { get; set; }
    }
}
