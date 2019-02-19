using System;
using System.Linq;

namespace Sudoku.CSharp.CommonImpl
{
    internal static class GridPointGenerator
    {
        public static GridPoint[] GenerateRow(Int32 row)
        {
            if (row < 1 || row > Defs.GridSide)
                throw new ArgumentOutOfRangeException(nameof(row));
            return Enumerable.Range(1, Defs.GridSide).Select(column => new GridPoint(row, column)).ToArray();
        }

        public static GridPoint[] GenerateColumn(Int32 column)
        {
            if (column < 1 || column > Defs.GridSide)
                throw new ArgumentOutOfRangeException(nameof(column));
            return Enumerable.Range(1, Defs.GridSide).Select(row => new GridPoint(row, column)).ToArray();
        }

        public static GridPoint[] GenerateSquare(GridPoint point)
        {
            if (point.Row > Defs.GridSide || point.Column > Defs.GridSide)
                throw new ArgumentOutOfRangeException(nameof(point));
            Int32 row = Defs.SquareSide * ((point.Row - 1) / Defs.SquareSide) + 1;
            Int32 column = Defs.SquareSide * ((point.Column - 1) / Defs.SquareSide) + 1;
            return new[]
            {
                new GridPoint(row, column), new GridPoint(row, column + 1), new GridPoint(row, column + 2),
                new GridPoint(row + 1, column), new GridPoint(row + 1, column + 1), new GridPoint(row + 1, column + 2),
                new GridPoint(row + 2, column), new GridPoint(row + 2, column + 1), new GridPoint(row + 2, column + 2),
            };
        }
    }
}