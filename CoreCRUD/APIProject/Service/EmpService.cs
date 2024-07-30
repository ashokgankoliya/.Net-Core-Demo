using APIProject.Data;
using APIProject.Model;
using Microsoft.EntityFrameworkCore;


namespace APIProject.Service
{
    public class EmpService : IEmpInterface
    {
        private readonly APIDbContext tasksDbContext;

        public EmpService(APIDbContext tasksDbContext)
        {
            this.tasksDbContext = tasksDbContext;
        }

        //public async Task<DeleteTaskResponse> DeleteTask(int taskId, int userId)
        //{
        //    var task = await tasksDbContext.Taskss.FindAsync(taskId);

        //    if (task == null)
        //    {
        //        return new DeleteTaskResponse
        //        {
        //            Success = false,
        //            Error = "Task not found",
        //            ErrorCode = "T01"
        //        };
        //    }

        //    if (task.UserId != userId)
        //    {
        //        return new DeleteTaskResponse
        //        {
        //            Success = false,
        //            Error = "You don't have access to delete this task",
        //            ErrorCode = "T02"
        //        };
        //    }
        //    tasksDbContext.Taskss.Remove(task);

        //    var saveResponse = await tasksDbContext.SaveChangesAsync();
        //    if (saveResponse >= 0)
        //    {
        //        return new DeleteTaskResponse
        //        {
        //            Success = true,
        //            TaskId = task.Id
        //        };
        //    }

        //    return new DeleteTaskResponse
        //    {
        //        Success = false,
        //        Error = "Unable to delete task",
        //        ErrorCode = "T03"
        //    };
        //}

        public async Task<GetEmpResponse> GetTasks()
        {
            var task = await tasksDbContext.Employees.ToListAsync();
            if (task.Count == 0)
            {
                return new GetEmpResponse
                {
                    Success = false,
                    Error = "No tasks found for this user",
                    ErrorCode = "T04"
                };
            }

            return new GetEmpResponse { Success = true, Taskss = task };
        }

        //public async Task<SaveTaskResponse> SaveTask(Tasks task)
        //{
        //    await tasksDbContext.Taskss.AddAsync(task);
        //    var saveResponse = await tasksDbContext.SaveChangesAsync();

        //    if (saveResponse >= 0)
        //    {
        //        return new SaveTaskResponse
        //        {
        //            Success = true,
        //            Taskss = task
        //        };
        //    }

        //    return new SaveTaskResponse
        //    {
        //        Success = false,
        //        Error = "Unable to save task",
        //        ErrorCode = "T05"
        //    };
        //}
    }
}
