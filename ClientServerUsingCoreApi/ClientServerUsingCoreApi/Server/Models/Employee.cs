using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public bool IsActive { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:yyyy-MM-dd}")]
        public DateTime JoinDate { get; set; }
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }=new List<Experience>();
    }

    public class Experience
    {
        public int ExperienceId { get; set; }
        public string Title { get; set; } = null!;
        public int Duration { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;
    }
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> op):base(op)
        { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Experience> Experiences { get; set; }
    }
}
