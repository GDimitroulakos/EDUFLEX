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
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ASTIterator {
    /// <summary>
    /// <c>AbstractIterator</c> provides the general interface of the iterator pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CAbstractIterator<T> {

        protected T m_item;

        /// <summary>
        /// Get the current item pointed by the iterator
        /// </summary>
        /// <value>
        /// The item
        /// </value>
        public virtual T M_item {
            get { return m_item; }
        }

        /// <summary>
        /// This method provides configurable initialization of the iterator.
        /// The default version works exactly as Begin(). It can be overriden
        /// by a subclass if a configurable initialization is necessary
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public virtual T Begin(object param=null) {
            return Begin();
        }

        /// <summary>
        /// This method provides configurable initialization of the iterator.
        /// The default version works exactly as ItBegin(). It can be overriden
        /// by a subclass if a configurable initialization is necessary
        /// </summary>
        /// <param name="param">The parameter.</param>
        public virtual void ItBegin(object param = null) {
            ItBegin();
        }

        /// <summary>
        /// Initializes the iterator an returns the first element.
        /// </summary>
        /// <returns></returns>
        public abstract T Begin();

        /// <summary>
        /// Assures iteration inside the loop bounds
        /// </summary>
        /// <returns></returns>
        public abstract bool End();

        /// <summary>
        /// Increases the iterator and get the element it points (after increament).
        /// </summary>
        /// <returns></returns>
        public abstract T Next();

        /// <summary>
        /// Initializes the iterator
        /// </summary>
        public abstract void ItBegin();

        /// <summary>
        /// Assures iteration inside the loop bounds
        /// </summary>
        /// <returns></returns>
        public abstract bool ItEnd();

        /// <summary>
        /// Increases the iterator to the next item
        /// </summary>
        public abstract void ITNext();
    }
}
