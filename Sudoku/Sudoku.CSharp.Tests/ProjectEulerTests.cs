using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Sudoku.CSharp.Tests.Utils;

namespace Sudoku.CSharp.Tests
{
    [TestFixture]
    public class ProjectEulerTests
    {
        [Test]
        public void Problem096()
        {
            const String inputFilename = "TestCases\\projecteuler\\problem_096.in";
            const Int32 expectedValue = 24702;
            IList<Tuple<String, Grid>> source = ReadInput(inputFilename);
            IList<Grid> dest = new List<Grid>();
            Solver solver = new Solver();
            foreach (Tuple<String, Grid> item in source)
            {
                Console.WriteLine($"Processing of the \"{item.Item1}\" case");
                Grid result = solver.Solve(item.Item2);
                dest.Add(result);
            }
            Int32 actualValue = dest.Aggregate(0, (sum, grid) => sum + 100 * grid[1, 1] + 10 * grid[1, 2] + grid[1, 3]);
            Console.WriteLine($"Result = {actualValue}");
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        private IList<Tuple<String, Grid>> ReadInput(String filename)
        {
            IList<Tuple<String, Grid>> dest = new List<Tuple<String, Grid>>();
            String[] lines = File.ReadAllLines(filename);
            for (Int32 index = 0; index < lines.Length; index += (1 + Defs.GridSide))
            {
                String name = lines[index];
                String[] gridLines = new String[Defs.GridSide];
                Array.Copy(lines, index + 1, gridLines, 0, Defs.GridSide);
                Grid grid = GridConverter.Convert(gridLines);
                dest.Add(new Tuple<String, Grid>(name, grid));
            }
            return dest;
        }
    }
}
