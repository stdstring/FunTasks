using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VariableStateCalculator.Roslyn
{
    public class VariableStateRoslynDirectCalculator : IVariableStateCalculator
    {
        public Int32[] Calculate(String source)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            RoslynCompilationChecker.CheckErrors(tree);
            MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First(decl => decl.Identifier.Text == "Evaluate");
            Stack<ExecutionFrame> frameStack = new Stack<ExecutionFrame>();
            frameStack.Push(new ExecutionFrame());
            ProcessBlock(method.Body, frameStack);
            return frameStack.Peek().States.Select(state => state.AssignmentValue).OrderBy(value => value).ToArray();
        }

        private void ProcessBlock(BlockSyntax block, Stack<ExecutionFrame> frameStack)
        {
            foreach (SyntaxNode child in block.ChildNodes())
            {
                if (child is ExpressionStatementSyntax)
                {
                    ProcessExpression((ExpressionStatementSyntax) child, frameStack);
                }
                else if (child is IfStatementSyntax)
                {
                    ProcessIf((IfStatementSyntax) child, frameStack);
                }
                else if (child is BlockSyntax)
                {
                    ProcessBlock((BlockSyntax) child, frameStack);
                }
                else if (child is LocalDeclarationStatementSyntax)
                {
                    ProcessLocalDeclaration((LocalDeclarationStatementSyntax) child, frameStack);
                }
                else if (child is ReturnStatementSyntax)
                {
                    // do nothing
                }
                else
                {
                    // unexpected case
                    throw new NotSupportedException();
                }
            }
        }

        private void ProcessLocalDeclaration(LocalDeclarationStatementSyntax localDeclaration, Stack<ExecutionFrame> frameStack)
        {
            if (localDeclaration.Declaration.Variables.Count != 1)
            {
                // unexpected case
                throw new NotSupportedException();
            }
            VariableDeclaratorSyntax variable = localDeclaration.Declaration.Variables[0];
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
                frameStack.Peek().States.Clear();
                frameStack.Peek().States.Add(new ExecutionState(Int32.Parse(value.Value.GetText().ToString())));
            }
        }

        private void ProcessExpression(ExpressionStatementSyntax expression, Stack<ExecutionFrame> frameStack)
        {
            if (expression.Expression.Kind() != SyntaxKind.SimpleAssignmentExpression)
            {
                // unexpected case
                throw new NotSupportedException();
            }
            AssignmentExpressionSyntax assignmentExpression = (AssignmentExpressionSyntax)expression.Expression;
            IdentifierNameSyntax left = assignmentExpression.Left as IdentifierNameSyntax;
            LiteralExpressionSyntax right = assignmentExpression.Right as LiteralExpressionSyntax;
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
            frameStack.Peek().States.Clear();
            frameStack.Peek().States.Add(new ExecutionState((Int32) value));
        }

        private void ProcessIf(IfStatementSyntax ifStatement, Stack<ExecutionFrame> frameStack)
        {
            if (ifStatement.Else != null)
            {
                // unexpected case
                throw new NotSupportedException();
            }
            Int32 usedParameter = ExtractParameterFromCondition(ifStatement.Condition);
            StatementSyntax ifBody = ifStatement.Statement;
            frameStack.Push(new ExecutionFrame(usedParameter));
            ProcessIfImpl(ifBody, frameStack);
            ExecutionFrame frame = frameStack.Pop();
            frameStack.Peek().Merge(frame);
        }

        private void ProcessIfImpl(StatementSyntax ifBody, Stack<ExecutionFrame> frameStack)
        {
            // process the following case: if (BOOL_EXPR) ID = VALUE;
            if (ifBody is ExpressionStatementSyntax)
            {
                ProcessExpression((ExpressionStatementSyntax)ifBody, frameStack);
            }
            // process the following case: if (BOOL_EXPR) { ... }
            else if (ifBody is BlockSyntax)
            {
                ProcessBlock((BlockSyntax)ifBody, frameStack);
            }
            // process the following case: if (BOOL_EXPR1) if (BOOL_EXPR2) ...
            else if (ifBody is IfStatementSyntax)
            {
                ProcessIf((IfStatementSyntax)ifBody, frameStack);
            }
            else
            {
                // unexpected case
                throw new NotSupportedException();
            }
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

        private const String VariableName = "x";
    }
}
