namespace Sudoku.CSharp.CalculatorImpl
{
    internal enum CalculationResult { Finish, NeedPrediction, Stop }

    internal class CalculationResultData
    {
        public CalculationResultData(CalculationResult result, /* CanBeNull */CalculationContext context)
        {
            Result = result;
            Context = context;
        }

        public CalculationResult Result { get; }

        public CalculationContext Context { get; }
    }
}