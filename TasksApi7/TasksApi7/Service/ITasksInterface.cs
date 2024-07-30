using TasksApi7.Entities;
using TasksApi7.Model;

namespace TasksApi7.Service
{
    public interface ITasksInterface
    {
        Task<GetTasksResponse> GetTasks(int userId);
        Task<SaveTaskResponse> SaveTask(Tasks task);
        Task<DeleteTaskResponse> DeleteTask(int taskId, int userId);
    }
}
