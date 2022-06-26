using Data;
using Data.Models;
using HCMA.Services.Employees.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMA.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        public EmployeeService(Context ctx)
        {
            this.context = ctx;
        }
        private readonly Context context;
        public async Task CreateAsync(string firstName, string lastName, string phone, string email, DateTime startDate, string Image, decimal salary, string image,
            string username, string password, int departmentId, int roleId)
        {
            var department = await this.context.Departments.FirstOrDefaultAsync(x => x.Id == departmentId);
            var role = await this.context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

            Employee employee = new Employee()
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Email = email,
                StartDate = startDate,
                Salary = salary,
                Username = username,
                Password = password,
                Image = image,
                Department = department,
                DepartmentId = department.Id,
                Role = role,
                RoleId = role.Id,
            };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public string EditAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TModel>> GetAllAsync<TModel>(string search = null)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetByIdAsync<TModel>(EmployeeServiceModel employeeServiceModel, int id)
        {
            throw new NotImplementedException();
        }
    }
}
