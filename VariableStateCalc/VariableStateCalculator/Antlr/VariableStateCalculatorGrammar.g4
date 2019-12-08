grammar VariableStateCalculatorGrammar;

/*
 * Parser Rules
 */

appDef : (usingDef)* namespaceDef EOF;

usingDef : 'using' (ID | FULL_QUALIFIED_ID) ';';

namespaceDef : 'namespace' (ID | FULL_QUALIFIED_ID) '{' rootClassDef '}';

namespaceName : ID | FULL_QUALIFIED_ID;

rootClassDef : ('public')? 'class' ID '{' (mainMethodDef evaluateMethodDef | evaluateMethodDef mainMethodDef) '}';

mainMethodDef : ('public')? 'static' 'void' 'Main' '(' ('string[]' ID)? ')' '{' mainMethodBodyEntry* '}';

mainMethodBodyEntry : ('System.Console.WriteLine' | 'Console.WriteLine') '(' 'Evaluate' '(' ( BOOL_VALUE (',' BOOL_VALUE)*)? ')' ')' ';';

evaluateMethodDef : ('public' | 'internal' | 'protected' | 'private')? 'static' 'int' 'Evaluate' '(' 'params' 'bool[]' ID ')' '{' evaluateBodyDef '}';

evaluateBodyDef : xVariableDef blockStatementDef+ 'return' 'x' ';';

xVariableDef : 'int' 'x' ('=' NUMBER)? ';';

blockDef : '{' blockStatementDef* '}';

blockStatementDef : blockDef | assignmentDef | ifDef;

assignmentDef : 'x' '=' NUMBER ';';

ifDef : 'if' '(' ID ('[' NUMBER ']')? ')' ifBodyDef;

ifBodyDef : blockDef | assignmentDef | ifDef;

/*
 * Lexer Rules
 */

BOOL_VALUE : 'true' | 'false';

NUMBER : ('0'..'9')+;

ID : ('A'..'Z' | 'a' .. 'z' | '_') ('A'..'Z' | 'a' .. 'z' | '0' .. '9' | '_')*;

FULL_QUALIFIED_ID : ID ('.' ID)+ ;

COMMENT : '/*' .*? '*/' -> channel(HIDDEN);

LINE_COMMENT : '//' .*? '\r'? '\n' -> channel(HIDDEN);

WS : [ \t\r\n\f]+ -> channel(HIDDEN) ;
