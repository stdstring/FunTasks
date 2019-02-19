using System;

namespace Sudoku.CSharp
{
    public struct GridPoint
    {
        public GridPoint(Int32 row, Int32 column)
        {
            if (row < 1)
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 1)
                throw new ArgumentOutOfRangeException(nameof(column));
            Row = row;
            Column = column;
        }

        public Int32 Row { get; }

        public Int32 Column { get; }
    }
}