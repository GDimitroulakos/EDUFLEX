using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Parser {
    class ASTGeneration : RegExpParserBaseVisitor<int> {
        private CASTComposite m_ASTRoot = null;

        private Stack<CASTComposite> m_parents = new Stack<CASTComposite>();

        private Stack<ContextType> m_currentContext = new Stack<ContextType>();

        public CASTComposite M_ASTRoot {
            get { return m_ASTRoot; }
            private set { m_ASTRoot = value; }
        }

        private ITerminalNode GetTerminalNode(ParserRuleContext node, IToken terminal) {

            for (int i = 0; i < node.ChildCount; i++) {
                ITerminalNode child = node.GetChild(i) as ITerminalNode;
                if (child != null) {
                    if (child.Symbol == terminal) {
                        return child;
                    }
                }
            }
            return null;
        }

        private int VisitTerminalInContext(ParserRuleContext tokenParent, IToken token, ContextType contextType) {
            int res;
            m_currentContext.Push(contextType);
            res = Visit(GetTerminalNode(tokenParent, token));
            m_currentContext.Pop();
            return res;
        }

        private int VisitElementInContext(IParseTree element, ContextType contextType) {
            int res;
            m_currentContext.Push(contextType);
            res = Visit(element);
            m_currentContext.Pop();
            return res;
        }

        private int VisitElementsInContext(IEnumerable<IParseTree> context, ContextType contextType) {
            int res = 0;
            m_currentContext.Push(contextType);
            foreach (ParserRuleContext elem in context) {
                res = Visit(elem);
            }
            m_currentContext.Pop();
            return res;
        }
        /* 
         /* 25 
         /* 27 #1# CT_RANGE_MIN = 27, CT_RANGE_MAX = 28,
             CT_NA*/
        public override int VisitLexerDescription(RegExpParser.LexerDescriptionContext context) {

            // Create new element
            CASTComposite newNode = new CLexerDescription();
            //assign it to root
            M_ASTRoot = newNode;

            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementsInContext(context.regexp_statement(), ContextType.CT_LEXERDESCRIPTION_BODY);

            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitRegexp_statement(RegExpParser.Regexp_statementContext context) {


            // 1. Create new AST node
            CRegexpStatement newNode = new CRegexpStatement(m_parents.Peek());
            var i = context.SourceInterval;

            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.ID(), ContextType.CT_REGEXPSTATEMENT_TOKENNAME);
            VisitElementInContext(context.regexp(), ContextType.CT_REGEXPSTATEMENT_REGEXP);
            if (context.action_code() != null) {
                VisitElementInContext(context.action_code(), ContextType.CT_REGEXPSTATEMENT_ACTIONCODE);
            }

            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitRegexp_alternation(RegExpParser.Regexp_alternationContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CRegexpAlternation(m_parents.Peek());

            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.regexp(), ContextType.CT_REGEXPALTERNATION_TERMS);
            VisitElementInContext(context.regexp_conc(), ContextType.CT_REGEXPALTERNATION_TERMS);
            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitRegexp_concatenation(RegExpParser.Regexp_concatenationContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CRegexpConcatenation(m_parents.Peek());

            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            // Update parents stack
            m_parents.Push(newNode);
            // VISIT CHILDREN
            VisitElementInContext(context.regexp_conc(), ContextType.CT_REGEXPCONCATENATION_TERMS);
            VisitElementInContext(context.regexp_closure(), ContextType.CT_REGEXPCONCATENATION_TERMS);
            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }
        public override int VisitRegexp_clos(RegExpParser.Regexp_closContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CRegexpClosure(m_parents.Peek());

            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            // Update parents stack
            m_parents.Push(newNode);
            // VISIT CHILDREN
            VisitElementInContext(context.regexp_closure(), ContextType.CT_REGEXPCLOSURE_REGEXP);
            VisitElementInContext(context.quantifier(), ContextType.CT_REGEXPCLOSURE_QUANTIFIER);

            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitFinate_closure_range(RegExpParser.Finate_closure_rangeContext context) {

            // 1. Create new AST node
            CRegexpClosure parent = m_parents.Peek() as CRegexpClosure;

            // Set closure type
            parent.SetClosureType(CRegexpClosure.ClosureType.CLT_FINITECLOSURE);

            parent.SetClosureMultiplicity(Int32.Parse(context.NUMBER(0).GetText()));

            if (context.NUMBER().Length > 1) {
                parent.SetClosureMultiplicity(Int32.Parse(context.NUMBER(1).GetText()), true);
            }
            return 0;
        }
        public override int VisitRegexpbasic_parenthesized(RegExpParser.Regexpbasic_parenthesizedContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CRegexpbasicParen(m_parents.Peek());

            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            // Update parents stack
            m_parents.Push(newNode);
            // VISIT CHILDREN
            VisitElementInContext(context.regexp(), ContextType.CT_RGEXPBASIC_PAREN);

            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_any(RegExpParser.Regexpbasic_anyContext context) {
            // 1. Create new AST node
            CASTLeaf<CASTElement> newNode = new CRegexpbasicAnyexcepteol(m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // VISIT CHILDREN
            VisitElementInContext(context.ANY_EXCEPT_EOL(), ContextType.CT_REGEXPBASIC_ANYEXCEPTEOL);

            // POSTORDER ACTIONS

            // Update parents stack
            //None
            return 0;
        }
        /*public override int VisitRegexpbasic_char(RegExpParser.Regexpbasic_charContext context) {
            // 1. Create new AST node
            CASTLeaf<CASTElement> newNode = new CRegexpbasicChar(context.GetText(), m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // VISIT CHILDREN
            VisitElementInContext(context.@char(), ContextType.CT_REGEXPBASIC_CHAR);

            // POSTORDER ACTIONS

            // Update parents stack
            //None

            return 0;
        }*/
        public override int VisitRegexpbasic_eol(RegExpParser.Regexpbasic_eolContext context) {
            // 1. Create new AST node
            CASTLeaf<CASTElement> newNode = new CRegexpbasicEndofline(m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // VISIT CHILDREN
            VisitElementInContext(context.ENDOFLINE(), ContextType.CT_REGEXPBASIC_ENDOFLINE);
            // POSTORDER ACTIONS
            //None
            return 0;
        }
        public override int VisitRegexpbasic_sol(RegExpParser.Regexpbasic_solContext context) {
            // 1. Create new AST node
            CASTLeaf<CASTElement> newNode = new CRegexpbasicStartofline(m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // VISIT CHILDREN
            VisitElementInContext(context.STARTOFLINE(), ContextType.CT_REGEXPBASIC_STARTOFLINE);

            // POSTORDER ACTIONS

            // Update parents stack
            //None
            return 0;
        }
        public override int VisitRegexpbasic_string(RegExpParser.Regexpbasic_stringContext context) {
            // 1. Create new AST node
            string str = context.STRING().GetText();
            CASTLeaf<string> newNode = new CRegexpbasicString(str.Substring(0,str.Length-1), m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // VISIT CHILDREN
            if (context.STRING() != null) {
                VisitElementInContext(context.STRING(), ContextType.CT_REGEXPBASIC_STRING);
            }
            // POSTORDER ACTIONS
            //None

            return 0;
        }

        public override int VisitAssertion_fwdpos(RegExpParser.Assertion_fwdposContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CAssertionFwdpos(m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.regexp(), ContextType.CT_ASSERTION_FWDPOS);
            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitAssertion_fwdneg(RegExpParser.Assertion_fwdnegContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CAssertionFwdneg(m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.regexp(), ContextType.CT_ASSERTION_FWDNEG);
            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitAssertion_bwdpos(RegExpParser.Assertion_bwdposContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CAssertionBwdpos(m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.regexp(), ContextType.CT_ASSERTION_BWDPOS);
            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitAssertion_bwdneg(RegExpParser.Assertion_bwdnegContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CAssertionBwdneg(m_parents.Peek());
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.regexp(), ContextType.CT_ASSERTION_BWDNEG);
            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitQuantifier_oneorzero(RegExpParser.Quantifier_oneorzeroContext context) {
            CRegexpClosure parent = m_parents.Peek() as CRegexpClosure;

            if (parent != null) {
                parent.SetClosureType(CRegexpClosure.ClosureType.CLT_ONEORZERO);
                parent.SetClosureMultiplicity(0);
                parent.SetClosureMultiplicity(1, true);
            }
            else {
                throw new Exception("Invalid parent type in VisitQuantifier_oneorzero");
            }

            return 0;
        }

        public override int VisitQuantifier_oneormultiple(RegExpParser.Quantifier_oneormultipleContext context) {
            CRegexpClosure parent = m_parents.Peek() as CRegexpClosure;

            if (parent != null) {
                parent.SetClosureType(CRegexpClosure.ClosureType.CLT_ONEORMULTIPLE);
                parent.SetClosureMultiplicity(1);
                parent.SetClosureMultiplicity(Int32.MaxValue, true);
            }
            else {
                throw new Exception("Invalid parent type in VisitQuantifier_oneormultiple");
            }

            return 0;
        }

        public override int VisitQuantifier_oneormultipleNG(RegExpParser.Quantifier_oneormultipleNGContext context) {
            CRegexpClosure parent = m_parents.Peek() as CRegexpClosure;

            if (parent != null) {
                parent.SetClosureType(CRegexpClosure.ClosureType.CLT_ONEORMULTIPLE_NONGREEDY);
                parent.SetClosureMultiplicity(1);
                parent.SetClosureMultiplicity(Int32.MaxValue, true);
            }
            else {
                throw new Exception("Invalid parent type in VisitQuantifier_oneormultipleNG");
            }

            return 0;
        }

        public override int VisitQuantifier_noneormultiple(RegExpParser.Quantifier_noneormultipleContext context) {
            CRegexpClosure parent = m_parents.Peek() as CRegexpClosure;

            if (parent != null) {
                parent.SetClosureType(CRegexpClosure.ClosureType.CLT_NONEORMULTIPLE);
                parent.SetClosureMultiplicity(0);
                parent.SetClosureMultiplicity(Int32.MaxValue, true);
            }
            else {
                throw new Exception("Invalid parent type in VisitQuantifier_noneormultiple");
            }

            return 0;
        }

        public override int VisitQuantifier_noneormultipleNG(RegExpParser.Quantifier_noneormultipleNGContext context) {
            CRegexpClosure parent = m_parents.Peek() as CRegexpClosure;

            if (parent != null) {
                parent.SetClosureType(CRegexpClosure.ClosureType.CLT_ONEORMULTIPLE_NONGREEDY);
                parent.SetClosureMultiplicity(0);
                parent.SetClosureMultiplicity(Int32.MaxValue, true);
            }
            else {
                throw new Exception("Invalid parent type in VisitQuantifier_noneormultipleNG");
            }

            return 0;
        }
        
        public override int VisitSetofitems(RegExpParser.SetofitemsContext context) {
            // 1. Create new AST node
            CRegexpbasicSet newNode = new CRegexpbasicSet(m_parents.Peek(),false);
            
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());
            // Update parents stack
            m_parents.Push(newNode);
            // VISIT CHILDREN
            VisitElementInContext(context.setitems(), ContextType.CT_REGEXPBASIC_SET);

            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitSetofitems_negation(RegExpParser.Setofitems_negationContext context) {
            // 1. Create new AST node
            CASTComposite newNode = new CRegexpbasicSet(m_parents.Peek(),true);

            
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.setitems(), ContextType.CT_REGEXPBASIC_SET);

            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();

            return 0;
        }

        public override int VisitAction_code(RegExpParser.Action_codeContext context) {
            CASTComposite newNode = new CActionCode(m_parents.Peek());
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            // Update parents stack
            m_parents.Push(newNode);

            // VISIT CHILDREN
            VisitElementInContext(context.CODE(), ContextType.CT_ACTIONCODE);

            // POSTORDER ACTIONS

            // Update parents stack
            m_parents.Pop();
            return 0;
        }

        public override int VisitRange(RegExpParser.RangeContext context) {
            CRegexpbasicSet parent = m_parents.Peek() as CRegexpbasicSet;
            
            // 1. Create new AST node
           CRange newNode = new CRange(m_parents.Peek());
            
            // Add new element to the parent's descentants
            m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

            //Push father
            m_parents.Push(newNode);
            // VISIT CHILDREN
           VisitElementInContext(context.setchar(0), ContextType.CT_RANGE_MIN);
           VisitElementInContext(context.setchar(1), ContextType.CT_RANGE_MAX);
           
            // POSTORDER ACTIONS
            // Create range object
            newNode.Update();
            //Parent takes the next range
            parent.InsertRange(newNode.MRange);

            // Update parents stack
            m_parents.Pop();

            return 0;
        }
        public override int VisitTerminal(ITerminalNode node)
        {
            CASTComposite parent = m_parents.Peek();
           
            switch (node.Symbol.Type){
                case RegExpLexer.CONTROL_CHARACTERS:
                case RegExpLexer.SET_LITERAL_CHARACTER:
                case RegExpLexer.LITERAL_CHARACTER:
                    CRegexpbasicChar newNode = new CRegexpbasicChar(node.GetText(),parent);

                    // Add new element to the parent's descentants
                    m_parents.Peek().AddChild(newNode, m_currentContext.Peek());

                    if (parent is CRegexpbasicSet){
                        switch (node.Symbol.Type){
                            case RegExpLexer.CONTROL_CHARACTERS:
                            case RegExpLexer.SET_LITERAL_CHARACTER:
                                CRegexpbasicSet set = parent as CRegexpbasicSet;
                                set.InsertChar(node.GetText()[0]);
                                break;
                        }
                    }

                    break;
                case RegExpLexer.ID:
                    if (m_currentContext.Peek() == ContextType.CT_REGEXPSTATEMENT_TOKENNAME) {
                        CRegexpID id = new CRegexpID(node.GetText(),parent);
                        ((CRegexpStatement) m_parents.Peek()).M_StatementID = node.GetText();
                        m_parents.Peek().AddChild(id, m_currentContext.Peek());
                    }
                    break;
            }
/*
            if (parent is CRange){
                switch (node.Symbol.Type){
                    case RegExpLexer.CONTROL_CHARACTERS:
                    case RegExpLexer.SET_LITERAL_CHARACTER:


                        CRange range = parent as CRange;
                        switch (m_currentContext.Peek()){
                            case ContextType.CT_RANGE_MIN:
                                range.MLower = node.GetText()[0];
                                break;
                            case ContextType.CT_RANGE_MAX:
                                range.MUpper = node.GetText()[0];
                                break;
                        }
                        break;
                }
            }
            else if (parent is CRegexpbasicSet){
                switch (node.Symbol.Type){
                    case RegExpLexer.CONTROL_CHARACTERS:
                    case RegExpLexer.SET_LITERAL_CHARACTER:
                        CRegexpbasicSet set = parent as CRegexpbasicSet;
                        set.InsertChar(node.GetText()[0]);
                        break;
                }
            }*/

            return 0;
        }
    }
}
