namespace ParseLispExpression

open System.Collections.Generic
open System.Text
open System

type EvaluateState = {Expression: char list; Result: int}

type ReadState<'TValue> = {Expression: char list; Value: 'TValue}

type ExpressionParser() =

    member public this.Evaluate(expression: string) =
        this.EvaluateExpression(expression |> Seq.toList, new Dictionary<string, int>()).Result

    member private this.EvaluateExpression(expression: char list, context: IDictionary<string, int>) =
        let evaluateNumber (sign: int) (expression: char list) =
            let numberState = expression |> this.ReadNumber
            {EvaluateState.Expression = numberState.Expression |> this.SkipWhitespaces; EvaluateState.Result = sign * numberState.Value}
        match expression with
        | '(' :: 'l' :: 'e' :: 't' :: ' ' :: expressionRest -> this.EvaluateLetExpression(expressionRest |> this.SkipWhitespaces, context)
        | '(' :: 'a' :: 'd' :: 'd' :: ' ' :: expressionRest -> this.EvaluateAddExpression(expressionRest |> this.SkipWhitespaces, context)
        | '(' :: 'm' :: 'u' :: 'l' :: 't' :: ' ' :: expressionRest -> this.EvaluateMultExpression(expressionRest |> this.SkipWhitespaces, context)
        | ch :: _ when ch |> Char.IsLower ->
            let nameState = expression |> this.ReadName
            {EvaluateState.Expression = nameState.Expression |> this.SkipWhitespaces; EvaluateState.Result = context.[nameState.Value]}
        | ch :: _ when ch |> Char.IsDigit -> expression |> evaluateNumber 1
        | '-' :: ch :: expressionRest when ch |> Char.IsDigit -> ch :: expressionRest |> evaluateNumber -1
        | _ -> failwith "Unexpected branch of match expression"

    member private this.EvaluateLetExpression(expression: char list, parentContext: IDictionary<string, int>) =
        let currentContext = new Dictionary<string, int>(parentContext)
        let expressionRest = this.EvaluateAssignExpressions(expression, currentContext)
        let letExpressionValue = this.EvaluateExpression(expressionRest, currentContext)
        // process trailing ')'
        let expressionRest = letExpressionValue.Expression |> this.SkipWhitespaces |> List.tail
        {EvaluateState.Expression = expressionRest; EvaluateState.Result = letExpressionValue.Result}

    member private this.EvaluateAssignExpressions(expression: char list, context: IDictionary<string, int>) =
        let rec evaluateImpl (expression: char list) =
            match expression with
            | ch :: _ when ch |> Char.IsLower ->
                let nameState = expression |> this.ReadName
                match nameState.Expression |> this.SkipWhitespaces with
                | ')' :: _ -> expression
                | expressionRest ->
                    let valueState = this.EvaluateExpression(expressionRest, context)
                    context.[nameState.Value] <- valueState.Result
                    valueState.Expression |> this.SkipWhitespaces |> evaluateImpl
            | _ -> expression
        expression |> evaluateImpl

    member private this.EvaluateAddExpression(expression: char list, context: IDictionary<string, int>) =
        let operand1State = this.EvaluateExpression(expression, context)
        let operand2State = this.EvaluateExpression(operand1State.Expression |> this.SkipWhitespaces, context)
        // process trailing ')'
        let expressionRest = operand2State.Expression |> this.SkipWhitespaces |> List.tail
        {EvaluateState.Expression = expressionRest; EvaluateState.Result = operand1State.Result + operand2State.Result}

    member private this.EvaluateMultExpression(expression: char list, context: IDictionary<string, int>) =
        let operand1State = this.EvaluateExpression(expression, context)
        let operand2State = this.EvaluateExpression(operand1State.Expression |> this.SkipWhitespaces, context)
        // process trailing ')'
        let expressionRest = operand2State.Expression |> this.SkipWhitespaces |> List.tail
        {EvaluateState.Expression = expressionRest; EvaluateState.Result = operand1State.Result * operand2State.Result}

    member private this.SkipWhitespaces(expression: char list) =
        match expression with
        | ' ' :: expressionRest -> this.SkipWhitespaces(expressionRest)
        | _ -> expression

    member private this.ReadNumber(expression: char list) =
        let rec readNumberImpl (expression: char list) (number: int) =
            // TODO (std_string) : think about more smart solution
            match expression with
            | '0' :: expressionRest -> (number * 10) |> readNumberImpl expressionRest
            | '1' :: expressionRest -> (number * 10 + 1) |> readNumberImpl expressionRest
            | '2' :: expressionRest -> (number * 10 + 2) |> readNumberImpl expressionRest
            | '3' :: expressionRest -> (number * 10 + 3) |> readNumberImpl expressionRest
            | '4' :: expressionRest -> (number * 10 + 4) |> readNumberImpl expressionRest
            | '5' :: expressionRest -> (number * 10 + 5) |> readNumberImpl expressionRest
            | '6' :: expressionRest -> (number * 10 + 6) |> readNumberImpl expressionRest
            | '7' :: expressionRest -> (number * 10 + 7) |> readNumberImpl expressionRest
            | '8' :: expressionRest -> (number * 10 + 8) |> readNumberImpl expressionRest
            | '9' :: expressionRest -> (number * 10 + 9) |> readNumberImpl expressionRest
            | _ -> {ReadState.Expression = expression; ReadState.Value = number}
        readNumberImpl expression 0

    member private this.ReadName(expression: char list) =
        // A variable starts with a lowercase letter, then zero or more lowercase letters or digits.
        let name = new StringBuilder()
        let rec readNameImpl (expression: char list) =
            match expression with
            | ch :: expressionRest when ch |> Char.IsLower || ch |> Char.IsDigit ->
                ch |> name.Append |> ignore
                expressionRest |> readNameImpl
            | _ -> {ReadState.Expression = expression; ReadState.Value = name.ToString()}
        expression |> readNameImpl