namespace ParseLispExpressionViaLexYacc

open FSharp.Text.Lexing
open NUnit.Framework

[<TestFixture>]
type ExpressionParserTests() =

    [<TestCase("(add 1 2)", 3)>]
    [<TestCase("(mult 3 (add 2 3))", 15)>]
    [<TestCase("(let x 2 (mult x 5))", 10)>]
    [<TestCase("(let x 2 (mult x (let x 3 y 4 (add x y))))", 14)>]
    [<TestCase("(let x 3 x 2 x)", 2)>]
    [<TestCase("(let x 1 y 2 x (add x y) (add x y))", 5)>]
    [<TestCase("(let x 2 (add (let x 3 (let x 4 x)) x))", 6)>]
    [<TestCase("(let a1 3 b2 (add a1 1) b2)", 4)>]
    [<TestCase("(let x 7 -12)", -12)>]
    member public this.EvaluateLispExpression(expression: string, expectedValue: int) =
        let lexbuf = LexBuffer<_>.FromString expression
        let expressionTree = LispExpressionParser.start LispExpressionLexer.tokenize lexbuf
        let actualValue = expressionTree |> LispExpressionDefs.calcExpressionValue
        Assert.AreEqual(expectedValue, actualValue)
