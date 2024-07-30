using APIProject.Model;
using APIProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace APIProject.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BaseApiController
    {
        private readonly IEmpInterface empService;
        public TasksController(IEmpInterface empService)
        {
            this.empService = empService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getTasksResponse = await empService.GetTasks();
            if (!getTasksResponse.Success)
            {
                return UnprocessableEntity(getTasksResponse);
            }
            var tasksResponse = getTasksResponse.Taskss.ConvertAll(o => new TaskResponse { Id = o.Id, Name = o.Name});
            return Ok(tasksResponse);
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(TaskRequest taskRequest)
        //{
        //    var task = new Tasks { IsCompleted = taskRequest.IsCompleted, Ts = taskRequest.Ts, Name = taskRequest.Name, UserId = UserID };
        //    var saveTaskResponse = await taskService.SaveTask(task);
        //    if (!saveTaskResponse.Success)
        //    {
        //        return UnprocessableEntity(saveTaskResponse);
        //    }
        //    var taskResponse = new TaskResponse { Id = saveTaskResponse.Taskss.Id, IsCompleted = saveTaskResponse.Taskss.IsCompleted, Name = saveTaskResponse.Taskss.Name, Ts = saveTaskResponse.Taskss.Ts };
        //    return Ok(taskResponse);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var deleteTaskResponse = await taskService.DeleteTask(id, UserID);
        //    if (!deleteTaskResponse.Success)
        //    {
        //        return UnprocessableEntity(deleteTaskResponse);
        //    }

        //    return Ok(deleteTaskResponse.TaskId);
        //}
    }
}
