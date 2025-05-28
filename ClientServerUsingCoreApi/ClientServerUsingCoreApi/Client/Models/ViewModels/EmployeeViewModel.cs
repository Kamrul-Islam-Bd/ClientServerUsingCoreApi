using System.ComponentModel.DataAnnotations;

namespace Client.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public bool IsActive { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime JoinDate { get; set; }
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? ProfileFile { get; set; }
        public string Title { get; set; } = null!;
        public int Duration { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    }
}
