using System;
using System.Collections.Generic;

namespace Sudoku.CSharp.PredictorImpl
{
    internal class PredictContext
    {
        public PredictContext(Queue<GridPoint> freeCells, Int32 number, Grid grid)
        {
            if (freeCells == null)
                throw new ArgumentNullException(nameof(freeCells));
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            FreeCells = freeCells;
            Number = number;
            Grid = grid;
        }

        public Queue<GridPoint> FreeCells { get; }

        public Int32 Number { get; }

        public Grid Grid { get; }
    }
}