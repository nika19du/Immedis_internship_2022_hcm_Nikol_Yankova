using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMA.Services.Employees.Model
{
    public class EmployeeServiceModel
    { 
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public int DepartmentId { get; set; }
        public int RoleId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
    }
}
