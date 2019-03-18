using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Parser.ASTEvents {

    /// <summary>
    /// Event handler signature
    /// </summary>
    /// <typeparam name="T">The type of AST node element</typeparam>
    /// <param name="current">The current.</param>
    /// <param name="args">The <see cref="CASTIteratorEventArgs" /> instance containing the event data.</param>
    public delegate void ASTVisitorEventHandler(CASTElement current, CASTVisitorEventArgs args);

    /// <summary>
    /// Event handler information
    /// </summary>
    public class CASTVisitorEventArgs {


    }

    /// <summary>
    /// <c>CASTAbstractIteratorEvents</c> class holds the events raised during the visitor
    /// traversal in the composite structure.
    /// </summary>
    public class CASTGenericIteratorEvents {

        protected event ASTVisitorEventHandler m_OnNodeEnter;

        protected event ASTVisitorEventHandler m_OnNodeLeave;

        protected event ASTVisitorEventHandler m_OnContextEnter;

        protected event ASTVisitorEventHandler m_OnContextLeave;

        public CASTGenericIteratorEvents() { }


        public virtual void OnNodeEnter(CASTElement node, CASTVisitorEventArgs args) {
            m_OnNodeEnter?.Invoke(node, args);
        }
        public virtual void OnNodeLeave(CASTElement node, CASTVisitorEventArgs args) {
            m_OnNodeLeave?.Invoke(node,args);
        }
        public virtual void OnContextEnter(CASTElement node, CASTVisitorEventArgs args) {
            m_OnContextEnter?.Invoke(node,args);
        }
        public virtual void OnContextLeave(CASTElement node, CASTVisitorEventArgs args) {
            m_OnContextLeave?.Invoke(node,args);
        }

    }
}
