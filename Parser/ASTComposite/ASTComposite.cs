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
using Parser.ASTEvents;
using Parser.ASTFactories;
using Parser.ASTIterator;
using Parser.ASTVisitor;

namespace Parser {

   /// <summary>
    /// Represents an AST node.
    /// </summary>
    public abstract partial class CASTElement {

        /// <summary>
        /// The type of node in the abstract syntax tree
        /// </summary>
        protected NodeType m_nodeType;

        /// <summary>
        /// The <c>m_nodeCategory</c> member variable refers to the broader category which every
        /// node belongs. The category refers to the grammar symbol to which the alternative nodes
        /// that create ASTElements belong
        /// </summary>
        protected NodeType m_nodeCategory;

        /// <summary>
        /// Refers to the parent element of the given node
        /// </summary>
        protected CASTElement m_parent;

        /// <summary>
        /// The node has a unique serialnumber
        /// </summary>
        private int m_serialNumber;

        /// <summary>
        /// Textual representation of the element in the source program. By default is null.
        /// Set it to a non-null value if required
        /// </summary>
        private string m_text=null;

        /// <summary>
        /// The node label consist of the type and serial number
        /// </summary>
        protected string m_label=null;
        
        /// <summary>
        /// m_serialNumberCounter counts the number of ASTElements instanciated
        /// </summary>
        static private int m_serialNumberCounter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CASTElement"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        public CASTElement(NodeType nodeType, CASTElement parent, NodeType nodeCategory) {
            m_nodeType = nodeType;
            m_nodeCategory = nodeCategory;
            m_parent = parent;
            m_serialNumber = m_serialNumberCounter++;
            m_label = nodeType.ToString() + "_" + m_serialNumber;
        }
        
        protected abstract void AddChild( CASTElement child, int context, int pos = -1 /*insert last by default*/);

        /// <summary>
        /// The Accept function is called by the Visitors' Visit functions
        /// </summary>
        /// <typeparam name="Return">The type of the eturn.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns></returns>
        public abstract Return AcceptVisitor<Return>(CASTAbstractVisitor<Return> visitor);

        /// <summary>
        /// Accepts the iterator.
        /// </summary>
        /// <param name="iteratorFactory">The iterator factory.</param>
        /// <returns></returns>
        public abstract CAbstractIterator<CASTElement> AcceptIterator(CASTAbstractConcreteIteratorFactory iteratorFactory);

        /// <summary>
        /// Accepts the event iterator.
        /// </summary>
        /// <param name="iteratorFactory">The iterator factory.</param>
        /// <returns></returns>
        public abstract CAbstractIterator<CASTElement> AcceptEventIterator(CASTAbstractConcreteIteratorFactory iteratorFactory,
            CASTGenericIteratorEvents events, object info = null);


        /// <summary>
        /// Gets the type of the m_ node.
        /// </summary>
        /// <value>
        /// The type of the m_ node.
        /// </value>
        public NodeType M_NodeType {
            get { return m_nodeType; }
        }

        /// <summary>
        /// Gets the m_ node category.
        /// </summary>
        /// <value>
        /// The m_ node category.
        /// </value>
        public NodeType M_NodeCategory {
            get { return m_nodeCategory; }
        }

        /// <summary>
        /// Returns the parent of this node
        /// </summary>
        /// <value>
        /// The m_ parent.
        /// </value>
        public CASTElement M_Parent {
            get { return m_parent; }
        }

        /// <summary>
        /// The node has a unique serial number
        /// </summary>
        /// <value>
        /// The m_ serial number.
        /// </value>
        public int M_SerialNumber {
            get { return m_serialNumber; }
        }

        /// <summary>
        /// Returns the node label
        /// </summary>
        /// <value>
        /// The m_ label.
        /// </value>
        public virtual string M_Label {
            get{return m_label; }
        }

        /// <summary>
        /// Returns the configuration for this type of node
        /// </summary>
        /// <value>
        /// The m_ configuration.
        /// </value>
        public CNodeTypeConfiguration M_Configuration {
            get { return CConfigurationSettings.m_nodeTypeConfiguration[m_nodeType]; }
        }

        /// <summary>
        /// Textual representation of the element in the source code
        /// </summary>
        internal string M_Text {
            get => m_text;
            set => m_text = value;
        }
   }

    /// <summary>
    /// Represents an AST Leaf node.
    /// </summary>
    /// <typeparam name="T">The type of Leaf node's semantic value</typeparam>
    /// <seealso cref="Parser.CASTElement" />
    public abstract class CASTLeaf<T> : CASTElement {

        /// <summary>
        /// It carries the string representation of the Token
        /// </summary>
        private string m_TokenLiteral;

        /// <summary>
        /// It carries the semantic value of the Token
        /// </summary>
        protected T m_value=default(T);

        /// <summary>
        /// An instance of this class converts the token of this type
        /// to its semantic value and the opposite. One instance exists
        /// for all instance of each type of ASTLeaf<T> type
        /// </summary>
        static protected CTokenSemanticValueConverter<T> m_semanticValueConverter=null;

        public CASTLeaf(string literal, NodeType nodetype, CASTElement parent,
            NodeType nodeCategory= NodeType.CAT_NA) : base(nodetype, parent,nodeCategory) {
            m_TokenLiteral = literal;
            m_label += "< " + m_TokenLiteral + " >";
        }


        /// <summary>
        /// Performs an explicit conversion from <see cref="CASTLeaf{T}"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator T(CASTLeaf<T> token) {
            return token.M_Value;
        }

        /// <summary>
        /// Represents the semantic value of the literal.
        /// </summary>
        /// <value>
        /// The m_ value.
        /// </value>
        public T M_Value {
            get {
                if (m_value != null) {
                    return m_value;
                }
                else {
                    if (m_semanticValueConverter != null) {
                        m_value = m_semanticValueConverter.GetSemanticValue(M_TokenLiteral);
                        return m_value;
                    }
                    else {
                        throw new Exception("Semantic value Converter does not exist");
                    }
                }
            }
        }

        /// <summary>
        /// Returns the string of the token
        /// </summary>
        /// <value>
        /// The m_ token literal.
        /// </value>
        public string M_TokenLiteral {
            get {
                return m_TokenLiteral;
            }
        }

        /// <summary>
        /// Returns the node label
        /// </summary>
        /// <value>
        /// The m_ label.
        /// </value>
        public override string M_Label {
            get {
                if (m_label == null) {
                    m_label = "<" + m_TokenLiteral + ">" + M_SerialNumber.ToString();
                }
                return m_label;
            }
        }
    }

    /// <summary>
    /// This is an interface that is required to convert the token to
    /// its semantic value and the opposite
    /// </summary>
    /// <typeparam name="T">The token's semantic value type</typeparam>
    public abstract class CTokenSemanticValueConverter<T> {
        // A reference to the token node
        protected CASTLeaf<T> m_token;

        public CTokenSemanticValueConverter(CASTLeaf<T> token) {
            m_token = token;
        }

        /// <summary>
        /// Converts the token string literal to its semantic value
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns></returns>
        public abstract T GetSemanticValue(string literal);

        /// <summary>
        /// Converts the semantic value to a string literal
        /// </summary>
        /// <param name="semanticValue">The semantic value.</param>
        /// <returns></returns>
        public abstract string GetLiteral(T semanticValue);

    }

    /// <summary>
    /// Default token value converter. It is assumed that the semantic value is the
    /// token object itself. It is practical for identifiers and other objects assignable
    /// to symbol tables
    /// </summary>
    /// <seealso cref="CASTElement" />
    public class TokenSemanticValueDefaultConverter : CTokenSemanticValueConverter<CASTElement> {

        static private TokenSemanticValueDefaultConverter m_singleton=null;

        protected TokenSemanticValueDefaultConverter(CASTLeaf<CASTElement> token) : base(token) {}
        public override CASTElement GetSemanticValue(string literal) {
            return m_token;
        }
        public override string GetLiteral(CASTElement semanticValue) {
            return semanticValue.ToString();
        }

        public static TokenSemanticValueDefaultConverter Create(CASTLeaf<CASTElement> token) {
            if (m_singleton == null) {
                m_singleton = new TokenSemanticValueDefaultConverter(token);
            }
            return m_singleton;
        }
    }

    /// <summary>
    /// Represents an AST Composite node.
    /// </summary>
    /// <seealso cref="Parser.CASTElement" />
    public abstract partial class CASTComposite : CASTElement {

        private List<CASTElement>[] m_descentands;

       public CASTComposite(NodeType nodeType, CASTElement parent,
            NodeType nodeCategory = NodeType.CAT_NA) : base(nodeType, parent,nodeCategory) {

            M_NumberOfContexts = CConfigurationSettings.m_nodeTypeConfiguration[nodeType].M_NumberOfContexts;

            // Instanciate the contextual IR
            m_descentands = new List<CASTElement>[M_NumberOfContexts];
            for (int j = 0; j < M_NumberOfContexts; j++) {
               m_descentands[j] = new List<CASTElement>();
            }
        }

        /// <summary>
        /// AddChild method inserts a child at the given context to the specified location
        /// The location given by the pos parameter can take one of the following values
        /// 1) -1 to indicate the last element of the list
        /// 2) a positive integer indicating any position in the list where if it equals to
        ///    m_descentands[context].Count, it is placed at the end of the list as in option 1
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="child">The child.</param>
        /// <param name="pos">The position.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Error! Negative index in array reference
        /// or
        /// Error! Negative index in array reference
        /// </exception>
        protected override void AddChild(CASTElement child, int context, int pos=-1 /*insert last by default*/) {
            if (pos == -1) {
                m_descentands[context].Add(child);
            }
            else if (pos > -1) {
                if (pos <= m_descentands[context].Count) {
                    m_descentands[context].Insert(pos, child);
                }
                else {
                    throw new IndexOutOfRangeException("Error! Negative index in array reference");
                }
            }
            else {
                throw new IndexOutOfRangeException("Error! Negative index in array reference");
            }
        }

        /// <summary>
        /// Adds the child to the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="child">The child.</param>
        /// <param name="pos">The position.</param>
        public virtual void AddChild(CASTElement child, ContextType context=ContextType.CT_NA, int pos = -1) {
            if (context == ContextType.CT_NA) {
                throw new Exception("ERROR!!! Invalid context type");
            }
            int contextIndex = CConfigurationSettings.m_contextTypeConfiguration[context].M_ContextIndex;
            AddChild(child, contextIndex,  pos);
        }

        /// <summary>
        /// Returns the pos-th child at the specified context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        public virtual CASTElement GetChild(int context, int pos = -1) {
            if (context < M_NumberOfContexts && pos < m_descentands[context].Count && pos > -1) {
                return m_descentands[context][pos];
            }
            return null;
        }

        /// <summary>
        /// Returns the pos-th child at the specified context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        public virtual CASTElement GetChild(ContextType context, int pos = -1) {
            int ctxpos = MapContextToIndex(context);
            if (ctxpos < M_NumberOfContexts && pos < m_descentands[ctxpos].Count && pos > -1) {
                return m_descentands[ctxpos][pos];
            }
            return null;
        }

        /// <summary>
        /// Returns the list of children in a given context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public virtual List<CASTElement> GetContextChildren(int context) {
            if (context < M_NumberOfContexts) {
                return m_descentands[context];
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the list of children in a given context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual List<CASTElement> GetContextChildren(ContextType context) {
            int contextIndex = MapContextToIndex(context);
            return GetContextChildren(contextIndex);
        }


        /// <summary>
        /// Returns the number of elements in a given context.
        /// </summary>
        /// <param name="context">The context index</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public int GetNumberOfContextElements(int context) {
            if (context < M_NumberOfContexts && context > -1) {
                return m_descentands[context].Count;
            }
            else {
                throw  new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the number of elements in a given context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public int GetNumberOfContextElements(ContextType context) {
            return GetNumberOfContextElements(MapContextToIndex(context));
        }


        /// <summary>
        /// Maps a context type to the index in the descentands collection of the ASTComposite node
        /// The function uses the context type as an index of the m_contextMappings table
        /// </summary>
        /// <param name="ctype">The ctype.</param>
        /// <returns>The context index</returns>
        internal int MapContextToIndex(ContextType ctype) {
            return (int)ctype-(int)m_nodeType;
        }

        /// <summary>
        /// Maps the index to context.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal ContextType MapIndexToContext(int index) {
            if (index < M_NumberOfContexts) {
                return (ContextType) (m_nodeType + index);
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the number of contexts for this composite node
        /// </summary>
        /// <value>
        /// The m_ number of contexts.
        /// </value>
        public int M_NumberOfContexts {
            get ; private set; }

        /// <summary>
        /// Returns the node label
        /// </summary>
        /// <value>
        /// The m_ label.
        /// </value>
        public override string M_Label {
            get {
                if (m_label == null) {
                    m_label = m_nodeType.ToString() + "_" + M_SerialNumber.ToString();
                }
                return m_label;
            }
        }
    }
}