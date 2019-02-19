using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.CSharp.CommonImpl;

namespace Sudoku.CSharp.PredictorImpl
{
    internal class PredictContextFactory
    {
        public PredictContext Create(Grid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            CellsInfo predictObject = FindPredictObject(grid);
            Tuple<Int32, NumbersBinary> numberData = CreateNumberData(predictObject.Data.First().Value);
            foreach (GridPoint cell in predictObject.Data.Keys.ToArray())
                predictObject.Data[cell] = predictObject.Data[cell].AppendRow(grid, cell.Row).AppendColumn(grid, cell.Column).AppendSquare(grid, cell);
            Queue<GridPoint> cells = new Queue<GridPoint>(predictObject.Data.Where(info => (info.Value & numberData.Item2) != 0).Select(info => info.Key));
            return new PredictContext(cells, numberData.Item1, grid);
        }

        private CellsInfo FindPredictObject(Grid grid)
        {
            CellsInfo rowPredictObject = Enumerable.Range(1, Defs.GridSide)
                .Select(grid.ScanRow)
                .Where(cellsInfo => cellsInfo.Data.Count > 0)
                .Aggregate((left, right) => left.Data.Count <= right.Data.Count ? left : right);
            CellsInfo columnPredictObject = Enumerable.Range(1, Defs.GridSide)
                .Select(grid.ScanColumn)
                .Where(cellsInfo => cellsInfo.Data.Count > 0)
                .Aggregate(rowPredictObject, (left, right) => left.Data.Count <= right.Data.Count ? left : right);
            return Defs.Squares
                .Select(grid.ScanSquare)
                .Where(cellsInfo => cellsInfo.Data.Count > 0)
                .Aggregate(columnPredictObject, (left, right) => left.Data.Count <= right.Data.Count ? left : right);
        }

        private Tuple<Int32, NumbersBinary> CreateNumberData(NumbersBinary numbersBinary)
        {
            for (Int32 number = Defs.MinNumber; number <= Defs.MaxNumber; ++number)
            {
                NumbersBinary singleNumberBinary = NumbersBinaryHelper.CreateForNumber(number);
                if ((numbersBinary & singleNumberBinary) != 0)
                    return new Tuple<Int32, NumbersBinary>(number, singleNumberBinary);
            }
            throw new InvalidOperationException();
        }
    }
}