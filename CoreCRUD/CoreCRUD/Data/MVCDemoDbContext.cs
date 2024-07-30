﻿//Ashok
using CoreCRUD.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CoreCRUD.Data
{
   
    public class MVCDemoDbContext : DbContext
    {
        public MVCDemoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }


    }
}
