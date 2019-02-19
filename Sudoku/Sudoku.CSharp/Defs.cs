using System;

namespace Sudoku.CSharp
{
    public static class Defs
    {
        public const Int32 SquareSide = 3;
        public const Int32 GridSide = 9;

        public const Int32 MinNumber = 1;
        public const Int32 MaxNumber = 9;

        public static readonly GridPoint[] Squares =
        {
            new GridPoint(1, 1), new GridPoint(1, 4), new GridPoint(1, 7),
            new GridPoint(4, 1), new GridPoint(4, 4), new GridPoint(4, 7),
            new GridPoint(7, 1), new GridPoint(7, 4), new GridPoint(7, 7)
        };
    }
}