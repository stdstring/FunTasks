using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.CSharp.CommonImpl;

namespace Sudoku.CSharp.CalculatorImpl
{
    internal enum CalculationContinuation { Continue, NeedPrediction, Finish, Stop }

    internal class Calculator
    {
        public CalculationResultData Process(CalculationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            CalculationContinuation continuation;
            do
            {
                Int32 emptyCount = context.EmptyCount;
                continuation = ExecuteProcessors(context, ProcessRows, ProcessColumns, ProcessSquares);
                if (context.EmptyCount == 0)
                    continuation = CalculationContinuation.Finish;
                if (emptyCount == context.EmptyCount)
                    continuation = CalculationContinuation.NeedPrediction;
            } while (continuation == CalculationContinuation.Continue);
            return CreateCalculationResultData(continuation, context);
        }

        private Boolean ProcessRows(CalculationContext context)
        {
            foreach (Int32 row in Enumerable.Range(1, Defs.GridSide))
            {
                if (!Process(_cellsInfoFactory.CreateForRow(context.Grid, row), context))
                    return false;
            }
            return true;
        }

        private Boolean ProcessColumns(CalculationContext context)
        {
            foreach (Int32 column in Enumerable.Range(1, Defs.GridSide))
            {
                if (!Process(_cellsInfoFactory.CreateForColumn(context.Grid, column), context))
                    return false;
            }
            return true;
        }

        private Boolean ProcessSquares(CalculationContext context)
        {
            foreach (GridPoint cell in Defs.Squares)
            {
                if (!Process(_cellsInfoFactory.CreateForSquare(context.Grid, cell), context))
                    return false;
            }
            return true;
        }

        private Boolean Process(CellsInfo cellsInfo, CalculationContext context)
        {
            NumbersInfo numbersInfo = _numbersInfoFactory.Create(cellsInfo);
            Tuple<GridPoint, Int32> data = ChooseCell(cellsInfo, numbersInfo);
            while (data != null)
            {
                context.Grid[data.Item1] = data.Item2;
                context.EmptyCount--;
                UseCell(cellsInfo, numbersInfo, data);
                if (!_checker.Check(cellsInfo, numbersInfo))
                    return false;
                data = ChooseCell(cellsInfo, numbersInfo);
            }
            return true;
        }

        private Tuple<GridPoint, Int32> ChooseCell(CellsInfo cellsInfo, NumbersInfo numbersInfo)
        {
            return ChooseCell(cellsInfo) ?? ChooseCell(numbersInfo);
        }

        private Tuple<GridPoint, Int32> ChooseCell(CellsInfo cellsInfo)
        {
            foreach (KeyValuePair<GridPoint, NumbersBinary> cellInfo in cellsInfo.Data)
            {
                for (Int32 number = Defs.MinNumber; number <= Defs.MaxNumber; ++number)
                {
                    if (cellInfo.Value == NumbersBinaryHelper.CreateForNumber(number))
                        return new Tuple<GridPoint, Int32>(cellInfo.Key, number);
                }
            }
            return null;
        }

        private Tuple<GridPoint, Int32> ChooseCell(NumbersInfo numbersInfo)
        {
            return numbersInfo.Data.Where(info => info.Value.Count == 1).Select(info => new Tuple<GridPoint, Int32>(info.Value[0], info.Key)).FirstOrDefault();
        }

        private void UseCell(CellsInfo cellsInfo, NumbersInfo numbersInfo, Tuple<GridPoint, Int32> data)
        {
            cellsInfo.Data.Remove(data.Item1);
            foreach (GridPoint cell in cellsInfo.Data.Keys.ToArray())
                cellsInfo.Data[cell] = cellsInfo.Data[cell].UseNumber(data.Item2);
            numbersInfo.Data.Remove(data.Item2);
            foreach (Int32 number in numbersInfo.Data.Keys.ToArray())
                numbersInfo.Data[number].Remove(data.Item1);
        }

        private CalculationResultData CreateCalculationResultData(CalculationContinuation continuation, CalculationContext context)
        {
            switch (continuation)
            {
                case CalculationContinuation.Finish:
                    return new CalculationResultData(CalculationResult.Finish, context);
                case CalculationContinuation.NeedPrediction:
                    return new CalculationResultData(CalculationResult.NeedPrediction, context);
                case CalculationContinuation.Stop:
                    return new CalculationResultData(CalculationResult.Stop, null);
                default:
                    throw new InvalidOperationException();
            }
        }

        private CalculationContinuation ExecuteProcessors(CalculationContext context, params Func<CalculationContext, Boolean>[] processors)
        {
            return processors.Any(processor => !processor(context)) ? CalculationContinuation.Stop : CalculationContinuation.Continue;
        }

        private readonly CellsInfoFactory _cellsInfoFactory = new CellsInfoFactory();
        private readonly NumbersInfoFactory _numbersInfoFactory = new NumbersInfoFactory();
        private readonly CalculationChecker _checker = new CalculationChecker();
    }
}
