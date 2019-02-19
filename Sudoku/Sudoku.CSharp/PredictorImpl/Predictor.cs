using System;
using System.Collections.Generic;
using Sudoku.CSharp.CalculatorImpl;

namespace Sudoku.CSharp.PredictorImpl
{
    internal class Predictor
    {
        public Grid CreatePrediction(Stack<PredictContext> predictionStack, CalculationContext calculationContext)
        {
            if (predictionStack == null)
                throw new ArgumentNullException(nameof(predictionStack));
            if (calculationContext == null)
                throw new ArgumentNullException(nameof(calculationContext));
            PredictContextFactory factory = new PredictContextFactory();
            PredictContext predictContext = factory.Create(calculationContext.Grid);
            predictionStack.Push(predictContext);
            return ApplyNextPrediction(predictionStack);
        }

        public Grid ApplyNextPrediction(Stack<PredictContext> predictionStack)
        {
            if (predictionStack == null)
                throw new ArgumentNullException(nameof(predictionStack));
            while (predictionStack.Peek().FreeCells.Count == 0)
                predictionStack.Pop();
            PredictContext predictContext = predictionStack.Peek();
            GridPoint cell = predictContext.FreeCells.Dequeue();
            Grid grid = predictContext.Grid.Clone();
            grid[cell] = predictContext.Number;
            return grid;
        }
    }
}