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
            return listener.StateStack.Peek().OrderBy(value => value).ToArray();
        }

        private class VariableStateCalculatorGrammarListener : VariableStateCalculatorGrammarBaseListener
        {
            public VariableStateCalculatorGrammarListener()
            {
                StateStack = new Stack<ISet<Int32>>();
            }

            public override void EnterEvaluateBodyDef(VariableStateCalculatorGrammarParser.EvaluateBodyDefContext context)
            {
                StateStack.Push(new HashSet<Int32>());
                base.EnterEvaluateBodyDef(context);
            }

            public override void EnterXVariableDef(VariableStateCalculatorGrammarParser.XVariableDefContext context)
            {
                ITerminalNode valueNode = context.NUMBER();
                if (valueNode != null)
                {
                    Int32 value = Int32.Parse(valueNode.GetText());
                    StateStack.Peek().Clear();
                    StateStack.Peek().Add(value);
                }
                base.EnterXVariableDef(context);
            }

            public override void EnterBlockStatementAssignment(VariableStateCalculatorGrammarParser.BlockStatementAssignmentContext context)
            {
                Int32 value = Int32.Parse(context.assignmentDef().NUMBER().GetText());
                StateStack.Peek().Clear();
                StateStack.Peek().Add(value);
                base.EnterBlockStatementAssignment(context);
            }

            public override void EnterIfBodyAssignment(VariableStateCalculatorGrammarParser.IfBodyAssignmentContext context)
            {
                Int32 value = Int32.Parse(context.assignmentDef().NUMBER().GetText());
                StateStack.Peek().Add(value);
                base.EnterIfBodyAssignment(context);
            }

            public override void EnterIfBodyBlock(VariableStateCalculatorGrammarParser.IfBodyBlockContext context)
            {
                StateStack.Push(new HashSet<Int32>());
                base.EnterIfBodyBlock(context);
            }

            public override void ExitIfBodyBlock(VariableStateCalculatorGrammarParser.IfBodyBlockContext context)
            {
                ISet<Int32> blockStates = StateStack.Pop();
                StateStack.Peek().UnionWith(blockStates);
                base.ExitIfBodyBlock(context);
            }

            public Stack<ISet<Int32>> StateStack { get; }
        }
    }
}
