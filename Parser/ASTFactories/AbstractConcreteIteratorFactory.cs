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
using Parser.ASTEvents;
using Parser.ASTIterator;


namespace Parser.ASTFactories {

    public interface IASTAbstractConcreteIteratorFactory {
        //non-terminals start here
        #region LexerDescription iterator

        CAbstractIterator<CASTElement> CreateLexerDescriptionIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateLexerDescriptionIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpStatement iterator

        CAbstractIterator<CASTElement> CreateRegexpStatementIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpStatementIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpAlternation iterator

        CAbstractIterator<CASTElement> CreateRegexpAlternationIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpAlternationIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpConcatenation iterator

        CAbstractIterator<CASTElement> CreateRegexpConcatenationIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpConcatenationIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpClosure iterator

        CAbstractIterator<CASTElement> CreateRegexpClosureIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpClosureIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region ActioCode iterator

        CAbstractIterator<CASTElement> CreateActionCodeIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateActionCodeIteratorEvents(CASTElement element, CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicParen iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicParenIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicParenIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicSet iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicSetIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicSetIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicAnyexcepteol iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicAnyexcepteolIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicAnyexcepteolIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicChar iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicCharIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicCharIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicEndofline iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicEndoflineIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicEndoflineIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicStartofline iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicStartoflineIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicStartoflineIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicAssertions iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicAssertionsIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicAssertionsIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region RegexpbasicString iterator

        CAbstractIterator<CASTElement> CreateRegexpbasicStringIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRegexpbasicStringIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region AssertionFwdpos iterator

        CAbstractIterator<CASTElement> CreateAssertionFwdposIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateAssertionFwdposIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region AssertionFwdneg iterator

        CAbstractIterator<CASTElement> CreateAssertionFwdnegIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateAssertionFwdnegIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region AssertionBwdpos iterator

        CAbstractIterator<CASTElement> CreateAssertionBwdposIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateAssertionBwdposIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region AssertionBwdneg iterator

        CAbstractIterator<CASTElement> CreateAssertionBwdnegIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateAssertionBwdnegIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region QuantifierQmark iterator

        CAbstractIterator<CASTElement> CreateQuantifierQmarkIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateQuantifierQmarkIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region QuantifierOneormultiple iterator

        CAbstractIterator<CASTElement> CreateQuantifierOneormultipleIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateQuantifierOneormultipleIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region QuantifierOneormultipleng iterator

        CAbstractIterator<CASTElement> CreateQuantifierOneormultiplengIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateQuantifierOneormultiplengIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region QuantifierNoneormultiple iterator

        CAbstractIterator<CASTElement> CreateQuantifierNoneormultipleIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateQuantifierNoneormultipleIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region QuantifierNoneormultipleng iterator

        CAbstractIterator<CASTElement> CreateQuantifierNoneormultiplengIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateQuantifierNoneormultiplengIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region QuantifierFinateclosurerange iterator

        CAbstractIterator<CASTElement> CreateQuantifierFinateclosurerangeIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateQuantifierFinateclosurerangeIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion
        #region Range iterator

        CAbstractIterator<CASTElement> CreateRangeIterator(CASTElement element);

        CAbstractIterator<CASTElement> CreateRangeIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);
        #endregion

        //non-terminals end here
    }


    public class CASTAbstractConcreteIteratorFactory : CASTAbstractGenericIteratorFactory,
        IASTAbstractConcreteIteratorFactory {
        
        #region RegexpStatement iterator
        public virtual CAbstractIterator<CASTElement> CreateRegexpStatementIterator(CASTElement element)
        {
            return CreateIteratorASTElementDescentantsFlatten(element);
        }

        public virtual CAbstractIterator<CASTElement> CreateRegexpStatementIteratorEvents(CASTElement element,
        CASTGenericIteratorEvents events, object info = null)
        {
            return CreateIteratorASTElementDescentantsFlattenEvents(element, events, info);
        }
        #endregion
        #region RegexpAlternation iterator
        public CAbstractIterator<CASTElement> CreateRegexpAlternationIterator(CASTElement element)
        {
            return CreateIteratorASTElementDescentantsFlatten(element);
        }

        public CAbstractIterator<CASTElement> CreateRegexpAlternationIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            return CreateIteratorASTElementDescentantsFlattenEvents(element, events, info);
        }
        #endregion
        #region RegexpConcatenation iterator
        public CAbstractIterator<CASTElement> CreateRegexpConcatenationIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpConcatenationIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RegexpClosure iterator
        public CAbstractIterator<CASTElement> CreateRegexpClosureIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpClosureIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region ActionCode iterator
        public CAbstractIterator<CASTElement> CreateActionCodeIterator(CASTElement element) {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateActionCodeIteratorEvents(CASTElement element, CASTGenericIteratorEvents events, object info=null) {
            throw new NotImplementedException();
        }

        #endregion
        #region RegexpbasicParen iterator

        public CAbstractIterator<CASTElement> CreateRegexpbasicParenIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateClosureRangeIterator(CASTElement element) {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateClosureRangeIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicParenIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null) {
            throw new NotImplementedException();
        }




        #endregion
        #region RegexpbasicSet iterator

        public CAbstractIterator<CASTElement> CreateRegexpbasicSetIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicSetIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RegexpbasicAnyexcepteol iterator

        public CAbstractIterator<CASTElement> CreateRegexpbasicAnyexcepteolIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicAnyexcepteolIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RegexpbasicChar iterator

        public CAbstractIterator<CASTElement> CreateRegexpbasicCharIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicCharIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RegexpbasicEndofline iterator

        public CAbstractIterator<CASTElement> CreateRegexpbasicEndoflineIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicEndoflineIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RegexpbasicStartofline iterator

        public CAbstractIterator<CASTElement> CreateRegexpbasicStartoflineIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicStartoflineIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RegexpbasicAssertions iterator

        public CAbstractIterator<CASTElement> CreateRegexpbasicAssertionsIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicAssertionsIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RegexpbasicString iterator
        public CAbstractIterator<CASTElement> CreateRegexpbasicStringIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRegexpbasicStringIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region AssertionFwdpos iterator

        public CAbstractIterator<CASTElement> CreateAssertionFwdposIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateAssertionFwdposIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region AssertionFwdneg iterator

        public CAbstractIterator<CASTElement> CreateAssertionFwdnegIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateAssertionFwdnegIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region AssertionBwdpos iterator

        public CAbstractIterator<CASTElement> CreateAssertionBwdposIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateAssertionBwdposIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region AssertionBwdneg iterator

        public CAbstractIterator<CASTElement> CreateAssertionBwdnegIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateAssertionBwdnegIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region QuantifierQmark iterator

        public CAbstractIterator<CASTElement> CreateQuantifierQmarkIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateQuantifierQmarkIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region QuantifierOneormultiple iterator

        public CAbstractIterator<CASTElement> CreateQuantifierOneormultipleIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateQuantifierOneormultipleIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region QuantifierOneormultipleng iterator

        public CAbstractIterator<CASTElement> CreateQuantifierOneormultiplengIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateQuantifierOneormultiplengIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region QuantifierNoneormultiple iterator

        public CAbstractIterator<CASTElement> CreateQuantifierNoneormultipleIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateQuantifierNoneormultipleIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region QuantifierNoneormultipleng iterator

        public CAbstractIterator<CASTElement> CreateQuantifierNoneormultiplengIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateQuantifierNoneormultiplengIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region QuantifierFinateclosurerange iterator

        public CAbstractIterator<CASTElement> CreateQuantifierFinateclosurerangeIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateQuantifierFinateclosurerangeIteratorEvents(CASTElement element, CASTGenericIteratorEvents events,
            object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Range iterator

        public CAbstractIterator<CASTElement> CreateRangeIterator(CASTElement element)
        {
            throw new NotImplementedException();
        }

        public CAbstractIterator<CASTElement> CreateRangeIteratorEvents(CASTElement element, CASTGenericIteratorEvents events, object info = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region LexerDescription iterator
        public virtual  CAbstractIterator<CASTElement> CreateLexerDescriptionIterator(CASTElement element) {
            return CreateIteratorASTElementDescentantsFlatten(element);
        }

        public virtual  CAbstractIterator<CASTElement> CreateLexerDescriptionIteratorEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null) {
            return CreateIteratorASTElementDescentantsFlattenEvents(element, events, info);
        }
        #endregion

    }


}

