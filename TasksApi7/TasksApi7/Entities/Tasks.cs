using System;
using System.Collections.Generic;

namespace TasksApi7.Entities
{
    public partial class Tasks

    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime Ts { get; set; }
        public virtual User Users { get; set; }


    }
}
