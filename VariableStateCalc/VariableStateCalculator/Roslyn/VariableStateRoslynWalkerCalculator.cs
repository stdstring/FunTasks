using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VariableStateCalculator.Roslyn
{
    public class VariableStateRoslynWalkerCalculator : IVariableStateCalculator
    {
        public Int32[] Calculate(String source)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            RoslynCompilationChecker.CheckErrors(tree);
            MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First(decl => decl.Identifier.Text == "Evaluate");
            VariableStateCalculatorWalker walker = new VariableStateCalculatorWalker();
            walker.Visit(method.Body);
            return walker.FrameStack.Peek().States.Select(state => state.AssignmentValue).OrderBy(value => value).ToArray();
        }

        private class VariableStateCalculatorWalker : CSharpSyntaxWalker
        {
            public VariableStateCalculatorWalker()
            {
                FrameStack = new Stack<ExecutionFrame>();
                FrameStack.Push(new ExecutionFrame());
            }

            public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
            {
                if (node.Declaration.Variables.Count != 1)
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
                VariableDeclaratorSyntax variable = node.Declaration.Variables[0];
                String variableId = variable.Identifier.Text;
                if (variableId != VariableName)
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
                IList<SyntaxNode> variableChildren = variable.ChildNodes().ToList();
                if (variableChildren.Count == 1 && variableChildren[0] is EqualsValueClauseSyntax)
                {
                    EqualsValueClauseSyntax value = (EqualsValueClauseSyntax)variableChildren[0];
                    FrameStack.Peek().States.Clear();
                    FrameStack.Peek().States.Add(new ExecutionState(Int32.Parse(value.Value.GetText().ToString())));
                }
                base.VisitLocalDeclarationStatement(node);
            }

            public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
            {
                IdentifierNameSyntax left = node.Left as IdentifierNameSyntax;
                LiteralExpressionSyntax right = node.Right as LiteralExpressionSyntax;
                if (left == null || right == null)
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
                String identifier = left.Identifier.Text;
                Object value = right.Token.Value;
                if (identifier != VariableName)
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
                FrameStack.Peek().States.Clear();
                FrameStack.Peek().States.Add(new ExecutionState((Int32) value));
                base.VisitAssignmentExpression(node);
            }

            public override void VisitIfStatement(IfStatementSyntax node)
            {
                if (node.Else != null)
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
                Int32 usedParameter = ExtractParameterFromCondition(node.Condition);
                FrameStack.Push(new ExecutionFrame(usedParameter));
                base.VisitIfStatement(node);
                ExecutionFrame frame = FrameStack.Pop();
                FrameStack.Peek().Merge(frame);
            }

            private Int32 ExtractParameterFromCondition(ExpressionSyntax condition)
            {
                ElementAccessExpressionSyntax elementAccessExpression = condition as ElementAccessExpressionSyntax;
                if (elementAccessExpression == null)
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
                BracketedArgumentListSyntax args = elementAccessExpression.ArgumentList;
                if (args.Arguments.Count != 1)
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
                return Int32.Parse(args.Arguments[0].Expression.GetText().ToString());
            }

            public Stack<ExecutionFrame> FrameStack { get; }

            private const String VariableName = "x";
        }
    }
}
