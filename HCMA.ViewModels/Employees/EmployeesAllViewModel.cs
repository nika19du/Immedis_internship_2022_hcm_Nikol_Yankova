using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMA.ViewModels.Employees
{
   public class EmployeesAllViewModel
    {
        public string Search { get; set; }
        public IEnumerable<EmployeesInfoViewModel> Employees { get; set; }
    }
}
