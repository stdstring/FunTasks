using System;
using System.Linq;

namespace Sudoku.CSharp.CommonImpl
{
    internal static class NumbersBinaryHelper
    {
        public static NumbersBinary CreateForNumber(Int32 number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));
            return new NumbersBinary(1 << (number - 1));
        }

        public static NumbersBinary CreateForRange(Int32 minNumber, Int32 maxNumber)
        {
            if (minNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(minNumber));
            if (maxNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(maxNumber));
            if (maxNumber <= minNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber));
            return Enumerable.Range(minNumber, maxNumber - minNumber + 1)
                             .Select(CreateForNumber)
                             .Aggregate(new NumbersBinary(0), (acc, value) => acc | value);
        }

        public static NumbersBinary UseNumber(this NumbersBinary numbers, Int32 number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));
            return numbers & ~(1 << (number - 1));
        }

        public static NumbersBinary AppendRow(this NumbersBinary numbers, Grid grid, Int32 row)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (row < 1 || row > grid.RowCount)
                throw new ArgumentOutOfRangeException(nameof(row));
            return AppendCells(numbers, grid, GridPointGenerator.GenerateRow(row));
        }

        public static NumbersBinary AppendColumn(this NumbersBinary numbers, Grid grid, Int32 column)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (column < 1 || column > grid.ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(column));
            return AppendCells(numbers, grid, GridPointGenerator.GenerateColumn(column));
        }

        public static NumbersBinary AppendSquare(this NumbersBinary numbers, Grid grid, GridPoint point)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (point.Row > grid.RowCount || point.Column > grid.ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(point));
            return AppendCells(numbers, grid, GridPointGenerator.GenerateSquare(point));
        }

        private static NumbersBinary AppendCells(NumbersBinary numbers, Grid grid, GridPoint[] cells)
        {
            return cells.Select(cell => grid[cell]).Where(number => number != 0).Aggregate(numbers, UseNumber);
        }
    }
}