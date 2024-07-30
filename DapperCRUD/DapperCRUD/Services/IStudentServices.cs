using DapperCRUD.Models;

namespace DapperCRUD.Services
{
    public interface IStudentServices
    {
        public List<Students> GetStudentsList();
        public string InsertStudent(Students students);
        public string UpdateStudent(Students students);
        public string DeleteStudent(int StudentId);
        public Students GetStudentListbyId(int? StudentId);

        
    }
}
