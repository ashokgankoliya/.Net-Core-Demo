using TasksApi7.Entities;

namespace TasksApi7.Model
{
    public class SaveTaskResponse : BaseResponse
    {
        public Tasks Taskss { get; set; }
    }
}
