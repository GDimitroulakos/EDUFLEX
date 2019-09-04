using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using RangeIntervals;


namespace Parser.UOPCore
{
    [Serializable]
    public class RERecord {
        private FA m_DFA = null;    // Result from Subset Construction
        private FA m_NFA = null;    // Result from Thompson
        private FA m_minDFA = null; // Result from Hopcroft
        private TextSpan m_REPosition; // Position of Regular Expression into the code
        private string m_label = null;   // Regular Expression label;
        [NonSerialized]
        private CRegexpStatement m_RETree = null;
        private string m_actionCode;

        public FA M_Dfa {
            get => m_DFA;
            set => m_DFA = value;
        }

        public FA M_Nfa {
            get => m_NFA;
            set => m_NFA = value;
        }

        public FA M_MinDfa {
            get => m_minDFA;
            set => m_minDFA = value;
        }

        public TextSpan M_RePosition {
            get => m_REPosition;
            set => m_REPosition = value;
        }

        public uint M_Line {
            get => m_REPosition.M_StartLine;
        }

        public string M_Label {
            get => m_label;
            set => m_label = value;
        }

        public string M_ActionCode {
            get => m_actionCode;
            set => m_actionCode = value;
        }

        public CRegexpStatement M_ReTree {
            get => m_RETree;
            set => m_RETree = value;
        }
    }



    public enum FAStateType {
        FT_NA,FT_ACCEPTED,FT_NONACCEPTED
    }

    /// <summary>
    /// Represents a graph query info object for finite automaton where information 
    /// on nodes, edges and the graph is represented by the FAStateInfo, FAEdgeInfo, FAInfo
    /// class correspondingly.  
    /// </summary>
    [Serializable]
    public class FAGraphQueryInfo : CGraphQueryInfo<FAStateInfo, FAEdgeInfo, FAInfo> {
        public FAGraphQueryInfo(CGraph graph, object key) : base(graph, key) {
        }

        public CCharRangeSet GetDFAEdgeTransitionCharacterSet(CGraphEdge edge) {
            return Info(edge).M_TransitionCharSet;
        }

        public void SetDFAEdgeTransitionCharacterSet(CGraphEdge edge, CCharRangeSet set) {
            Info(edge).M_TransitionCharSet = set;
        }
    }

   [Serializable]
    public class FAInfo {
        /// <summary>
        /// Refers to the FA hosting the information
        /// </summary> 
        private FA m_fa;
        /// <summary>
        /// It is the string prefix applied to all nodes of the FA
        /// </summary>
        //private string m_nodeLabelsPrefix="";
        /// <summary>
        /// It is the initial FA node
        /// </summary>
        private CGraphNode m_initial=null;
        /// <summary>
        /// It is the set of FA accepting states
        /// </summary>
        private HashSet<CGraphNode> m_final = new HashSet<CGraphNode>();

        /// <summary>
        /// Records the information referring to each separate regular expression
        /// </summary>

        internal FAInfo(FA fa) {
            m_fa = fa;
        }

        
        internal CGraphNode M_Initial {
            get => m_initial;
            set => m_initial = value;
        }

        internal HashSet<CGraphNode> M_Final {
            get => m_final;
            set => m_final = value;
        }

        internal List<CGraphNode> GetFinalStates(){
            return new List<CGraphNode>(m_final);
        }

    }

    /// <summary>
    /// Encapsulates information of FA nodes. Provides access to 
    /// the information through methods and properties
    /// </summary>
    [Serializable]
    public class FAStateInfo :CGraphNode{
        private FAStateType m_stateType;
        /// <summary>
        /// It is the string prefix applied to the current node of the FA
        /// The prefix derives from the concatenation of the string gathered
        /// inside the m_nodeLabelsPrefix set
        /// </summary>
        private HashSet<string> m_nodeLabelsPrefix=new HashSet<string>();
        
        private List<uint> m_LineDependencies=new List<uint>();

        public string M_NodeLabelPrefix {
            get {
                string st="";
                foreach (string s in m_nodeLabelsPrefix) {
                    st += s;
                }

                return st;
            }
            set {
                if (!m_nodeLabelsPrefix.Contains(value)) {
                    m_nodeLabelsPrefix.Add(value);
                }
            } 
        }

        /// <summary>
        /// Adds the given dependency if it doesn't exist in the list
        /// and immediately sorts the list 
        /// </summary>
        /// <param name="line"></param>
        public void AddLineDependency(uint line) {
            if (!m_LineDependencies.Contains(line)) {
                m_LineDependencies.Add(line);
                m_LineDependencies.Sort();
            }
        }

        public List<uint> M_LineDependencies => m_LineDependencies;

        public HashSet<string> GetNodeLabelPrefix() {
            return m_nodeLabelsPrefix;
        }

        public FAStateType MStateType {
            get => m_stateType;
            set => m_stateType = value;
        }
    }

    [Serializable]
    public class FAEdgeInfo {
        // Represents the closure multiplicity if the edge refers to 
        // a loop edge of a closure
        //private RangeSetO<Range<Int32>,Int32> m_closureMultiplicityRange;

        private CCharRangeSet m_transitionCharSet;

        internal CCharRangeSet M_TransitionCharSet {
            get => m_transitionCharSet;
            set => m_transitionCharSet = value;
        }
    }
    


    /// <summary>
    /// This class represents a finite automaton. It is a graph with ///    additional information regarding the labels' transitions.
    /// </summary>
    /// <seealso cref="GraphLibrary.CGraph" />
    [Serializable]
    public class FA: CGraph
    {
        private FAGraphQueryInfo m_FAInfo=null;
        private CCharRangeSet m_alphabet;
        
        public CCharRangeSet M_Alphabet {
            get { return m_alphabet; }
            set { m_alphabet = value; }
        }

        // The key by which we can access edge transition info
        public const string m_FAINFOKEY = "FAinfo";

       
        public CGraphNode M_Initial{
            get {
                return ((FAInfo)(m_FAInfo.Info())).M_Initial;
            }
            set {
                ((FAInfo) (m_FAInfo.Info())).M_Initial = value;
            } 
        }
        
        public List<CGraphNode> GetFinalStates() {
            return m_FAInfo.Info().GetFinalStates();
        }

        public void SetFinalState(CGraphNode node) {
            bool suc;
            if (IsANodeOfGraph(node) && !IsFinalState(node)) {
                suc = m_FAInfo.Info().M_Final.Add(node);
            }
        }

        public bool IsFinalState(CGraphNode state) {
            return m_FAInfo.Info().M_Final.Contains(state);
        }

       public CGraphNode GetTransitionTarget(CGraphNode source, Int32 character) {
            // Search every successor of source node and find the one with
            // with an edge labeled by the given character
              foreach (CGraphEdge transition in source.OutgoingEdges) {
                CCharRangeSet set = GetFAEdgeInfo(transition);
                if (set.IsCharInSet(character)) {
                    return transition.M_Target;
                }
            }
            return null;
        }

        public FA(){
            m_FAInfo = new FAGraphQueryInfo(this, m_FAINFOKEY);
            m_FAInfo.CreateInfo(new FAInfo(this));
            m_alphabet = new CCharRangeSet(false);
        }

        public override N CreateGraphNode<N>() {
            N newNode= base.CreateGraphNode<N>();
            m_FAInfo.CreateInfo(newNode,new FAStateInfo());
            return newNode;
        }

        public override N CreateGraphNode<N>(string label) {
            N newNode = base.CreateGraphNode<N>(label);
            m_FAInfo.CreateInfo(newNode, new FAStateInfo());
            return newNode;
        }

        public override E AddGraphEdge<E, N>(N source, N target, string label = null, GraphType edgetype = GraphType.GT_DIRECTED) {
            E newedge = base.AddGraphEdge<E, N>(source, target, label, edgetype);
            m_FAInfo.CreateInfo(newedge,new FAEdgeInfo());
            return newedge;
        }

        public override E AddGraphEdge<E, N>(N source, N target, GraphType edgetype = GraphType.GT_DIRECTED) {
            E newedge = base.AddGraphEdge<E, N>(source, target, edgetype);
            m_FAInfo.CreateInfo(newedge,new FAEdgeInfo());
            return newedge;
        }

        
        /// <summary>
        /// Returns the destination nodes reached from the specified node and the given
        /// character
        /// </summary>
        /// <param name="node"></param>
        /// <param name="alphabetChar"></param>
        /// <returns></returns>
        public HashSet<CGraphNode> Delta(CGraphNode node, Int32 alphabetChar) {
            HashSet<CGraphNode> d = new HashSet<CGraphNode>();
            CGraphEdge e;
            CCharRangeSet transitionlbl;

            // 1. Find all outgoing edges of the node
            CIt_NodeOutgoingEdges it = new CIt_NodeOutgoingEdges(node);
            for (it.Begin();!it.End(); it.Next()) {
                e = it.M_CurrentItem;

                // 2. Check which outgoing edges include the alphabetChar
                transitionlbl = GetFAEdgeInfo(e);
                if (transitionlbl.IsCharInSet(alphabetChar)) {
                    // 2a. Add the target nodes to the output set
                    d.Add(e.M_Target);
                }
            }
            return d;
        }

        /// <summary>
        /// Updates the FA alphabet after a series of edge insertions
        /// </summary>
        public void UpdateAlphabet() {
            CCharRangeSet set;
            foreach (CGraphEdge edge in m_graphEdges) {
                set = GetFAEdgeInfo(edge);
                if (set != null) {
                    m_alphabet.AddSet(set);
                }
            }
        }

        /// <summary>
        /// Returns the character set info associated with a specific FA edge 
        /// </summary>
        /// <param name="e">The specified FA edge</param>
        /// <returns></returns>
        public CCharRangeSet GetFAEdgeInfo(CGraphEdge e) {
            return m_FAInfo.Info(e).M_TransitionCharSet;
        }

        /// <summary>
        /// Sets the character set associated with an FA transition
        /// </summary>
        /// <param name="e"></param>
        /// <param name="transitionInfo"></param>
        public void SetFAEdgeInfo(CGraphEdge e, CCharRangeSet transitionInfo) {
            m_FAInfo.Info(e).M_TransitionCharSet = transitionInfo;
        }

        /// <summary>
        /// Returns the string used as a prefix to label the nodes of the FA
        /// </summary>
        /// <returns></returns>
        public string GetFANodePrefix(CGraphNode node) {
            return m_FAInfo.Info(node).M_NodeLabelPrefix;
        }

        public HashSet<string> GetFANodePrefixLabels(CGraphNode node) {
            return m_FAInfo.Info(node).GetNodeLabelPrefix();
        }

        /// <summary>
        /// Sets the string used as a prefix to label the nodes of the FA
        /// </summary>
        /// <param name="prefix"></param>
        public void SetFANodePrefix(string prefix,CGraphNode node) {
            m_FAInfo.Info(node).M_NodeLabelPrefix = prefix;
        }
        public void SetFANodePrefix(string prefix) {
            foreach (CGraphNode node in m_graphNodes) {
                m_FAInfo.Info(node).M_NodeLabelPrefix = prefix;
            }
            PrefixGraphElementLabels(prefix,GraphElementType.ET_NODE);
        }

        public List<uint> GetFANodeLineDependencies(CGraphNode node) {
            return m_FAInfo.Info(node).M_LineDependencies;
        }

        public void SetFANodeLineDependency(uint line, CGraphNode node) {
            m_FAInfo.Info(node).AddLineDependency(line);
        }

        public void SetFANodesLineDependency(uint line) {
            foreach (CGraphNode node in m_graphNodes) {
                m_FAInfo.Info(node).AddLineDependency(line);
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Initial State: " + M_Initial.M_Label +"\n");
            str.Append("Final States: ");
            foreach (CGraphNode finalState in GetFinalStates()) {
                str.Append(finalState.M_Label + ";");
            }

            str.Append("FA edges: \n");
            foreach (CGraphEdge edge in m_graphEdges) {
                str.Append(edge.M_Source.M_Label + "->" + edge.M_Target.M_Label+";\n");
            }
            return str.ToString();

        }
    }
}
