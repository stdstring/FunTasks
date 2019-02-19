using System;
using System.Linq;

namespace Sudoku.CSharp.CommonImpl
{
    internal class CellsInfoFactory
    {
        public CellsInfo CreateForRow(Grid grid, Int32 row)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (row < 1 || row > grid.RowCount)
                throw new ArgumentOutOfRangeException(nameof(row));
            CellsInfo cellsInfo = grid.ScanRow(row);
            foreach (GridPoint cell in cellsInfo.Data.Keys.ToArray())
            {
                NumbersBinary numbers = cellsInfo.Data[cell].AppendColumn(grid, cell.Column).AppendSquare(grid, cell);
                cellsInfo.Data[cell] = numbers;
            }
            return cellsInfo;
        }

        public CellsInfo CreateForColumn(Grid grid, Int32 column)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (column < 1 || column > grid.ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(column));
            CellsInfo cellsInfo = grid.ScanColumn(column);
            foreach (GridPoint cell in cellsInfo.Data.Keys.ToArray())
            {
                NumbersBinary number = cellsInfo.Data[cell].AppendRow(grid, cell.Row).AppendSquare(grid, cell);
                cellsInfo.Data[cell] = number;
            }
            return cellsInfo;
        }

        public CellsInfo CreateForSquare(Grid grid, GridPoint point)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (point.Row > grid.RowCount || point.Column > grid.ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(point));
            CellsInfo cellsInfo = grid.ScanSquare(point);
            foreach (GridPoint cell in cellsInfo.Data.Keys.ToArray())
            {
                NumbersBinary number = cellsInfo.Data[cell].AppendRow(grid, cell.Row).AppendColumn(grid, cell.Column);
                cellsInfo.Data[cell] = number;
            }
            return cellsInfo;
        }
    }
}