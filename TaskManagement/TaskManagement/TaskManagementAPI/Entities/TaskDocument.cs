using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Entities
{
    public partial class TaskDocument
    {
        [Key]
        public int DocumentId { get; set; }
        public int TaskId { get; set; }
        public Tasks Tasks { get; set; }
        public string FilePath { get; set; }
    }
}
