using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace VariableStateCalculator.Antlr
{
    public class VariableStateAntlrCalculator : IVariableStateCalculator
    {
        public Int32[] Calculate(String source)
        {
            AntlrInputStream inputStream = new AntlrInputStream(source);
            VariableStateCalculatorGrammarLexer lexer = new VariableStateCalculatorGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            VariableStateCalculatorGrammarParser parser = new VariableStateCalculatorGrammarParser(commonTokenStream);
            VariableStateCalculatorGrammarParser.AppDefContext appDefContext = parser.appDef();
            if (parser.NumberOfSyntaxErrors > 0)
                throw new InvalidOperationException();
            ParseTreeWalker walker = new ParseTreeWalker();
            VariableStateCalculatorGrammarListener listener = new VariableStateCalculatorGrammarListener();
            walker.Walk(listener, appDefContext);
            return listener.FrameStack.Peek().States.Select(state => state.AssignmentValue).OrderBy(value => value).ToArray();
        }

        private class VariableStateCalculatorGrammarListener : VariableStateCalculatorGrammarBaseListener
        {
            public VariableStateCalculatorGrammarListener()
            {
                FrameStack = new Stack<ExecutionFrame>();
            }

            public override void EnterEvaluateBodyDef(VariableStateCalculatorGrammarParser.EvaluateBodyDefContext context)
            {
                FrameStack.Push(new ExecutionFrame());
                base.EnterEvaluateBodyDef(context);
            }

            public override void EnterXVariableDef(VariableStateCalculatorGrammarParser.XVariableDefContext context)
            {
                ITerminalNode valueNode = context.NUMBER();
                if (valueNode != null)
                {
                    Int32 value = Int32.Parse(valueNode.GetText());
                    FrameStack.Peek().States.Clear();
                    FrameStack.Peek().States.Add(new ExecutionState(value));
                }
                base.EnterXVariableDef(context);
            }

            public override void EnterAssignmentDef(VariableStateCalculatorGrammarParser.AssignmentDefContext context)
            {
                Int32 value = Int32.Parse(context.NUMBER().GetText());
                FrameStack.Peek().States.Clear();
                FrameStack.Peek().States.Add(new ExecutionState(value));
                base.EnterAssignmentDef(context);
            }

            public override void EnterIfDef(VariableStateCalculatorGrammarParser.IfDefContext context)
            {
                Int32 usedParameter = Int32.Parse(context.NUMBER().GetText());
                FrameStack.Push(new ExecutionFrame(usedParameter));
                base.EnterIfDef(context);
            }

            public override void ExitIfDef(VariableStateCalculatorGrammarParser.IfDefContext context)
            {
                ExecutionFrame frame = FrameStack.Pop();
                FrameStack.Peek().Merge(frame);
                base.ExitIfDef(context);
            }

            public Stack<ExecutionFrame> FrameStack { get; }
        }
    }
}
