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
            Stack<ISet<Int32>> stateStack = new Stack<ISet<Int32>>();
            stateStack.Push(new HashSet<Int32>());
            ProcessBlock(method.Body, stateStack);
            return stateStack.Peek().OrderBy(value => value).ToArray();
        }

        private void ProcessBlock(BlockSyntax block, Stack<ISet<Int32>> stateStack)
        {
            foreach (SyntaxNode child in block.ChildNodes())
            {
                if (child is ExpressionStatementSyntax)
                {
                    ProcessExpression((ExpressionStatementSyntax) child, stateStack);
                }
                else if (child is IfStatementSyntax)
                {
                    ProcessIf((IfStatementSyntax) child, stateStack);
                }
                else if (child is BlockSyntax)
                {
                    ProcessBlock((BlockSyntax) child, stateStack);
                }
                else if (child is LocalDeclarationStatementSyntax)
                {
                    ProcessLocalDeclaration((LocalDeclarationStatementSyntax) child, stateStack);
                }
                else
                {
                    // warn about unexpected case
                }
            }
        }

        private void ProcessLocalDeclaration(LocalDeclarationStatementSyntax localDeclaration, Stack<ISet<Int32>> stateStack)
        {
            if (localDeclaration.Declaration.Variables.Count == 1)
            {
                VariableDeclaratorSyntax variable = localDeclaration.Declaration.Variables[0];
                String variableId = variable.Identifier.Text;
                if (variableId == VariableName)
                {
                    IList<SyntaxNode> variableChildren = variable.ChildNodes().ToList();
                    if (variableChildren.Count == 1 && variableChildren[0] is EqualsValueClauseSyntax)
                    {
                        EqualsValueClauseSyntax value = (EqualsValueClauseSyntax) variableChildren[0];
                        stateStack.Peek().Clear();
                        stateStack.Peek().Add(Int32.Parse(value.Value.GetText().ToString()));
                    }
                }
                else
                {
                    // warn about unexpected case
                }
            }
            else
            {
                // warn about unexpected case
            }
        }

        private void ProcessExpression(ExpressionStatementSyntax expression, Stack<ISet<Int32>> stateStack)
        {
            if (expression.Expression.Kind() == SyntaxKind.SimpleAssignmentExpression)
            {
                AssignmentExpressionSyntax assignmentExpression = (AssignmentExpressionSyntax) expression.Expression;
                ExpressionSyntax left = assignmentExpression.Left;
                ExpressionSyntax right = assignmentExpression.Right;
                if (left is IdentifierNameSyntax && right is LiteralExpressionSyntax)
                {
                    String identifier = ((IdentifierNameSyntax) left).Identifier.Text;
                    Object value = ((LiteralExpressionSyntax) right).Token.Value;
                    if (identifier == VariableName)
                    {
                        stateStack.Peek().Clear();
                        stateStack.Peek().Add((Int32) value);
                    }
                    else
                    {
                        // warn about unexpected case
                    }
                }
                else
                {
                    // warn about unexpected case
                }
            }
            else
            {
                // warn about unexpected case
            }
        }

        private void ProcessIf(IfStatementSyntax ifStatement, Stack<ISet<Int32>> stateStack)
        {
            SyntaxNode bodyNode = ifStatement.ChildNodes().Last();
            // process the following case: if (BOOL_EXPR) ID = VALUE;
            if (bodyNode is ExpressionStatementSyntax)
            {
                stateStack.Push(new HashSet<Int32>());
                ProcessExpression((ExpressionStatementSyntax) bodyNode, stateStack);
                ISet<Int32> expressionState = stateStack.Pop();
                stateStack.Peek().UnionWith(expressionState);
            }
            // process the following case: if (BOOL_EXPR) { ... }
            else if (bodyNode is BlockSyntax)
            {
                stateStack.Push(new HashSet<Int32>());
                ProcessBlock((BlockSyntax) bodyNode, stateStack);
                ISet<Int32> expressionState = stateStack.Pop();
                stateStack.Peek().UnionWith(expressionState);
            }
            // process the following case: if (BOOL_EXPR1) if (BOOL_EXPR2) ...
            else if (bodyNode is IfStatementSyntax)
            {
                ProcessIf((IfStatementSyntax) bodyNode, stateStack);
            }
            else
            {
                // warn about unexpected case
            }
        }

        private const String VariableName = "x";
    }
}
