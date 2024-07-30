using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Entities
{
    public partial class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int? ManagerId { get; set; }
        public Employee Manager { get; set; }
        public ICollection<Tasks> Tasks { get; set; }
    }
    public class EmployeeCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int? ManagerId { get; set; }
    }
}
