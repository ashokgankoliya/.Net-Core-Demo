using DapperCRUD.Models;
using DapperCRUD.Services;
using DapperCRUD.ViewModel;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace DapperCRUD.Controllers
{
    public class StudentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IStudentServices _studentServices;

        public StudentController(IConfiguration configuration, IStudentServices studentServices)
        {
            _configuration = configuration;
            _studentServices = studentServices;
        }

        public IActionResult Index()
        {
            StudentViewModel model = new StudentViewModel();
            model.StudentsList = _studentServices.GetStudentsList().ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult InsertUpdate(int? Id)
        {
            Students model = _studentServices.GetStudentListbyId(Id);
            return View(model);
        }
        [HttpPost]
        public IActionResult InsertUpdate(Students std)
        {
            StudentViewModel vm = new StudentViewModel();
            std.CreateBy = "1";
            string result = string.Empty;
            if (std.StudentId > 0)
            {
                result = _studentServices.UpdateStudent(std);
            }
            else
            {
                result = _studentServices.InsertStudent(std);
            }
            if (result == "Save Sucessfully")
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult DeleteStudent(int Id)
        {
            string result = _studentServices.DeleteStudent(Id);
            if (result == "Delete Sucessfully")
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Json(new { Message = "Error" });
            }
        }
    }
}
