using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgPoe.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Hours Worked is required.")]
        [Range(1, 150, ErrorMessage = "Hours Worked must be between 1 and 150.")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly Rate is required.")]
        [Range(200, 1000, ErrorMessage = "Hourly Rate must be between 200 and 1000.")]
        public decimal HourlyRate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [MaxLength(500, ErrorMessage = "Notes can't exceed 500 characters.")]
        public string Notes { get; set; }

        [Required]
        public DateTime DateSubmitted { get; set; }

        public string Status { get; set; } = "Pending";

        public bool IsApprovedByCoordinator { get; set; } = false;

        public bool IsApprovedByManager { get; set; } = false;

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        public DateTime EndDate { get; set; }

        [Required]
        public string PaymentStatus { get; set; } = "Pending";
    }
}

