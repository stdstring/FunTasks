using System;
using System.Linq;

namespace Sudoku.CSharp.CommonImpl
{
    internal static class CellsInfoHelper
    {
        public static CellsInfo ScanRow(this Grid grid, Int32 row)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (row < 1 || row > grid.RowCount)
                throw new ArgumentOutOfRangeException(nameof(row));
            return ScanCells(grid, GridPointGenerator.GenerateRow(row));
        }

        public static CellsInfo ScanColumn(this Grid grid, Int32 column)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (column < 1 || column > grid.ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(column));
            return ScanCells(grid, GridPointGenerator.GenerateColumn(column));
        }

        public static CellsInfo ScanSquare(this Grid grid, GridPoint point)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (point.Row > grid.RowCount || point.Column > grid.ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(point));
            return ScanCells(grid, GridPointGenerator.GenerateSquare(point));
        }

        private static CellsInfo ScanCells(Grid grid, GridPoint[] cells)
        {
            CellsInfo cellsInfo = new CellsInfo();
            NumbersBinary allNumbers = NumbersBinaryHelper.CreateForRange(Defs.MinNumber, Defs.MaxNumber);
            NumbersBinary result = cells.Where(cell => grid[cell] != 0).Aggregate(allNumbers, (numbers, cell) => numbers.UseNumber(grid[cell]));
            foreach (GridPoint cell in cells.Where(cell => grid[cell] == 0))
                cellsInfo.Data[cell] = result;
            return cellsInfo;
        }
    }
}