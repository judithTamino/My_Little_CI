using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Calc calc = new Calc();
            string print = calc.Check(7, 9).ToString();
            Console.WriteLine(print);
        }


    }
}
