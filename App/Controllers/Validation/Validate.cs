using Data;
using HCMA.InputModels.Employees;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Controllers.Validation
{
    public  class Validate
    {
        public Validate(Context ctx)
        {
            this.data = ctx;
        }
        private readonly Context data;
        public string CheckCreating(EmployeesCreateInputModel model)
        {
            if (data.Employees.Any(x=>x.Username==model.Username))
            { 
                return "Someone already has that username. Try another?"; 
            }
            if (model.Password.Length < 6 && model.Password.Any(char.IsUpper) == false && model.Password.Any(char.IsSymbol) == false)
            {
                return "Password must be more than 6 characters long, should contain at-least 1 Uppercase,1 Lowercase,1 Numeric and 1 special character.";
            }
            if(model.Salary<800)
            {
                return "Salary can't be less than 800lv.";
            }
            return null;
        }
    }
}
