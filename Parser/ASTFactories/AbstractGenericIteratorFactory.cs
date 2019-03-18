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

    public interface IAbstractASTIteratorsFactory {

        CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsFlatten(CASTElement element);

        CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsFlattenEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);

        CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsContext(CASTElement element);

        CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsContextEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null);

    }

    public abstract class CASTAbstractGenericIteratorFactory : IAbstractASTIteratorsFactory {
        public virtual CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsFlatten(CASTElement element) {
            return new CASTElementDescentantsFlattenIterator(element);
        }

        public virtual CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsFlattenEvents(CASTElement element, 
            CASTGenericIteratorEvents events, object info=null) {
            return new CASTElementDescentantsFlattenEventIterator(element,events,info);
        }

        public virtual CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsContext(CASTElement element) {
            return new CASTElementDescentantsContextIterator(element);
        }

        public virtual CAbstractIterator<CASTElement> CreateIteratorASTElementDescentantsContextEvents(CASTElement element,
            CASTGenericIteratorEvents events, object info = null) {
            return new CASTElementDescentantsContextEventIterator(element, events, info);
        }
    }
}
