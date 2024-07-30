using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksApi7.Service;
using TasksApi7.Model;
using TasksApi7.Entities;

namespace TasksApi7.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BaseApiController
    {
        private readonly ITasksInterface taskService;
        public TasksController(ITasksInterface taskService)
        {
            this.taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getTasksResponse = await taskService.GetTasks(UserID);
            if (!getTasksResponse.Success)
            {
                return UnprocessableEntity(getTasksResponse);
            }
            var tasksResponse = getTasksResponse.Taskss.ConvertAll(o => new TaskResponse { Id = o.Id, IsCompleted = o.IsCompleted, Name = o.Name, Ts = o.Ts });
            return Ok(tasksResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Post(TaskRequest taskRequest)
        {
            var task = new Tasks { IsCompleted = taskRequest.IsCompleted, Ts = taskRequest.Ts, Name = taskRequest.Name, UserId = UserID };
            var saveTaskResponse = await taskService.SaveTask(task);
            if (!saveTaskResponse.Success)
            {
                return UnprocessableEntity(saveTaskResponse);
            }
            var taskResponse = new TaskResponse { Id = saveTaskResponse.Taskss.Id, IsCompleted = saveTaskResponse.Taskss.IsCompleted, Name = saveTaskResponse.Taskss.Name, Ts = saveTaskResponse.Taskss.Ts };
            return Ok(taskResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteTaskResponse = await taskService.DeleteTask(id, UserID);
            if (!deleteTaskResponse.Success)
            {
                return UnprocessableEntity(deleteTaskResponse);
            }

            return Ok(deleteTaskResponse.TaskId);
        }
    }
}
