/*
MIT License

Copyright(c) [2016] [Grigoris Dimitroulakos]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;

namespace Parser {
    class PTPrinter : RegExpParserBaseVisitor<int> {
        private Stack<string> m_PTPath;

        private static int ms_ASTElementCounter = 0;

        private StreamWriter m_outputStream;

        private string m_outputFile;

        public PTPrinter(string file,bool callGraphViz=true) {
            m_PTPath = new Stack<string>();
            m_outputFile = Path.GetFileNameWithoutExtension(file) + ".dot";
            try
            {
                m_outputStream = new StreamWriter(m_outputFile);
            }
            catch(UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public override int VisitLexerDescription(RegExpParser.LexerDescriptionContext context)
        {
            // PREORDER ACTIONS
            m_outputStream.WriteLine("digraph " + Path.GetFileNameWithoutExtension(m_outputFile) + "{\n");
            string label = "LexerDescription_" + ms_ASTElementCounter.ToString();
            //m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitLexerDescription(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();



            m_outputStream.WriteLine("}");
            m_outputStream.Close();

            if (true)
            {
                Process process = new Process();
                // Configure the process using the StartInfo properties.
                process.StartInfo.FileName = "dot.exe";
                process.StartInfo.Arguments = "-Tgif " + m_outputFile + " -o" +
                    Path.GetFileNameWithoutExtension(m_outputFile) + ".gif";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();// Waits here for the process to exit.
            }

            return 0;
        }

        public override int VisitRegexpbasic_sol(RegExpParser.Regexpbasic_solContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_sol_" + ms_ASTElementCounter.ToString();

            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_sol(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_char(RegExpParser.Regexpbasic_charContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_char_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_char(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_any(RegExpParser.Regexpbasic_anyContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_any_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_any(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_string(RegExpParser.Regexpbasic_stringContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_string_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_string(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_set(RegExpParser.Regexpbasic_setContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_set_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_set(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_eol(RegExpParser.Regexpbasic_eolContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_eol_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_eol(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_parenthesized(RegExpParser.Regexpbasic_parenthesizedContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_parenthesized_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_parenthesized(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexpbasic_assertions(RegExpParser.Regexpbasic_assertionsContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexpbasic_assertions_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexpbasic_assertions(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_alternation(RegExpParser.Regexp_alternationContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_alternation_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_alternation(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_alternation_other(RegExpParser.Regexp_alternation_otherContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_alternation_other_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_alternation_other(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitSetofitems(RegExpParser.SetofitemsContext context)
        {
            // PREORDER ACTIONS
            string label = "Setofitems_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitSetofitems(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitSetofitems_negation(RegExpParser.Setofitems_negationContext context)
        {
            // PREORDER ACTIONS
            string label = "Setofitems_negation_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitSetofitems_negation(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitQuantifier_oneorzero(RegExpParser.Quantifier_oneorzeroContext context)
        {
            // PREORDER ACTIONS
            string label = "Quantifier_oneorzero_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitQuantifier_oneorzero(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitQuantifier_noneormultiple(RegExpParser.Quantifier_noneormultipleContext context)
        {
            // PREORDER ACTIONS
            string label = "Quantifier_noneormultiple_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitQuantifier_noneormultiple(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitQuantifier_noneormultipleNG(RegExpParser.Quantifier_noneormultipleNGContext context)
        {
            // PREORDER ACTIONS
            string label = "Quantifier_noneormultipleNG_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitQuantifier_noneormultipleNG(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitQuantifier_range(RegExpParser.Quantifier_rangeContext context)
        {
            // PREORDER ACTIONS
            string label = "Quantifier_range_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitQuantifier_range(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitQuantifier_oneormultiple(RegExpParser.Quantifier_oneormultipleContext context)
        {
            // PREORDER ACTIONS
            string label = "Quantifier_oneormultiple_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitQuantifier_oneormultiple(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitQuantifier_oneormultipleNG(RegExpParser.Quantifier_oneormultipleNGContext context)
        {
            // PREORDER ACTIONS
            string label = "Quantifier_oneormultipleNG_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitQuantifier_oneormultipleNG(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_concatenation_other(RegExpParser.Regexp_concatenation_otherContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_concatenation_other_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_concatenation_other(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_concatenation(RegExpParser.Regexp_concatenationContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_concatenation_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_concatenation(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_clos(RegExpParser.Regexp_closContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_clos_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_clos(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_clos_other(RegExpParser.Regexp_clos_otherContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_clos_other_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_clos_other(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitAssertion_fwdpos(RegExpParser.Assertion_fwdposContext context)
        {
            // PREORDER ACTIONS
            string label = "Assertion_fwdpos_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitAssertion_fwdpos(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitAssertion_fwdneg(RegExpParser.Assertion_fwdnegContext context)
        {
            // PREORDER ACTIONS
            string label = "Assertion_fwdneg_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitAssertion_fwdneg(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitAssertion_bwdneg(RegExpParser.Assertion_bwdnegContext context)
        {
            // PREORDER ACTIONS
            string label = "Assertion_bwdneg_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitAssertion_bwdneg(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitAssertion_bwdpos(RegExpParser.Assertion_bwdposContext context)
        {
            // PREORDER ACTIONS
            string label = "Assertion_bwdpos_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitAssertion_bwdpos(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_statement(RegExpParser.Regexp_statementContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_statement_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_statement(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitAction_code(RegExpParser.Action_codeContext context)
        {
            // PREORDER ACTIONS
            string label = "Action_code_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitAction_code(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp(RegExpParser.RegexpContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_conc(RegExpParser.Regexp_concContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_conc_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_conc(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_closure(RegExpParser.Regexp_closureContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_closure_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_closure(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRegexp_basic(RegExpParser.Regexp_basicContext context)
        {
            // PREORDER ACTIONS
            string label = "Regexp_basic_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRegexp_basic(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitAssertions(RegExpParser.AssertionsContext context)
        {
            // PREORDER ACTIONS
            string label = "Assertions_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitAssertions(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitQuantifier(RegExpParser.QuantifierContext context)
        {
            // PREORDER ACTIONS
            string label = "Quantifier_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitQuantifier(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitFinate_closure_range(RegExpParser.Finate_closure_rangeContext context)
        {
            // PREORDER ACTIONS
            string label = "Finate_closure_range_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitFinate_closure_range(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitSet(RegExpParser.SetContext context)
        {
            // PREORDER ACTIONS
            string label = "Set_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitSet(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitSetitems(RegExpParser.SetitemsContext context)
        {
            // PREORDER ACTIONS
            string label = "Setitems_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitSetitems(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitSetitem(RegExpParser.SetitemContext context)
        {
            // PREORDER ACTIONS
            string label = "Setitem_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitSetitem(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitSetchar(RegExpParser.SetcharContext context)
        {
            // PREORDER ACTIONS
            string label = "Setchar_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitSetchar(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitRange(RegExpParser.RangeContext context)
        {
            // PREORDER ACTIONS
            string label = "Range_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitRange(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitChar(RegExpParser.CharContext context)
        {
            // PREORDER ACTIONS
            string label = "Char_" + ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;
            m_PTPath.Push(label);

            base.VisitChar(context);

            // POSTORDER ACTIONS
            m_PTPath.Pop();

            return 0;
        }

        public override int VisitTerminal(ITerminalNode node) {
            // PREORDER ACTIONS
            string label = "<"+node.GetText()+">"+ ms_ASTElementCounter.ToString();
            m_outputStream.WriteLine("\"{0}\"->\"{1}\";", m_PTPath.Peek(), label);
            ms_ASTElementCounter++;

            // POSTORDER ACTIONS

            return 0;
        }
    }
}
