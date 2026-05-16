using Microsoft.EntityFrameworkCore;

namespace WpfApp.CURD
{
    public class EmployeeDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Utilities.SelectedDatabase == "Sql Server")
                optionsBuilder.UseSqlServer(
                    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EmployeeDB;Integrated Security=True;" +
                    "Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;" +
                    "Multi Subnet Failover=False;Command Timeout=30");            
        }
    }
}
