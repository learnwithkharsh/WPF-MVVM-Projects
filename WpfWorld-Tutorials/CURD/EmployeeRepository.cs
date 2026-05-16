using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using WpfApp.Interfaces;

namespace WpfApp.CURD
{
    public class EmployeeRepository : IRepository<Employee>
    {
        private static EmployeeRepository _instance;
        private static readonly object _lock = new();

        public static EmployeeRepository Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new EmployeeRepository();
                }
            }
        }

        private readonly EmployeeDbContext context;

        private EmployeeRepository()
        {
            context = new EmployeeDbContext();
        }
        public async Task<Employee> GetByIdAsync(int id)
        {
            return await context.Employees.FindAsync(id);
        }
        public async Task<List<Employee>> GetAllAsync()
        {
            return await context.Employees.ToListAsync<Employee>();
        }

        public async Task AddAsync(Employee emp)
        {
            await context.Employees.AddAsync(emp);
            await context.SaveChangesAsync();
        }
        public async Task AddBulkAsync(List<Employee> employees)
        {
            await context.BulkInsertAsync(employees);
        }

        public async Task UpdateAsync(Employee emp)
        {
            var employee = await GetByIdAsync(emp.Id);
            if (employee != null)
            {
                employee.Name = emp.Name;
                employee.Email = emp.Email;
                employee.Gender = emp.Gender;
                employee.Company = emp.Company;
                employee.Role = emp.Role;
                employee.Country = emp.Country;
                employee.Experience = emp.Experience;
                context.Employees.Update(employee);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await GetByIdAsync(id);
            if (employee != null)
            {
                context.Employees.Remove(employee);
                await context.SaveChangesAsync();
            }
        }

        public async Task<(List<Employee> employee, int totalEmployee,int skip)> GetPagedAsync(int page, int pageSize)
        {
            using var context = new EmployeeDbContext();

            var query = context.Employees.AsNoTracking();

            var totalCount = await query.CountAsync();

            int skip;

            // Fix for last page
            if (page * pageSize > totalCount)
            {
                skip = Math.Max(0, totalCount - pageSize);
            }
            else
            {
                skip = (page - 1) * pageSize;
            }

            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return (employee:data,totalEmployee: totalCount,skip:skip);
        }

    }
}
