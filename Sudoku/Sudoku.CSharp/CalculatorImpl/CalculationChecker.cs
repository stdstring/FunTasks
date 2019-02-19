using System;
using System.Linq;
using Sudoku.CSharp.CommonImpl;

namespace Sudoku.CSharp.CalculatorImpl
{
    internal class CalculationChecker
    {
        public Boolean Check(CellsInfo cellsInfo, NumbersInfo numbersInfo)
        {
            if (cellsInfo == null)
                throw new ArgumentNullException(nameof(cellsInfo));
            if (numbersInfo == null)
                throw new ArgumentNullException(nameof(numbersInfo));
            return Check(cellsInfo) && Check(numbersInfo);
        }

        private Boolean Check(CellsInfo cellsInfo)
        {
            return cellsInfo.Data.All(info => info.Value != 0);
        }

        private Boolean Check(NumbersInfo numbersInfo)
        {
            return numbersInfo.Data.All(info => info.Value.Count > 0);
        }
    }
}