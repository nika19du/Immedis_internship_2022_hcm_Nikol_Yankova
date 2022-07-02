using HCMA.Services.Employees.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMA.Services.Employees
{
    public interface IEmployeeService
    {
        Task CreateAsync(string FirstName,string LastName,string Phone,string Email,DateTime startDate,string Image,decimal salary,string username,string password,int departmentId,int roleId );

        int EditAsync(EmployeeServiceModel employeeServiceModel,int id);

        Task<TModel> GetByIdAsync<TModel>(int id);
        Task DeleteAsync(int id);
        Task<IEnumerable<TModel>> GetAllAsync<TModel>(string search = null);
        int Login(string username, string password);
        bool Logout();
    }
}
