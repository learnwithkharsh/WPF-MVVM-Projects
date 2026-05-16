using SQLite;
using System.IO;
using WpfApp.Interfaces;

namespace WpfApp.CURD
{
    public class EmployeeSqliteRepository : IRepository<Employee>
    {
        private static EmployeeSqliteRepository _instance;
        private static readonly object _lock = new();
        SQLiteAsyncConnection connection;
        public static EmployeeSqliteRepository Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new EmployeeSqliteRepository();
                }
            }
        }

        private EmployeeSqliteRepository()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employee.db");

            connection = new SQLiteAsyncConnection(dbPath);
            connection.CreateTableAsync<EmployeeSqlite>();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {            
            return await connection.FindAsync<EmployeeSqlite>(id);
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            var data = await connection.Table<EmployeeSqlite>().ToListAsync();
            return data.Cast<Employee>().ToList();
        }

        public async Task AddAsync(Employee emp)
        {
            var sqliteEmployee = ToSqlite(emp);

            await connection.InsertAsync(sqliteEmployee);
        }

        public async Task AddBulkAsync(List<Employee> employees)
        {
            var sqliteList = employees.Select(ToSqlite).ToList();
            await connection.InsertAllAsync(sqliteList);
          
        }

        public async Task UpdateAsync(Employee emp)
        {
            var sqliteEmployee = ToSqlite(emp);

             await connection.UpdateAsync(sqliteEmployee);
        }

        public async Task DeleteAsync(int id)
        {
            await connection.DeleteAsync<EmployeeSqlite>(id);
        }

        public async Task<(List<Employee> employee, int totalEmployee, int skip)> GetPagedAsync(int page, int pageSize)
        {
            var query = connection.Table<EmployeeSqlite>();

            var totalCount = await query.CountAsync();

            int skip;

            if (page * pageSize > totalCount)
            {
                skip = Math.Max(0, totalCount - pageSize);
            }
            else
            {
                skip = (page - 1) * pageSize;
            }

            var data = await query.Skip(skip)
                                  .Take(pageSize)
                                  .ToListAsync();

            return (
                employee: data.Select(x => (Employee)x).ToList(),
                totalEmployee: totalCount,
                skip: skip
            );
        }

        private EmployeeSqlite ToSqlite(Employee employee)
        {
            return new EmployeeSqlite
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Gender = employee.Gender,
                Company = employee.Company,
                Role = employee.Role,
                Country = employee.Country,
                Experience = employee.Experience
            };
        }


        /*   public async Task<Employee> GetByIdAsync(int id)
           {
               return await connection.FindAsync<Employee>(id);
           }
           public async Task<List<Employee>> GetAllAsync()
           {           
               return await connection.Table<Employee>().ToListAsync();
           }

           public async Task AddAsync(Employee emp)
           {
               await connection.InsertAsync(emp);
           }
           public async Task AddBulkAsync(List<Employee> employees)
           {
               await connection.InsertAllAsync(employees);
           }

           public async Task UpdateAsync(Employee emp)
           {
               await connection.UpdateAsync(emp);
           }

           public async Task DeleteAsync(int id)
           {
               await connection.DeleteAsync(id);
           }

           public async Task<(List<Employee> employee, int totalEmployee, int skip)> GetPagedAsync(int page, int pageSize)
           {
               var query = await GetAllAsync();

               var totalCount = query.Count;

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

               var data = query
                   .Skip(skip)
                   .Take(pageSize)
                   .ToList();

               return (employee: data, totalEmployee: totalCount, skip: skip);
           }
        */

    }
}
