using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Entities
{
    public partial class Tasks
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public ICollection<TaskNote> Notes { get; set; }
        public ICollection<TaskDocument> Documents { get; set; }
    }

    public class TaskCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        [Required]
        public int EmployeeId { get; set; }
    }
}
