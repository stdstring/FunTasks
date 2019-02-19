using System;

namespace Sudoku.CSharp.CalculatorImpl
{
    internal class CalculationContext
    {
        public CalculationContext(Grid grid, Int32 emptyCount)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (emptyCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(emptyCount));
            Grid = grid;
            EmptyCount = emptyCount;
        }

        public Grid Grid { get; }

        public Int32 EmptyCount { get; set; }
    }
}