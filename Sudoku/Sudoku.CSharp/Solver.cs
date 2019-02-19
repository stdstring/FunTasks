using System;
using System.Collections.Generic;
using Sudoku.CSharp.CalculatorImpl;
using Sudoku.CSharp.PredictorImpl;

namespace Sudoku.CSharp
{
    public class Solver
    {
        public Grid Solve(Grid source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            CalculationContextFactory calculationContextFactory = new CalculationContextFactory();
            Calculator calculator = new Calculator();
            Predictor predictor = new Predictor();
            Stack<PredictContext> predictionStack = new Stack<PredictContext>();
            CalculationContext calculationContext = calculationContextFactory.Create(source);
            CalculationResultData calculationResult = calculator.Process(calculationContext);
            while (calculationResult.Result != CalculationResult.Finish)
            {
                switch (calculationResult.Result)
                {
                    case CalculationResult.NeedPrediction:
                        calculationContext = calculationContextFactory.Create(predictor.CreatePrediction(predictionStack, calculationContext));
                        break;
                    case CalculationResult.Stop:
                        calculationContext = calculationContextFactory.Create(predictor.ApplyNextPrediction(predictionStack));
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                calculationResult = calculator.Process(calculationContext);
            }
            return calculationResult.Context.Grid;
        }
    }
}
