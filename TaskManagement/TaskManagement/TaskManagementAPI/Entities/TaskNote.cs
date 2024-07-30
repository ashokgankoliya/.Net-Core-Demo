using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Entities
{
    public partial class TaskNote
    {
        [Key]
        public int NoteId { get; set; }
        public int TaskId { get; set; }
        public Tasks Tasks { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
