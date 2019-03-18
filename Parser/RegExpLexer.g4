lexer grammar RegExpLexer;

@header {	using System;
			using System.IO; }

@lexer::members{
				  public static bool guard = true;
				  public static bool guard_string = true;
				  public static int LBindex = 0;
				  public static bool escaped = false;
}
/*
 * Lexer Rules
 */

ID : [a-zA-Z][a-zA-Z0-9_]*;
COLON : ':' -> mode(REGEXP);
SEMICOLON : ';'; 
WS_DEFAULT : [ \u0009\u000A\u000D]+ ->skip;

 mode REGEXP;
LB : '[' -> mode(SET);
LBH : '[^' -> mode(SET); //left bracket hyphen
ENDOFLINE : '$';
STARTOFLINE : '^';
STARTCODEANCHOR : '{' {if(_input.La(-2)!=92){
							Mode(ACTIONCODE);
					   }
					RegExpLexer.escaped=false;};
LP : '(';
RP : ')';
QMARK : '?';
DIESIS :'#';
ANY_EXCEPT_EOL : '.';
SEMI : {LBindex =InputStream.Index; } ';' {InputStream.Seek(LBindex);
											Mode(RegExpLexer.DefaultMode);} ->skip;
ONEORMULTIPLE: '+';
NONEORMULTIPLE: '*';
ONEORMULTIPLE_NONGREEDY: '+?';
NONEORMULTIPLE_NONGREEDY: '*?';
ALTERNATION : '|';
ESCSLASH : '\\'  {RegExpLexer.escaped=true;}  ;
EXMARK : '!';
LESS : '<';
EQUAL : '=';
WS_REGEXP : [ \u0009\u000A\u000D]+ ->skip;
LITERAL_CHARACTER : ~[*+?|{[()^$.#;\\!<=];




//ONEORNONE : '?';






//http://stackoverflow.com/questions/42318810/matching-the-finite-closure-pattern-x-y-of-regular-expressions
CLOSURE_FLAG :  {guard}?  {LBindex =InputStream.Index; }
						 '~{' INTEGER ( ',' INTEGER? )? '}'  { RegExpLexer.guard = false;
																// Go back to the opening brace
																InputStream.Seek(LBindex);
																Console.WriteLine("Enter Closure Mode");
																Mode(CLOSURE);
															} -> skip
;

STRING_FLAG : {guard_string}? 
			  '\'' {LBindex =InputStream.Index; } .*? '\''  
							   { RegExpLexer.guard_string = false;
								// Go back to the opening brace
								InputStream.Seek(LBindex);
								Console.WriteLine("Enter Literal String Mode");
								Mode(STRINGMOD);
															} -> skip
			  ;

mode ACTIONCODE;
CODE : ~[}]*; 
ENDCODEANCHOR : '}' -> mode(DEFAULT_MODE);


mode CLOSURE;
LCA : '~{';  // Left Closure Anchor
RAC : '}' { RegExpLexer.guard = true; Mode(RegExpLexer.REGEXP); Console.WriteLine("Enter Default Mode"); };
COMMAC : ',' ;
NUMBER : INT ;
WS_CLOSURE : [ \u0009\u000A\u000D]+ ->skip;

mode SET;
						//   -    \      ]    
SET_LITERAL_CHARACTER : ~[\u002D\u005C\u005D]; 
HYPTHEN : '-';
RB : ']' ->mode(REGEXP);
CONTROL_CHARACTERS : '\\' [\u002D\u005D\u005Ctabfnrsv0];

INTEGER : INT;
		//    \t    \n    \r
WS_SET : [ \u0009\u000A\u000D]+ ->skip;

fragment INT : [1-9][0-9]*;

mode STRINGMOD;
STRING :  .*? '\'' {    RegExpLexer.guard_string = true;
						if ( _token !=null )						 
						  Console.WriteLine(_token.Text);
				   }->mode(REGEXP) ;
