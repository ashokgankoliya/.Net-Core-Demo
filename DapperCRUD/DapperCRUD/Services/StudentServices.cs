using Dapper;
using DapperCRUD.Models;
using System.Data;
using System.Data.SqlClient;

namespace DapperCRUD.Services
{
    public class StudentServices : IStudentServices
    {
        private readonly IConfiguration _configuration;

        public StudentServices(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DBConnectionofDapperDB");
            providerName = "System.Data.SqlClient";
        }

        public string ConnectionString { get; }
        public string providerName { get; }

        public IDbConnection Connection
        {
            get { return new SqlConnection(ConnectionString); }
        }

        public List<Students> GetStudentsList()
        {
            List<Students> students = new List<Students>();
            try
            {
                using (IDbConnection dbconnection = Connection)
                {
                    dbconnection.Open();
                    students = dbconnection.Query<Students>("GetStudentList", commandType: CommandType.StoredProcedure).ToList();
                    dbconnection.Close();
                    return students;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                return students;
            }
        }

        public string InsertStudent(Students students)
        {
            string result = "";
            try
            {
                using (IDbConnection dbconnection = Connection)
                {
                    dbconnection.Open();
                    var std = dbconnection.Query<Students>("InsertStudent",
                        new { FullName = students.FullName, City = students.City, EmailAddress = students.EmailAddress, CreateBy = students.CreateBy }, commandType: CommandType.StoredProcedure);
                    if (std != null && std.FirstOrDefault().Response == "Save Sucessfully")
                    {
                        result = "Save Sucessfully";
                    }
                    dbconnection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                return errorMsg;
            }
        }

        public string UpdateStudent(Students students)
        {
            string result = "";
            try
            {
                using (IDbConnection dbconnection = Connection)
                {
                    dbconnection.Open();
                    var std = dbconnection.Query<Students>("UpdateStudent",
                        new
                        {
                            FullName = students.FullName,
                            City = students.City,
                            EmailAddress = students.EmailAddress,
                            StudentId = students.StudentId
                        },
                        commandType: CommandType.StoredProcedure);
                    if (std != null && std.FirstOrDefault().Response == "Update Sucessfully")
                    {
                        result = "Update Sucessfully";
                    }
                    dbconnection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                return errorMsg;
            }
        }

        public string DeleteStudent(int StudentId)
        {
            string result = "";
            try
            {
                using (IDbConnection dbconnection = Connection)
                {
                    dbconnection.Open();
                    var std = dbconnection.Query<Students>("DeleteStudent",
                        new
                        {
                            StudentId = StudentId
                        },
                        commandType: CommandType.StoredProcedure);
                    if (std != null && std.FirstOrDefault().Response == "Delete Sucessfully")
                    {
                        result = "Delete Sucessfully";
                    }
                    dbconnection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                return errorMsg;
            }
        }
        
        public Students GetStudentListbyId(int? StudentId)
        {
            Students students = null;
            try
            {
                using (IDbConnection dbconnection = Connection)
                {
                    dbconnection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@StudentId", StudentId);
                    students = dbconnection.QuerySingleOrDefault<Students>("GetStudentListbyId", parameters, commandType: CommandType.StoredProcedure);
                    dbconnection.Close();
                    return students;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                return students;
            }
        }
    }
}
