using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Evaluate(true, false, false, false));
        }

        static int Evaluate(params bool[] parameters)
        {
            int x = 0;
            if (parameters[0])
            {
                x = 5;
            }
            if (parameters[0])
            {
                x = 6;
                if (parameters[0])
                {
                    x = 7;
                }
            }
            return x;
        }
    }
}
