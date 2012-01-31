using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> t = new List<string>();
            t.Add("a");
            t.Add("b");
            t.Add("c");
            t.Add("d");
            t.Add("ab");

            foreach (string current in t)
            {
                Console.WriteLine("Content:{0},Index:{1}",current,t.IndexOf(current));
            }
            Console.Read();
        }
    }
}
