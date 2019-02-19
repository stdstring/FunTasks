using System;

namespace Sudoku.CSharp.CalculatorImpl
{
    internal class CalculationContextFactory
    {
        public CalculationContext Create(Grid source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            Int32 emptyCount = 0;
            Grid dest = source.Clone();
            for (Int32 row = 1; row <= Defs.GridSide; row++)
            for (Int32 column = 1; column <= Defs.GridSide; column++)
            {
                if (dest[row, column] == 0)
                    emptyCount++;
            }
            return new CalculationContext(dest, emptyCount);
        }

        public CalculationContext Create(CalculationContext source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return Create(source.Grid);
        }
    }
}