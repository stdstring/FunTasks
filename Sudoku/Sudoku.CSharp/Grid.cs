using System;

namespace Sudoku.CSharp
{
    public class Grid
    {
        public Grid(Int32 rowCount, Int32 columnCount)
        {
            if (rowCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            if (columnCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(columnCount));
            RowCount = rowCount;
            ColumnCount = columnCount;
            _grid = new Int32[RowCount * ColumnCount];
        }

        public Int32 RowCount { get; }

        public Int32 ColumnCount { get; }

        public Int32 this[Int32 row, Int32 column]
        {
            get
            {
                if (row < 1 || row > RowCount)
                    throw new ArgumentOutOfRangeException(nameof(row));
                if (column < 1 || column > ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(column));
                Int32 index = (row - 1) * ColumnCount + (column - 1);
                return _grid[index];
            }
            set
            {
                if (row < 1 || row > RowCount)
                    throw new ArgumentOutOfRangeException(nameof(row));
                if (column < 1 || column > ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(column));
                Int32 index = (row - 1) * ColumnCount + (column - 1);
                _grid[index] = value;
            }
        }

        public Int32 this[GridPoint point]
        {
            get
            {
                if (point.Row > RowCount)
                    throw new ArgumentOutOfRangeException(nameof(point));
                if (point.Column > ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(point));
                Int32 index = (point.Row - 1) * ColumnCount + (point.Column - 1);
                return _grid[index];
            }
            set
            {
                if (point.Row > RowCount)
                    throw new ArgumentOutOfRangeException(nameof(point));
                if (point.Column > ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(point));
                Int32 index = (point.Row - 1) * ColumnCount + (point.Column - 1);
                _grid[index] = value;
            }
        }

        public Grid Clone()
        {
            Grid dest = new Grid(RowCount, ColumnCount);
            for (Int32 row = 1; row <= RowCount; ++row)
            for (Int32 column = 1; column <= ColumnCount; ++column)
                dest[row, column] = this[row, column];
            return dest;
        }

        private readonly Int32[] _grid;
    }
}