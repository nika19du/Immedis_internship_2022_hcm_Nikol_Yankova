using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMA.ViewModels.Employees
{
    public class EmployeesInfoViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required] 
        public string Email { get; set; }
        public string Image { get; set; }
        [Required] 
        public DateTime Started { get; set; }
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required] 
        public string ConfirmPassword { get; set; }
        public int DepartmenId { get; set; } 
        public Department Department { get; set; }
        public int RoleId { get; set; } 
        public Role Role { get; set; } 
    }
}
