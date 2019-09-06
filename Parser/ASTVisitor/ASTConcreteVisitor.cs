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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Parser.ASTVisitor {

    public interface IASTAbstractConcreteVisitor<Return> {
        Return VisitLexerDescription(CASTElement currentNode);

        Return VisitRegexpStatement(CASTElement currentNode);

        Return VisitRegexpAlternation(CASTElement currentNode);

        Return VisitRegexpConcatenation(CASTElement currentNode);

        Return VisitRegexpClosure(CASTElement currentNode);

        Return VisitRegexpbasicParen(CASTElement currentNode);

        Return VisitClosureRange(CASTElement currentNode);

        Return VisitRegexpbasicSet(CASTElement currentNode);

        Return VisitRegexpbasicAnyexcepteol(CASTElement currentNode);

        Return VisitRegexpbasicChar(CASTElement currentNode);

        Return VisitRegexpbasicEndofline(CASTElement currentNode);

        Return VisitRegexpbasicStartofline(CASTElement currentNode);

        Return VisitRegexpbasicAssertions(CASTElement currentNode);

        Return VisitRegexpbasicString(CASTElement currentNode);

        Return VisitRegexpID(CASTElement currentNode);

        Return VisitAssertionFwdpos(CASTElement currentNode);

        Return VisitAssertionFwdneg(CASTElement currentNode);

        Return VisitAssertionBwdpos(CASTElement currentNode);

        Return VisitAssertionBwdneg(CASTElement currentNode);

        Return VisitRange(CASTElement currentNode);
        Return VisitActionCode(CASTElement currentNode);
    }

    public class CASTAbstractConcreteVisitor<Return> : CASTAbstractVisitor<Return>, IASTAbstractConcreteVisitor<Return> {

        public virtual Return VisitLexerDescription(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }
        public virtual Return VisitRegexpStatement(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }
        public virtual Return VisitRegexpAlternation(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpConcatenation(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }
        public virtual Return VisitAction_code(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }
        public virtual Return VisitRegexpClosure(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitClosureRange(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicParen(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicSet(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicAnyexcepteol(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicChar(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicEndofline(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicStartofline(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicAssertions(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpbasicString(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitRegexpID(CASTElement currentNode) {
            return VisitTerminal(currentNode);
        }

        public virtual Return VisitAssertionFwdpos(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitAssertionFwdneg(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitAssertionBwdpos(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitAssertionBwdneg(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        
        public virtual Return VisitRange(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        public virtual Return VisitActionCode(CASTElement currentNode) {
            return base.VisitChildren(currentNode);
        }

        internal void VisitRegexpbasicSet_negation(CASTElement currentNode) {
            throw new NotImplementedException();
        }
    }
}
