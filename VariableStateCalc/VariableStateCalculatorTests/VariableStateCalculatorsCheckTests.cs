using System;
using NUnit.Framework;
using VariableStateCalculator.Antlr;
using VariableStateCalculator.Roslyn;

namespace VariableStateCalculatorTests
{
    [TestFixture]
    public class VariableStateCalculatorsCheckTests
    {
        [TestCase(Example1, new[] {1, 3, 99})]
        [TestCase(Example2, new[] {666})]
        [TestCase(Example3, new[] {1, 3, 99})]
        [TestCase(Example4, new[] {0, 6})]
        [TestCase(Example5, new[] {0, 6})]
        [TestCase(Example6, new[] {666})]
        [TestCase(Example7, new[] {0, 6})]
        [TestCase(Example8, new[] {0, 5, 6})]
        [TestCase(TaskExample1, new[] {1, 4, 5, 6})]
        [TestCase(TaskExample2, new[] {1, 4, 5, 6})]
        [TestCase(TaskExample3, new[] {0, 6})]
        [TestCase(TaskExample4, new[] {0, 99})]
        [TestCase(TaskExample5, new[] {0, 7})]
        public void CheckVariableStateAntlrCalculator(String source, Int32[] expectedStates)
        {
            VariableStateAntlrCalculator calculator = new VariableStateAntlrCalculator();
            Assert.AreEqual(expectedStates, calculator.Calculate(source));
        }

        [TestCase(Example1, new[] {1, 3, 99})]
        [TestCase(Example2, new[] {666})]
        [TestCase(Example3, new[] {1, 3, 99})]
        [TestCase(Example4, new[] {0, 6})]
        [TestCase(Example5, new[] {0, 6})]
        [TestCase(Example6, new[] {666})]
        [TestCase(Example7, new[] {0, 6})]
        [TestCase(Example8, new[] {0, 5, 6})]
        [TestCase(TaskExample1, new[] {1, 4, 5, 6})]
        [TestCase(TaskExample2, new[] {1, 4, 5, 6})]
        [TestCase(TaskExample3, new[] {0, 6})]
        [TestCase(TaskExample4, new[] {0, 99})]
        [TestCase(TaskExample5, new[] {0, 7})]
        public void CheckVariableStateRoslynDirectCalculator(String source, Int32[] expectedStates)
        {
            VariableStateRoslynDirectCalculator calculator = new VariableStateRoslynDirectCalculator();
            Assert.AreEqual(expectedStates, calculator.Calculate(source));
        }

        [TestCase(Example1, new[] {1, 3, 99})]
        [TestCase(Example2, new[] {666})]
        [TestCase(Example3, new[] {1, 3, 99})]
        [TestCase(Example4, new[] {0, 6})]
        [TestCase(Example5, new[] {0, 6})]
        [TestCase(Example6, new[] {666})]
        [TestCase(Example7, new[] {0, 6})]
        [TestCase(Example8, new[] {0, 5, 6})]
        [TestCase(TaskExample1, new[] {1, 4, 5, 6})]
        [TestCase(TaskExample2, new[] {1, 4, 5, 6})]
        [TestCase(TaskExample3, new[] {0, 6})]
        [TestCase(TaskExample4, new[] {0, 99})]
        [TestCase(TaskExample5, new[] {0, 7})]
        public void CheckVariableStateRoslynWalkerCalculator(String source, Int32[] expectedStates)
        {
            VariableStateRoslynWalkerCalculator calculator = new VariableStateRoslynWalkerCalculator();
            Assert.AreEqual(expectedStates, calculator.Calculate(source));
        }

        private const String Example1 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x;\r\n" +
                                        "            x = 1;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "                x = 2;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                x = 3;\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                    x = 99;\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String Example2 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x;\r\n" +
                                        "            x = 1;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "                x = 2;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                x = 3;\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                    x = 99;\r\n" +
                                        "            {\r\n" +
                                        "                x = 666;\r\n" +
                                        "            }\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String Example3 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x = 1;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "                x = 2;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                x = 3;\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                    x = 99;\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String Example4 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x = 0;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    x = 5;\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    x = 6;\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String Example5 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x = 0;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    x = 5;\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[1])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[0])\r\n" +
                                        "                {\r\n" +
                                        "                    x = 6;\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String Example6 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x = 0;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    x = 5;\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    x = 6;\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            x = 666;\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String Example7 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x = 0;\r\n" +
                                        "            if (parameters[3])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[0])\r\n" +
                                        "                {\r\n" +
                                        "                    if (parameters[1])\r\n" +
                                        "                    {\r\n" +
                                        "                        x = 5;\r\n" +
                                        "                    }\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    x = 6;\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String Example8 = "using System;\r\n" +
                                        "namespace ConsoleApp\r\n" +
                                        "{\r\n" +
                                        "    class Program\r\n" +
                                        "    {\r\n" +
                                        "        static void Main()\r\n" +
                                        "        {\r\n" +
                                        "        }\r\n" +
                                        "        static int Evaluate(params bool[] parameters)\r\n" +
                                        "        {\r\n" +
                                        "            int x = 0;\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    if (parameters[2])\r\n" +
                                        "                    {\r\n" +
                                        "                        x = 5;\r\n" +
                                        "                    }\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            if (parameters[0])\r\n" +
                                        "            {\r\n" +
                                        "                if (parameters[1])\r\n" +
                                        "                {\r\n" +
                                        "                    if (parameters[3])\r\n" +
                                        "                    {\r\n" +
                                        "                        x = 6;\r\n" +
                                        "                    }\r\n" +
                                        "                }\r\n" +
                                        "            }\r\n" +
                                        "            return x;\r\n" +
                                        "        }\r\n" +
                                        "    }\r\n" +
                                        "}";

        private const String TaskExample1 = "using System;\r\n" +
                                            "namespace ConsoleApp\r\n" +
                                            "{\r\n" +
                                            "    class Program\r\n" +
                                            "    {\r\n" +
                                            "        static void Main()\r\n" +
                                            "        {\r\n" +
                                            "            // выведет 1\r\n" +
                                            "            Console.WriteLine(Evaluate(false, false, false, false));\r\n" +
                                            "            // выведет 6\r\n" +
                                            "            Console.WriteLine(Evaluate(false, false, false, true));\r\n" +
                                            "            //...\r\n" +
                                            "        }\r\n" +
                                            "        static int Evaluate(params bool[] parameters)\r\n" +
                                            "        {\r\n" +
                                            "            int x;\r\n" +
                                            "            x = 1;\r\n" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 2;\r\n" +
                                            "                if (parameters[1])\r\n" +
                                            "                {\r\n" +
                                            "                    x = 3;\r\n" +
                                            "                }\r\n" +
                                            "                x = 4;\r\n" +
                                            "                if (parameters[2])" +
                                            "                {\r\n" +
                                            "                    x = 5;\r\n" +
                                            "                }\r\n" +
                                            "            }\r\n" +
                                            "            if (parameters[3])\r\n" +
                                            "            {\r\n" +
                                            "                x = 6;\r\n" +
                                            "            }\r\n" +
                                            "            return x;\r\n" +
                                            "        }\r\n" +
                                            "    }\r\n" +
                                            "}";

        private const String TaskExample2 = "using System;\r\n" +
                                            "namespace ConsoleApp\r\n" +
                                            "{\r\n" +
                                            "    class Program\r\n" +
                                            "    {\r\n" +
                                            "        static void Main()\r\n" +
                                            "        {\r\n" +
                                            "            // выведет 1\r\n" +
                                            "            Console.WriteLine(Evaluate(false, false, false, false));\r\n" +
                                            "            // выведет 6\r\n" +
                                            "            Console.WriteLine(Evaluate(false, false, false, true));\r\n" +
                                            "            //...\r\n" +
                                            "        }\r\n" +
                                            "        static int Evaluate(params bool[] parameters)\r\n" +
                                            "        {\r\n" +
                                            "            int x;\r\n" +
                                            "            x = 1\r\n;" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 2;\r\n" +
                                            "                if (parameters[1])\r\n" +
                                            "                {\r\n" +
                                            "                    x = 3;\r\n" +
                                            "                }\r\n" +
                                            "                x = 4;\r\n" +
                                            "                if (parameters[2])\r\n" +
                                            "                {\r\n" +
                                            "                    if (parameters[3])\r\n" +
                                            "                    {\r\n" +
                                            "                        x = 6;\r\n" +
                                            "                    }\r\n" +
                                            "                    if (parameters[4])\r\n" +
                                            "                    {\r\n" +
                                            "                        x = 5;\r\n" +
                                            "                    }\r\n" +
                                            "                }\r\n" +
                                            "            }\r\n" +
                                            "            return x;\r\n" +
                                            "        }\r\n" +
                                            "    }\r\n" +
                                            "}";

        private const String TaskExample3 = "using System;\r\n" +
                                            "namespace ConsoleApp\r\n" +
                                            "{\r\n" +
                                            "    class Program\r\n" +
                                            "    {\r\n" +
                                            "        static void Main()\r\n" +
                                            "        {\r\n" +
                                            "        }\r\n" +
                                            "        static int Evaluate(params bool[] parameters)\r\n" +
                                            "        {\r\n" +
                                            "            int x = 0;\r\n" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 5;\r\n" +
                                            "            }\r\n" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 6;\r\n" +
                                            "            }\r\n" +
                                            "            return x;\r\n" +
                                            "        }\r\n" +
                                            "    }\r\n" +
                                            "}";

        private const String TaskExample4 = "using System;\r\n" +
                                            "namespace ConsoleApp\r\n" +
                                            "{\r\n" +
                                            "    class Program\r\n" +
                                            "    {\r\n" +
                                            "        static void Main()\r\n" +
                                            "        {\r\n" +
                                            "        }\r\n" +
                                            "        static int Evaluate(params bool[] parameters)\r\n" +
                                            "        {\r\n" +
                                            "            int x = 0;\r\n" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 5;\r\n" +
                                            "            }\r\n" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 9;\r\n" +
                                            "                if (parameters[1])\r\n" +
                                            "                {\r\n" +
                                            "                    x = 10;\r\n" +
                                            "                }\r\n" +
                                            "                if (parameters[1])\r\n" +
                                            "                {\r\n" +
                                            "                    x = 11;\r\n" +
                                            "                }\r\n" +
                                            "                x = 99;\r\n" +
                                            "            }\r\n" +
                                            "            return x;\r\n" +
                                            "        }\r\n" +
                                            "    }\r\n" +
                                            "}";

        private const String TaskExample5 = "using System;\r\n" +
                                            "namespace ConsoleApp\r\n" +
                                            "{\r\n" +
                                            "    class Program\r\n" +
                                            "    {\r\n" +
                                            "        static void Main(string[] args)\r\n" +
                                            "        {\r\n" +
                                            "            Console.WriteLine(Evaluate(true, false, false, false));\r\n" +
                                            "        }\r\n" +
                                            "        static int Evaluate(params bool[] parameters)\r\n" +
                                            "        {\r\n" +
                                            "            int x = 0;\r\n" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 5;\r\n" +
                                            "            }\r\n" +
                                            "            if (parameters[0])\r\n" +
                                            "            {\r\n" +
                                            "                x = 6;\r\n" +
                                            "                if (parameters[0])\r\n" +
                                            "                {\r\n" +
                                            "                    x = 7;\r\n" +
                                            "                }\r\n" +
                                            "            }\r\n" +
                                            "            return x;\r\n" +
                                            "        }\r\n" +
                                            "    }\r\n" +
                                            "}";
    }
}