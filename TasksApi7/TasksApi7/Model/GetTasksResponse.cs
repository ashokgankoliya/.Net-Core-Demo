using TasksApi7.Entities;

namespace TasksApi7.Model
{
    public class GetTasksResponse : BaseResponse
    {
        public List<Tasks> Taskss { get; set; }
    }
}
