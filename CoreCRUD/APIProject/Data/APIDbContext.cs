//Ashok
using CoreCRUD.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace APIProject.Data
{
   
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }


    }
}
