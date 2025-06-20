﻿using System.ComponentModel.DataAnnotations;

namespace Client.Models.ViewModels
{
    public class Common
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public bool IsActive { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime JoinDate { get; set; }
        public string? ImageName { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Experiences { get; set; } = null!;
    }
}
