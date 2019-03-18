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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Parser.ASTVisitor;
using Parser;
using Parser.ASTEvents;
using Parser.ASTIterator;

namespace Parser.ASTIterator {


    /// <summary>
    /// CASTElementDescentantsContextIterator class refers to an iterator that iterates on the
    /// descendants of a given composite object that lie on a specific context. One overloaded
    /// <c>Begin</c> method takes the context as a parameter
    /// </summary>
    /// <seealso cref="CASTElement" />
    public class CASTElementDescentantsContextIterator : CAbstractIterator<CASTElement> {
        /// <summary>
        /// The composite node over which the iterator applies in one of its contexts
        /// </summary>
        private CASTComposite m_node;

        /// <summary>
        /// The context index
        /// </summary>
        private int m_contextIndex;


        /// <summary>
        /// The element index inside the context
        /// </summary>
        private int m_elementIndex;

        public CASTElementDescentantsContextIterator(CASTElement astNode) {
            m_node = astNode as CASTComposite;
            if (m_node == null) {
                throw new Exception("Iterator cannot iterate over leaf nodes");
            }
        }

        /// <summary>
        /// This method provides configurable initialization of the iterator.
        /// The default version works exactly as Begin(). It can be overriden
        /// by a subclass if a configurable initialization is necessary
        /// </summary>
        /// <param name="param">The context either as an integer index or as ContextType parameter </param>
        /// <returns>The first element in the context</returns>
        public override CASTElement Begin(object param = null) {
            m_elementIndex = 0;
            if (param is int) {
                m_contextIndex = (int) param;
            }
            else if (param is ContextType) {
                m_contextIndex = m_node.MapContextToIndex((ContextType) param);
            }
            else {
                return base.Begin(param);
            }

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            return m_item;

        }

        /// <summary>
        /// Initializes the iterator an returns the first element. This method iterates over the first context
        /// </summary>
        /// <returns></returns>
        public override CASTElement Begin() {
            m_elementIndex = 0;
            m_contextIndex = 0;
            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            return m_item;
        }

        /// <summary>
        /// Assures iteration inside the loop bounds
        /// </summary>
        /// <returns></returns>
        public override bool End() {
            if (m_item == null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increases the iterator and get the element it points (after increament).
        /// </summary>
        /// <returns></returns>
        public override CASTElement Next() {
            // Increase iterator
            m_elementIndex++;

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            return m_item;
        }


        /// <summary>
        /// This method provides configurable initialization of the iterator.
        /// The default version works exactly as ItBegin(). It can be overriden
        /// by a subclass if a configurable initialization is necessary
        /// </summary>
        /// <param name="param">The context either as an integer index or as ContextType parameter</param>
        public override void ItBegin(object param = null) {
            m_elementIndex = 0;
            if (param is int) {
                m_contextIndex = (int) param;
            }
            else if (param is ContextType) {
                m_contextIndex = m_node.MapContextToIndex((ContextType) param);
            }
            else {
                base.ItBegin(param);
            }

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);
        }

        /// <summary>
        /// Initializes the iterator
        /// </summary>
        public override void ItBegin() {
            m_elementIndex = 0;
            m_contextIndex = 0;
            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);
        }

        /// <summary>
        /// Assures iteration inside the loop bounds
        /// </summary>
        /// <returns></returns>
        public override bool ItEnd() {
            if (m_item == null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increases the iterator to the next item
        /// </summary>
        public override void ITNext() {
            // Increase iterator
            m_elementIndex++;

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);
        }
    }

    public class CASTElementDescentantsContextEventIterator : CAbstractIterator<CASTElement> {

        /// <summary>
        /// This class carries the events that are related to an AST structure traversal
        /// This class is specialized according to the type of node. That is, each different
        /// subclass of <c>CASTAbstractIteratorEvents</c> class describes the actions performed
        /// upon event raise in each type of AST node
        /// </summary>
        private CASTGenericIteratorEvents m_event;

        /// <summary>
        /// Node where its children are traversed
        /// </summary>
        private CASTComposite m_node;

        /// Current context under traversal
        private int m_contextIndex;

        /// Current element index
        private int m_elementIndex;

        // A flag that indicates whether the end of context is reached.
        // It is raised by the PrepareNext() and PrepareFirst() methods
        // and its lower immediately after the related event is raised
        private bool m_endOfContext = false;


        public CASTElementDescentantsContextEventIterator(CASTElement astNode,
            CASTGenericIteratorEvents events, object info=null) {
            m_event = events;
            m_node = astNode as CASTComposite;
            if (m_node == null) {
                throw new Exception("Iterator cannot iterate over leaf nodes");
            }
        }

        public override CASTElement Begin(object param = null) {
            int context = 0;

            // Initialize context from method parameter
            if (param != null) {
                if (param is int) {
                    context = (int)param;
                } else if (param is ContextType) {
                    context = m_node.MapContextToIndex((ContextType)param);
                } else {
                    Console.WriteLine("WARNING!!! Iterator initialization didn't work as expected");
                }
            }

            // Initialize Iterator
            PrepareFirst(context);

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            // Event Triggering
            if (m_item != null) {
                // Prepare event information
                CASTVisitorEventArgs args = new CASTVisitorEventArgs();

                // Raise OnContextEnter and OnNodeEnter events
                m_event.OnContextEnter(m_item, args);
                m_event.OnNodeEnter(m_item, args);
            }
            return base.Begin(param);
        }

        public override void ItBegin(object param = null) {
            Begin(param);
        }

        public override CASTElement Begin() {
            // Initialize iterator
            PrepareFirst();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            // Event Triggering
            if (m_item != null) {
                // Prepare event information
                CASTVisitorEventArgs args = new CASTVisitorEventArgs();

                // Raise OnContextEnter and OnNodeEnter events
                m_event.OnContextEnter(m_item, args);
                m_event.OnNodeEnter(m_item, args);
            }

            return m_item;
        }

        public override bool End() {
            if (m_item == null) {
                return true;
            }
            return false;
        }

        public override CASTElement Next() {
            //Event Triggering
            // Prepare event information
            CASTVisitorEventArgs args_prev = new CASTVisitorEventArgs();

            // Raise the Node Leave event
            m_event.OnNodeLeave(m_item, args_prev);

            // Raise the Node Leave event
            if (m_endOfContext) {
                m_event.OnContextLeave(m_item, args_prev);
                m_endOfContext = false;
            }

            // Increase iterator
            PrepareNext();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            //Event Triggering
            if (m_item != null) {
                // Prepare event information
                CASTVisitorEventArgs args_next = new CASTVisitorEventArgs();

                // Raise OnContextEnter and OnNodeEnter events
                m_event.OnNodeEnter(m_item, args_next);
            }
            return m_item;
        }

        public override void ItBegin() {
            Begin();
        }

        public override bool ItEnd() {
            return End();
        }

        public override void ITNext() {
            Next();
        }

        private void PrepareNext() {
            m_elementIndex++;

            if (m_elementIndex == m_node.GetNumberOfContextElements(m_contextIndex)) {
                // In case where the first element is also the last
                m_endOfContext = true;
            }
        }

        private void PrepareFirst(int contextIndex = 0) {

            // Initialize iterator
            m_contextIndex = contextIndex;
            m_elementIndex = 0;

            if (m_elementIndex == m_node.GetNumberOfContextElements(m_contextIndex)) {
                // In case where the first element is also the last
                m_endOfContext = true;
            }

        }
    }

    public class CASTElementDescentantsFlattenIterator : CAbstractIterator<CASTElement> {

        private CASTComposite m_node;

        private int m_contextIndex;

        private int m_elementIndex;

        public CASTElementDescentantsFlattenIterator(CASTElement astNode) {
            m_node = astNode as CASTComposite;
            if (m_node == null) {
                throw new Exception("Iterator cannot iterate over leaf nodes");
            }
        }

        /// <summary>
        /// Initializes the iterator an returns the first element.
        /// </summary>
        /// <returns></returns>
        public override CASTElement Begin() {
            // Initialize Iterator
            PrepareFirst();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            return m_item;
        }

        /// <summary>
        /// Assures iteration inside the loop bounds
        /// </summary>
        /// <returns></returns>
        public override bool End() {
            if (m_item == null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increases the iterator and get the element it points (after increament).
        /// </summary>
        /// <returns></returns>
        public override CASTElement Next() {
            // Increase iterator
            PrepareNext();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            return m_item;
        }

        /// <summary>
        /// Initializes the iterator
        /// </summary>
        public override void ItBegin() {
            // Initialize Iterator
            PrepareFirst();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

        }

        /// <summary>
        /// Assures iteration inside the loop bounds
        /// </summary>
        /// <returns></returns>
        public override bool ItEnd() {
            if (m_item == null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increases the iterator to the next item
        /// </summary>
        public override void ITNext() {
            // Increase iterator
            PrepareNext();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);
        }

        private void PrepareNext() {
            m_elementIndex++;

            // if element index reaches the end of context
            if (m_elementIndex == m_node.GetNumberOfContextElements(m_contextIndex)) {
                m_contextIndex++;
                // Find the next non-empty context
                while (m_contextIndex < m_node.M_NumberOfContexts && 
                    m_node.GetNumberOfContextElements(m_contextIndex) == 0 ) {
                    m_contextIndex++;
                }
                m_elementIndex = 0;
            }

        }

        private void PrepareFirst(int contextIndex = 0) {

            // Initialize context
            m_contextIndex = contextIndex;

            // Find the first non-empty context
            while (m_contextIndex < m_node.M_NumberOfContexts &&
                    m_node.GetNumberOfContextElements(m_contextIndex) == 0 ) {
                m_contextIndex++;
            }
            m_elementIndex = 0;
        }
    }
}

    public class CASTElementDescentantsFlattenEventIterator : CAbstractIterator<CASTElement> {

        /// <summary>
        /// This class carries the events that are related to an AST structure traversal
        /// This class is specialized according to the type of node. That is, each different
        /// subclass of <c>CASTAbstractIteratorEvents</c> class describes the actions performed
        /// upon event raise in each type of AST node
        /// </summary>
        private CASTGenericIteratorEvents m_Event;

        /// <summary>
        /// Node where its children are traversed
        /// </summary>
        private CASTComposite m_node;

        /// Current context under traversal
        private int m_contextIndex;

        /// Current element index
        private int m_elementIndex;

        // A flag that indicates whether the end of context is reached.
        // It is raised by the PrepareNext() and PrepareFirst() methods
        // and its lower immediately after the related event is raised
        private bool m_endOfContext = false;

        // A flag that indicates whether the start of context is reached.
        // It is raised by the PrepareNext() method and its lower immediately
        // after the related event is raised
        private bool m_startOfContext = false;

        public CASTElementDescentantsFlattenEventIterator(CASTElement astNode,
            CASTGenericIteratorEvents events,object info=null) {
            m_Event = events;
            m_node = astNode as CASTComposite;
            if (m_node == null) {
                throw new Exception("Iterator cannot iterate over leaf nodes");
            }
        }


        public override CASTElement Begin(object param = null) {
            int context=0;

            // Initialize context from method parameter
            if (param != null) {
                if (param is int) {
                    context = (int)param;
                } else if (param is ContextType) {
                    context = m_node.MapContextToIndex((ContextType) param);
                }
                else {
                    Console.WriteLine("WARNING!!! Iterator initialization didn't work as expected");
                }
            }

            // Initialize Iterator
            PrepareFirst(context);

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            //Event Triggering
            if (m_item != null) {
                // Prepare event information
                CASTVisitorEventArgs args = new CASTVisitorEventArgs();

                // Raise OnContextEnter and OnNodeEnter events
                m_Event.OnContextEnter(m_item, args);
                m_Event.OnNodeEnter(m_item, args);
            }

            return m_item;
        }

        public override void ItBegin(object param = null) {

            // Initialize context from method parameter
            if (param != null) {
                if (param is int) {
                    m_contextIndex = (int)param;
                } else if (param is ContextType) {
                    m_contextIndex = m_node.MapContextToIndex((ContextType)param);
                }
            }

            base.ItBegin(param);
        }

        public override CASTElement Begin() {
            // Initialize Iterator
            PrepareFirst();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            //Event Triggering
            if (m_item != null) {
                // Prepare event information
                CASTVisitorEventArgs args = new CASTVisitorEventArgs();

                // Raise OnContextEnter and OnNodeEnter events
                m_Event.OnContextEnter(m_item, args);
                m_Event.OnNodeEnter(m_item, args);
            }

            return m_item;
        }

        public override bool End() {
            if (m_item == null) {
                return true;
            }

            //Event Triggering
            // Prepare event information
            CASTVisitorEventArgs args = new CASTVisitorEventArgs();

            // Raise the Node Leave event
            m_Event.OnNodeLeave(m_item, args);

            // Raise the Node Leave event
            if (m_endOfContext) {
                m_Event.OnContextLeave(m_item, args);
                m_endOfContext = false;
            }
            return false;
        }

        public override CASTElement Next() {
            // Increase iterator
            PrepareNext();

            // Update item
            m_item = m_node.GetChild(m_contextIndex, m_elementIndex);

            //Event Triggering
            if (m_item != null) {
                // Prepare event information
                CASTVisitorEventArgs args = new CASTVisitorEventArgs();

                // Raise OnContextEnter and OnNodeEnter events
                m_Event.OnNodeEnter(m_item, args);
                if (m_startOfContext) {
                    m_Event.OnContextEnter(m_item, args);
                    m_startOfContext = false;
                }
            }
            return m_item;
        }

        public override void ItBegin() {
            Begin();
        }

        public override bool ItEnd() {
            return End();
        }

        public override void ITNext() {
            Next();
        }

        private void PrepareNext() {
            m_elementIndex++;

            // if element index reaches the end of context
            if (m_elementIndex == m_node.GetNumberOfContextElements(m_contextIndex)) {
                m_contextIndex++;
            // Find the next non-empty context
            while (m_contextIndex < m_node.M_NumberOfContexts &&
                    m_node.GetNumberOfContextElements(m_contextIndex) == 0) {
                m_contextIndex++;
                }

                m_elementIndex = 0;
                m_startOfContext = true;
            }

        }

        private void  PrepareFirst(int contextIndex=0) {

            // Initialize context
            m_contextIndex = contextIndex;

        // Find the first non-empty context
        while (m_contextIndex < m_node.M_NumberOfContexts &&
                m_node.GetNumberOfContextElements(m_contextIndex) == 0) {
            m_contextIndex++;
            }
            m_elementIndex = 0;
            if (m_elementIndex == m_node.GetNumberOfContextElements(m_contextIndex)) {
                // In case where the first element is also the last
                m_endOfContext = true;
            }

        }
    }
