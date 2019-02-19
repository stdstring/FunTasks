using System;
using System.Linq;
using System.Text;

namespace Sudoku.CSharp.Tests.Utils
{
    internal static class GridConverter
    {
        public static Grid Convert(String[] source)
        {
            return Convert(String.Concat(source));
        }

        public static Grid Convert(String source)
        {
            Grid dest = new Grid(Defs.GridSide, Defs.GridSide);
            Int32[] numbers = source.Select(c => (Int32) Char.GetNumericValue(c)).ToArray();
            for (Int32 numberIndex = 0; numberIndex < numbers.Length; ++numberIndex)
            {
                Int32 row = 1 + (numberIndex) / (Defs.GridSide);
                Int32 column = 1 + (numberIndex) % (Defs.GridSide);
                dest[row, column] = numbers[numberIndex];
            }
            return dest;
        }

        public static String Convert(Grid source)
        {
            StringBuilder dest = new StringBuilder();
            for (Int32 row = 1; row <= source.RowCount; ++row)
            for (Int32 column = 1; column <= source.ColumnCount; ++column)
                dest.Append(source[row, column]);
            return dest.ToString();
        }
    }
}
