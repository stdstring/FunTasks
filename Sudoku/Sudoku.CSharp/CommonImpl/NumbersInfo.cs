using System.Collections.Generic;

namespace Sudoku.CSharp.CommonImpl
{
    internal class NumbersInfo
    {
        public NumbersInfo()
        {
            Data = new Dictionary<int, IList<GridPoint>>();
        }

        public IDictionary<int, IList<GridPoint>> Data { get; }
    }
}