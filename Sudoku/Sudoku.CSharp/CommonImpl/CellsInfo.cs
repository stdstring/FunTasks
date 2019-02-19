using System.Collections.Generic;

namespace Sudoku.CSharp.CommonImpl
{
    internal class CellsInfo
    {
        public CellsInfo()
        {
            Data = new Dictionary<GridPoint, NumbersBinary>();
        }

        public IDictionary<GridPoint, NumbersBinary> Data { get; }
    }
}