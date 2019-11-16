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
                x = 9;
                if(parameters[1]){
                    x = 10;
                }
                if(parameters[1]){
                    x = 11;
                }
                x = 99;
            }
            return x;
        }
    }
}