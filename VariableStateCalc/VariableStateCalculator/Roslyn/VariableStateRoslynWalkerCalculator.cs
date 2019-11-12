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
            return walker.StateStack.Peek().OrderBy(value => value).ToArray();
        }

        private class VariableStateCalculatorWalker : CSharpSyntaxWalker
        {
            public VariableStateCalculatorWalker()
            {
                StateStack = new Stack<ISet<Int32>>();
                StateStack.Push(new HashSet<Int32>());
            }

            public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
            {
                if (node.Declaration.Variables.Count == 1)
                {
                    VariableDeclaratorSyntax variable = node.Declaration.Variables[0];
                    String variableId = variable.Identifier.Text;
                    if (variableId == VariableName)
                    {
                        IList<SyntaxNode> variableChildren = variable.ChildNodes().ToList();
                        if (variableChildren.Count == 1 && variableChildren[0] is EqualsValueClauseSyntax)
                        {
                            EqualsValueClauseSyntax value = (EqualsValueClauseSyntax) variableChildren[0];
                            StateStack.Peek().Clear();
                            StateStack.Peek().Add(Int32.Parse(value.Value.GetText().ToString()));
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
                base.VisitLocalDeclarationStatement(node);
            }

            public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
            {
                ExpressionSyntax left = node.Left;
                ExpressionSyntax right = node.Right;
                if (left is IdentifierNameSyntax && right is LiteralExpressionSyntax)
                {
                    String identifier = ((IdentifierNameSyntax)left).Identifier.Text;
                    Object value = ((LiteralExpressionSyntax)right).Token.Value;
                    if (identifier == VariableName)
                    {
                        StateStack.Peek().Clear();
                        StateStack.Peek().Add((Int32) value);
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
                base.VisitAssignmentExpression(node);
            }

            public override void VisitIfStatement(IfStatementSyntax node)
            {
                StateStack.Push(new HashSet<Int32>());
                base.VisitIfStatement(node);
                ISet<Int32> nestedStates = StateStack.Pop();
                StateStack.Peek().UnionWith(nestedStates);
            }

            public Stack<ISet<Int32>> StateStack { get; }

            private const String VariableName = "x";
        }
    }
}
