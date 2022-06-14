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
            string print = calc.Check(15, 15).ToString();
            Console.WriteLine(print);
        }


    }
}
