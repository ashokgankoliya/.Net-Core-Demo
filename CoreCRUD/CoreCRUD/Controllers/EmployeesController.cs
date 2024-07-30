using CoreCRUD.Data;
using CoreCRUD.Models;
using CoreCRUD.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreCRUD.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly MVCDemoDbContext mvcDemoDbContext;
        public EmployeesController(MVCDemoDbContext mvcDemoDbContext)
        {
            this.mvcDemoDbContext = mvcDemoDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //var employees = await mvcDemoDbContext.Employees.ToListAsync();

            //1 Using murge model
            //Create View Model empvm all table list
            //empvm.emp=context.employee.tolist
            //empvm.dept=context.Department.tolist


            //2 Direct Query use 
            AddEmployeeViewModel emp = new AddEmployeeViewModel();
            var empdata = mvcDemoDbContext.Employees.FromSqlRaw("select * from Employees").ToList();

            //Add data in view model with list (1) model murge 
            emp.employees = empdata;


            //3 Using Join Model
            //var data =(from e in context.emp
            //join d in context.department
            //on e.depid equals d.id
            //select new create new viewmodel with property only
            //{
            //  empid=e.empid,
            //  name=e.name,
            //  departname=d.name
            //}).tolist();

            //4 model 3 with property only
            //need to add DBSet model with [Notmapped] under this keyword
            //var data=context.viewmodel 3 .fromsqlraw(join query)

            //5 using procedure 
            // 1 type way diffent sp use
            // 2 using join 
            //context. model 3 .fromsqlraw (exec get_data)

            return View(emp);
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee EmpVM)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = EmpVM.Name,
                Email = EmpVM.Email,
                Salary = EmpVM.Salary,
                Department = EmpVM.Department,
                DateOfBirth = EmpVM.DateOfBirth
            };
            await mvcDemoDbContext.Employees.AddAsync(employee);
            await mvcDemoDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var employee = await mvcDemoDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    Department = employee.Department,
                    DateOfBirth = employee.DateOfBirth
                };
                return await Task.Run(() => View("View",viewModel));
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel model)
        {
            var employee = await mvcDemoDbContext.Employees.FindAsync(model.Id);
            if (employee != null)
            {
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Salary = model.Salary;
                employee.Department = model.Department;
                employee.DateOfBirth = model.DateOfBirth;
                await mvcDemoDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel model)
        {
            var employe = await mvcDemoDbContext.Employees.FindAsync(model.Id);
            if(employe != null)
            {
                mvcDemoDbContext.Employees.Remove(employe);
                await mvcDemoDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
