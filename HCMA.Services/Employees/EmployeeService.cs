using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public EmployeeService(Context ctx, IMapper mapper)
        {
            this.context = ctx;
            this.mapper = mapper;
        }
        private readonly Context context;
        private readonly IMapper mapper;
        public async Task CreateAsync(string firstName, string lastName, string phone, string email,Gender gender, DateTime startDate, string image, decimal salary,
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
                Gender=gender,
                StartDate = startDate,
                Salary = salary,
                Username = username,
                Password = password,
                Image = image,
                Department = department,
                DepartmentId = department.Id,
                Role = role,
                RoleId = role.Id
            };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await this.context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            this.context.Employees.Remove(e);
            await this.context.SaveChangesAsync();
        } 
        public int EditAsync(EmployeeServiceModel employeeServiceModel,int id)
        {
            var e =  context.Employees.SingleOrDefault(x => x.Id == id);
            var oldImg = e.Image;
            e=mapper.Map<EmployeeServiceModel, Employee>(employeeServiceModel); 
            e.Gender = employeeServiceModel.GenderType;
            if (e.Image == null)
                e.Image = oldImg;
            context.ChangeTracker.Clear();
            context.Employees.Update(e);
            context.SaveChanges();

            return e.Id;
        }

        public async Task<IEnumerable<TModel>> GetAllAsync<TModel>(string search = null)
        {
            var queryable=this.context.Employees.AsNoTracking();
            var filterString = search ?? "all-all";
            string[] filters=null;
            string position=string.Empty;
            string role = string.Empty;
            if(filterString.Contains('-'))
            {
                 filters = filterString.Split('-');
                 position = filters[0];
                 role = filters[1]; 
            }
            if (!String.IsNullOrWhiteSpace(search))
            {
                queryable = queryable.Where(x => x.Username.Contains(search) ||
                x.FirstName.Contains(search) || x.LastName.Contains(search) ||( x.Department.Name==position && x.Role.Name==role));
            }
            var employees = await queryable.ProjectTo<TModel>(this.mapper.ConfigurationProvider).ToListAsync();
            return employees;
        }

        public async Task<TModel> GetByIdAsync<TModel>(int id)
        {
            return await context.Employees.AsNoTracking()
                .Where(x => x.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public int Login(string username, string password)
        {
            var user = context.Employees.FirstOrDefault(x => x.Username == username && x.Password == password);
            if (user != null)
            {
                AccountService.UsrId = user.Id;
                AccountService.Username = user.Username;
                AccountService.Role = context.Roles.FirstOrDefault(x=>x.Id==user.RoleId).Name;
                return user.Id;
            }
            else return -1;
        }

        public bool Logout()
        {
            if(string.IsNullOrEmpty(AccountService.Username)==false)
            {
                AccountService.Username = null;
                AccountService.UsrId = null;
                return true;
            }
            else return false;
        }
    }
}
