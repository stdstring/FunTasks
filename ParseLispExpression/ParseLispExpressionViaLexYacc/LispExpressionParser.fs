// Implementation file for parser generated by fsyacc
module LispExpressionParser
#nowarn "64";; // turn off warnings that type variables used in production annotations are instantiated to concrete type
open FSharp.Text.Lexing
open FSharp.Text.Parsing.ParseHelpers
# 1 "LispExpressionParser.fsy"

open LispExpressionDefs

# 10 "LispExpressionParser.fs"
// This type is the type of tokens accepted by the parser
type token = 
  | EOF
  | OPEN_BRACKET
  | CLOSE_BRACKET
  | ADD
  | MULT
  | LET
  | INT of (int)
  | ID of (string)
// This type is used to give symbolic names to token indexes, useful for error messages
type tokenId = 
    | TOKEN_EOF
    | TOKEN_OPEN_BRACKET
    | TOKEN_CLOSE_BRACKET
    | TOKEN_ADD
    | TOKEN_MULT
    | TOKEN_LET
    | TOKEN_INT
    | TOKEN_ID
    | TOKEN_end_of_input
    | TOKEN_error
// This type is used to give symbolic names to token indexes, useful for error messages
type nonTerminalId = 
    | NONTERM__startstart
    | NONTERM_start
    | NONTERM_addExpression
    | NONTERM_multExpression
    | NONTERM_letExpression
    | NONTERM_variableExpressionPairList
    | NONTERM_expression

// This function maps tokens to integer indexes
let tagOfToken (t:token) = 
  match t with
  | EOF  -> 0 
  | OPEN_BRACKET  -> 1 
  | CLOSE_BRACKET  -> 2 
  | ADD  -> 3 
  | MULT  -> 4 
  | LET  -> 5 
  | INT _ -> 6 
  | ID _ -> 7 

// This function maps integer indexes to symbolic token ids
let tokenTagToTokenId (tokenIdx:int) = 
  match tokenIdx with
  | 0 -> TOKEN_EOF 
  | 1 -> TOKEN_OPEN_BRACKET 
  | 2 -> TOKEN_CLOSE_BRACKET 
  | 3 -> TOKEN_ADD 
  | 4 -> TOKEN_MULT 
  | 5 -> TOKEN_LET 
  | 6 -> TOKEN_INT 
  | 7 -> TOKEN_ID 
  | 10 -> TOKEN_end_of_input
  | 8 -> TOKEN_error
  | _ -> failwith "tokenTagToTokenId: bad token"

/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production
let prodIdxToNonTerminal (prodIdx:int) = 
  match prodIdx with
    | 0 -> NONTERM__startstart 
    | 1 -> NONTERM_start 
    | 2 -> NONTERM_addExpression 
    | 3 -> NONTERM_multExpression 
    | 4 -> NONTERM_letExpression 
    | 5 -> NONTERM_variableExpressionPairList 
    | 6 -> NONTERM_variableExpressionPairList 
    | 7 -> NONTERM_expression 
    | 8 -> NONTERM_expression 
    | 9 -> NONTERM_expression 
    | 10 -> NONTERM_expression 
    | 11 -> NONTERM_expression 
    | _ -> failwith "prodIdxToNonTerminal: bad production index"

let _fsyacc_endOfInputTag = 10 
let _fsyacc_tagOfErrorTerminal = 8

// This function gets the name of a token as a string
let token_to_string (t:token) = 
  match t with 
  | EOF  -> "EOF" 
  | OPEN_BRACKET  -> "OPEN_BRACKET" 
  | CLOSE_BRACKET  -> "CLOSE_BRACKET" 
  | ADD  -> "ADD" 
  | MULT  -> "MULT" 
  | LET  -> "LET" 
  | INT _ -> "INT" 
  | ID _ -> "ID" 

// This function gets the data carried by a token as an object
let _fsyacc_dataOfToken (t:token) = 
  match t with 
  | EOF  -> (null : System.Object) 
  | OPEN_BRACKET  -> (null : System.Object) 
  | CLOSE_BRACKET  -> (null : System.Object) 
  | ADD  -> (null : System.Object) 
  | MULT  -> (null : System.Object) 
  | LET  -> (null : System.Object) 
  | INT _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | ID _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
let _fsyacc_gotos = [| 0us; 65535us; 1us; 65535us; 0us; 1us; 8us; 65535us; 0us; 23us; 5us; 23us; 6us; 23us; 9us; 23us; 10us; 23us; 14us; 23us; 17us; 23us; 19us; 23us; 8us; 65535us; 0us; 24us; 5us; 24us; 6us; 24us; 9us; 24us; 10us; 24us; 14us; 24us; 17us; 24us; 19us; 24us; 8us; 65535us; 0us; 25us; 5us; 25us; 6us; 25us; 9us; 25us; 10us; 25us; 14us; 25us; 17us; 25us; 19us; 25us; 1us; 65535us; 13us; 14us; 8us; 65535us; 0us; 2us; 5us; 6us; 6us; 7us; 9us; 10us; 10us; 11us; 14us; 15us; 17us; 18us; 19us; 20us; |]
let _fsyacc_sparseGotoTableRowOffsets = [|0us; 1us; 3us; 12us; 21us; 30us; 32us; |]
let _fsyacc_stateToProdIdxsTableElements = [| 1us; 0us; 1us; 0us; 1us; 1us; 1us; 1us; 3us; 2us; 3us; 4us; 1us; 2us; 1us; 2us; 1us; 2us; 1us; 2us; 1us; 3us; 1us; 3us; 1us; 3us; 1us; 3us; 1us; 4us; 2us; 4us; 6us; 1us; 4us; 1us; 4us; 1us; 5us; 1us; 5us; 2us; 6us; 8us; 1us; 6us; 1us; 7us; 1us; 8us; 1us; 9us; 1us; 10us; 1us; 11us; |]
let _fsyacc_stateToProdIdxsTableRowOffsets = [|0us; 2us; 4us; 6us; 8us; 12us; 14us; 16us; 18us; 20us; 22us; 24us; 26us; 28us; 30us; 33us; 35us; 37us; 39us; 41us; 44us; 46us; 48us; 50us; 52us; 54us; |]
let _fsyacc_action_rows = 26
let _fsyacc_actionTableElements = [|3us; 32768us; 1us; 4us; 6us; 21us; 7us; 22us; 0us; 49152us; 1us; 32768us; 0us; 3us; 0us; 16385us; 3us; 32768us; 3us; 5us; 4us; 9us; 5us; 13us; 3us; 32768us; 1us; 4us; 6us; 21us; 7us; 22us; 3us; 32768us; 1us; 4us; 6us; 21us; 7us; 22us; 1us; 32768us; 2us; 8us; 0us; 16386us; 3us; 32768us; 1us; 4us; 6us; 21us; 7us; 22us; 3us; 32768us; 1us; 4us; 6us; 21us; 7us; 22us; 1us; 32768us; 2us; 12us; 0us; 16387us; 1us; 32768us; 7us; 17us; 3us; 32768us; 1us; 4us; 6us; 21us; 7us; 19us; 1us; 32768us; 2us; 16us; 0us; 16388us; 3us; 32768us; 1us; 4us; 6us; 21us; 7us; 22us; 0us; 16389us; 3us; 16392us; 1us; 4us; 6us; 21us; 7us; 22us; 0us; 16390us; 0us; 16391us; 0us; 16392us; 0us; 16393us; 0us; 16394us; 0us; 16395us; |]
let _fsyacc_actionTableRowOffsets = [|0us; 4us; 5us; 7us; 8us; 12us; 16us; 20us; 22us; 23us; 27us; 31us; 33us; 34us; 36us; 40us; 42us; 43us; 47us; 48us; 52us; 53us; 54us; 55us; 56us; 57us; |]
let _fsyacc_reductionSymbolCounts = [|1us; 2us; 5us; 5us; 5us; 2us; 3us; 1us; 1us; 1us; 1us; 1us; |]
let _fsyacc_productionToNonTerminalTable = [|0us; 1us; 2us; 3us; 4us; 5us; 5us; 6us; 6us; 6us; 6us; 6us; |]
let _fsyacc_immediateActions = [|65535us; 49152us; 65535us; 16385us; 65535us; 65535us; 65535us; 65535us; 16386us; 65535us; 65535us; 65535us; 16387us; 65535us; 65535us; 65535us; 16388us; 65535us; 16389us; 65535us; 16390us; 16391us; 16392us; 16393us; 16394us; 16395us; |]
let _fsyacc_reductions ()  =    [| 
# 124 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> LispExpressionDefs.Expression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
                      raise (FSharp.Text.Parsing.Accept(Microsoft.FSharp.Core.Operators.box _1))
                   )
                 : 'gentype__startstart));
# 133 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> 'gentype_expression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 16 "LispExpressionParser.fsy"
                                             _1 
                   )
# 16 "LispExpressionParser.fsy"
                 : LispExpressionDefs.Expression));
# 144 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _3 = parseState.GetInput(3) :?> 'gentype_expression in
            let _4 = parseState.GetInput(4) :?> 'gentype_expression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 18 "LispExpressionParser.fsy"
                                                                                           Expression.Add (left=_3, right=_4) 
                   )
# 18 "LispExpressionParser.fsy"
                 : 'gentype_addExpression));
# 156 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _3 = parseState.GetInput(3) :?> 'gentype_expression in
            let _4 = parseState.GetInput(4) :?> 'gentype_expression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 20 "LispExpressionParser.fsy"
                                                                                             Expression.Mult (left=_3, right=_4) 
                   )
# 20 "LispExpressionParser.fsy"
                 : 'gentype_multExpression));
# 168 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _3 = parseState.GetInput(3) :?> 'gentype_variableExpressionPairList in
            let _4 = parseState.GetInput(4) :?> 'gentype_expression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 22 "LispExpressionParser.fsy"
                                                                                                            Expression.Let (variables=List.rev _3, result=_4) 
                   )
# 22 "LispExpressionParser.fsy"
                 : 'gentype_letExpression));
# 180 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> string in
            let _2 = parseState.GetInput(2) :?> 'gentype_expression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 25 "LispExpressionParser.fsy"
                                           [(_1, _2)] 
                   )
# 25 "LispExpressionParser.fsy"
                 : 'gentype_variableExpressionPairList));
# 192 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> 'gentype_variableExpressionPairList in
            let _2 = parseState.GetInput(2) :?> string in
            let _3 = parseState.GetInput(3) :?> 'gentype_expression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 26 "LispExpressionParser.fsy"
                                                                      (_2, _3) :: _1 
                   )
# 26 "LispExpressionParser.fsy"
                 : 'gentype_variableExpressionPairList));
# 205 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> int in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 29 "LispExpressionParser.fsy"
                                 Expression.Number(value = _1) 
                   )
# 29 "LispExpressionParser.fsy"
                 : 'gentype_expression));
# 216 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> string in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 30 "LispExpressionParser.fsy"
                                Expression.Variable(name = _1) 
                   )
# 30 "LispExpressionParser.fsy"
                 : 'gentype_expression));
# 227 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> 'gentype_addExpression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 31 "LispExpressionParser.fsy"
                                           _1 
                   )
# 31 "LispExpressionParser.fsy"
                 : 'gentype_expression));
# 238 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> 'gentype_multExpression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 32 "LispExpressionParser.fsy"
                                            _1 
                   )
# 32 "LispExpressionParser.fsy"
                 : 'gentype_expression));
# 249 "LispExpressionParser.fs"
        (fun (parseState : FSharp.Text.Parsing.IParseState) ->
            let _1 = parseState.GetInput(1) :?> 'gentype_letExpression in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 33 "LispExpressionParser.fsy"
                                           _1 
                   )
# 33 "LispExpressionParser.fsy"
                 : 'gentype_expression));
|]
# 261 "LispExpressionParser.fs"
let tables : FSharp.Text.Parsing.Tables<_> = 
  { reductions= _fsyacc_reductions ();
    endOfInputTag = _fsyacc_endOfInputTag;
    tagOfToken = tagOfToken;
    dataOfToken = _fsyacc_dataOfToken; 
    actionTableElements = _fsyacc_actionTableElements;
    actionTableRowOffsets = _fsyacc_actionTableRowOffsets;
    stateToProdIdxsTableElements = _fsyacc_stateToProdIdxsTableElements;
    stateToProdIdxsTableRowOffsets = _fsyacc_stateToProdIdxsTableRowOffsets;
    reductionSymbolCounts = _fsyacc_reductionSymbolCounts;
    immediateActions = _fsyacc_immediateActions;
    gotos = _fsyacc_gotos;
    sparseGotoTableRowOffsets = _fsyacc_sparseGotoTableRowOffsets;
    tagOfErrorTerminal = _fsyacc_tagOfErrorTerminal;
    parseError = (fun (ctxt:FSharp.Text.Parsing.ParseErrorContext<_>) -> 
                              match parse_error_rich with 
                              | Some f -> f ctxt
                              | None -> parse_error ctxt.Message);
    numTerminals = 11;
    productionToNonTerminalTable = _fsyacc_productionToNonTerminalTable  }
let engine lexer lexbuf startState = tables.Interpret(lexer, lexbuf, startState)
let start lexer lexbuf : LispExpressionDefs.Expression =
    engine lexer lexbuf 0 :?> _
