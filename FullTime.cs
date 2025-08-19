using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Group_Final_Project
{
    public class FullTime : Employee
    {
        public decimal Wage { get; set; }
        public override decimal GetPay(decimal hours = 1)
        {
            return Wage * hours;
        }
    }
}
