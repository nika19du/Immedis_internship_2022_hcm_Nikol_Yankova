using AutoMapper;
using Data.Models;
using HCMA.ViewModels.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App
{
    public class AppProfile:Profile
    {
        public AppProfile()
        {
            #region Employees
            this.CreateMap<Employee, EmployeesInfoViewModel>();
            this.CreateMap<EmployeesInfoViewModel, EmployeesAllViewModel>().ReverseMap();
            this.CreateMap<Employee, Employee>();
            #endregion
        }
    }
}
