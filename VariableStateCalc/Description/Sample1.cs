using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            // выведет 1
            Console.WriteLine( Evaluate(false, false, false, false) );

            // выведет 6
            Console.WriteLine( Evaluate(false, false, false, true) );

            //...
        }

        static int Evaluate(params bool[] parameters)
        {
            int x;
            x = 1;
            if (parameters[0])
            {
                x = 2;
                if (parameters[1])
                {
                    x = 3;
                }
                x = 4;
                if (parameters[2])
                {
                    x = 5;
                }
            }
            if (parameters[3])
            {
                x = 6;
            }

            return x;
        }
    }
}
