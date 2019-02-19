using System;
using System.Collections.Generic;

namespace Sudoku.CSharp.CommonImpl
{
    internal class NumbersInfoFactory
    {
        public NumbersInfo Create(CellsInfo cellsInfo)
        {
            if (cellsInfo == null)
                throw new ArgumentNullException(nameof(cellsInfo));
            NumbersInfo numbersInfo = new NumbersInfo();
            foreach (KeyValuePair<GridPoint, NumbersBinary> cellInfo in cellsInfo.Data)
                Process(numbersInfo, cellInfo.Key, cellInfo.Value);
            return numbersInfo;
        }

        private void Process(NumbersInfo numbersInfo, GridPoint cell, NumbersBinary freeNumbers)
        {
            for (Int32 number = Defs.MinNumber; number < Defs.MaxNumber; ++number)
            {
                if ((freeNumbers & NumbersBinaryHelper.CreateForNumber(number)) != 0)
                    AddCell(numbersInfo, number, cell);
            }
        }

        private void AddCell(NumbersInfo numbersInfo, Int32 number, GridPoint cell)
        {
            if (!numbersInfo.Data.ContainsKey(number))
                numbersInfo.Data[number] = new List<GridPoint>();
            numbersInfo.Data[number].Add(cell);
        }
    }
}