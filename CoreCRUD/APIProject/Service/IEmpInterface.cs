
using APIProject.Model;

namespace APIProject.Service
{
    public interface IEmpInterface
    {
        Task<GetEmpResponse> GetTasks();
        //Task<SaveTaskResponse> SaveTask(Tasks task);
        //Task<DeleteTaskResponse> DeleteTask(int taskId, int userId);
    }
}
