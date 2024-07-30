using CoreCRUD.Models.Domain;

namespace APIProject.Model
{
    public class GetEmpResponse : BaseResponse
    {
        public List<Employee> Taskss { get; set; }
    }
}
