using System.ComponentModel.DataAnnotations;

namespace ProgPoe.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        [Required(ErrorMessage = "Report Name is required.")]
        [StringLength(100, ErrorMessage = "Report Name cannot exceed 100 characters.")]
        public string ReportName { get; set; }

        [Required(ErrorMessage = "Report Type is required.")]
        [StringLength(50, ErrorMessage = "Report Type cannot exceed 50 characters.")]
        public string ReportType { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DateGreaterThan("StartDate", ErrorMessage = "End Date must be after Start Date.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "File Path is required.")]
        [StringLength(200, ErrorMessage = "File Path cannot exceed 200 characters.")]
        [RegularExpression(@"^.*\.(pdf|docx|xlsx)$", ErrorMessage = "File Path must be a .pdf, .docx, or .xlsx file.")]
        public string FilePath { get; set; }
    }
}

