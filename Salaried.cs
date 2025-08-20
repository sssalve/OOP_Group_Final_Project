using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Group_Final_Project
{
    public class Salaried : Employee
    {
        public decimal Salary { get; set; }

        public override decimal GetPay(decimal weeks = 54)
        {
            return (Salary / 54) * weeks;
        }
    }
}