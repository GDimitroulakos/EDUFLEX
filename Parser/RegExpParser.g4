parser grammar RegExpParser;

options { tokenVocab = RegExpLexer; }


/*
 * Parser Rules
 Perl grammar specification http://www.cs.sfu.ca/~cameron/Teaching/384/99-3/regexp-plg.html
							http://stackoverflow.com/questions/265457/regex-grammar
 */

lexerDescription: regexp_statement+
				  ;

regexp_statement : ID? COLON regexp  action_code? SEMICOLON
				 ;

action_code : STARTCODEANCHOR CODE ENDCODEANCHOR
			 ; 

regexp : regexp ALTERNATION regexp_conc   #regexp_alternation
	   | regexp_conc						  #regexp_alternation_other
	   ;

regexp_conc :  regexp_conc regexp_closure  #regexp_concatenation
			|  regexp_closure				 #regexp_concatenation_other
			;

regexp_closure : regexp_closure quantifier	#regexp_clos
			   | regexp_basic				#regexp_clos_other
			   ;


regexp_basic : LP regexp RP		#regexpbasic_parenthesized
			 | set				#regexpbasic_set
			 | ANY_EXCEPT_EOL	#regexpbasic_any
			 | char				#regexpbasic_char
			 | ENDOFLINE		#regexpbasic_eol
			 | STARTOFLINE		#regexpbasic_sol
			 | assertions		#regexpbasic_assertions
			 | STRING			#regexpbasic_string
			 ;

assertions :   LP QMARK EQUAL regexp  RP		#assertion_fwdpos// forward positive predicate
			 | LP QMARK EXMARK regexp  RP		#assertion_fwdneg // negative lookahead
			 | LP QMARK LESS EQUAL regexp RP	#assertion_bwdpos// positive lookbehind
			 | LP QMARK LESS EXMARK regexp RP	#assertion_bwdneg// negative lookbehind
		   ;

quantifier : QMARK								#quantifier_oneorzero
		   | ONEORMULTIPLE						#quantifier_oneormultiple
		   | ONEORMULTIPLE_NONGREEDY			#quantifier_oneormultipleNG
		   | NONEORMULTIPLE						#quantifier_noneormultiple
		   | NONEORMULTIPLE_NONGREEDY			#quantifier_noneormultipleNG
		   | finate_closure_range				#quantifier_range
		   ;

finate_closure_range : LCA NUMBER (COMMAC NUMBER? )?  RAC;


set : LB setitems RB	#setofitems
	| LBH setitems RB  #setofitems_negation
	;

setitems : setitem+
		 ;

setitem : range
		| setchar
		;

setchar : SET_LITERAL_CHARACTER
		 | CONTROL_CHARACTERS 
		 ;

range : setchar HYPTHEN setchar
		;

// The following rule describes all the characters that may appear literally
// in the body of a regural expression
char :	  ESCSLASH? LITERAL_CHARACTER
		| ESCSLASH? EXMARK
		| ESCSLASH? LESS
		| ESCSLASH? EQUAL
		| ESCSLASH ENDOFLINE
		| ESCSLASH STARTOFLINE
		| ESCSLASH STARTCODEANCHOR
		| ESCSLASH LP
		| ESCSLASH RP
		| ESCSLASH LB
		| ESCSLASH ANY_EXCEPT_EOL
		| ESCSLASH DIESIS
		| ESCSLASH ESCSLASH
		| ESCSLASH QMARK
		| ESCSLASH ONEORMULTIPLE
		| ESCSLASH NONEORMULTIPLE
		| ESCSLASH ALTERNATION
		;

