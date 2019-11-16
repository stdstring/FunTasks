using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {

        }

        static int Evaluate(params bool[] parameters)
        {
            int x = 0;
            if(parameters[0]){
                x = 5;
            }
            if(parameters[0]){
                x = 6;
            }
            return x;
        }
    }
}