module LispExpressionDefs

type Expression =
    | Number of value: int
    | Variable of name: string
    | Add of left: Expression * right: Expression
    | Mult of left: Expression * right: Expression
    | Let of variables: (string * Expression) list * result: Expression

let calcExpressionValue (expression: Expression) =
    let rec calcImpl (expression: Expression) (context: Map<string, int>) =
        match expression with
        | Expression.Number value -> value
        | Expression.Variable name -> context.[name]
        | Expression.Add (left, right) -> (calcImpl left context) + (calcImpl right context)
        | Expression.Mult (left, right) -> (calcImpl left context) * (calcImpl right context)
        | Expression.Let (variables, result) ->
            variables |> List.fold (fun (localContext) (name, expression) -> let value = localContext |> calcImpl expression in localContext |> Map.add name value) context |> calcImpl result
    Map.empty |> calcImpl expression