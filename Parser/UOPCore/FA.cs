using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;

namespace Parser.UOPCore
{
    public enum FAStateType {
        FT_NA,FT_ACCEPTED,FT_NONACCEPTED
    }
    
    internal class FAInfo {
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

        internal FAInfo(FA fa) {
            m_fa = fa;
        }

        /*internal string M_NodeLabelsPrefix {
            get => m_nodeLabelsPrefix;
            set => m_nodeLabelsPrefix = value;
        }*/

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
    public class FAStateInfo :CGraphNode{
        private FAStateType m_stateType;
        /// <summary>
        /// It is the string prefix applied to all nodes of the FA
        /// </summary>
        private string m_nodeLabelsPrefix="";

        public string M_NodeLabelsPrefix {
            get => m_nodeLabelsPrefix;
            set => m_nodeLabelsPrefix = value;
        }

        public FAStateType MStateType {
            get => m_stateType;
            set => m_stateType = value;
        }
    }

    internal class FAEdgeInfo {
        private CCharRangeSet m_transitionCharSet;

        internal CCharRangeSet M_TransitionCharSet {
            get => m_transitionCharSet;
            set => m_transitionCharSet = value;
        }
    }
    
    /// <summary>
    /// This class represents a finite automaton. It is a graph with additional information
    /// regarding the labels' transitions.
    /// </summary>
    /// <seealso cref="GraphLibrary.CGraph" />
    public class FA: CGraph
    {
        private CGraphQueryInfo m_FAEdgeInfo=null;
        private CGraphQueryInfo m_FAStateInfo = null;
        private CGraphQueryInfo m_FAInfo = null;
        private CCharRangeSet m_alphabet;
        
        public CCharRangeSet M_Alphabet {
            get { return m_alphabet; }
            set { m_alphabet = value; }
        }

        // The key by which we can access edge transition info
        public const string m_FASTATEINFOKEY = "FAstatesInfo";
        public const string m_FAEDGEINFOKEY = "FAtransitionsInfo";
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
            return m_FAInfo.CastGraphInfo<FAInfo>().GetFinalStates();
        }

        public void SetFinalState(CGraphNode node) {
            bool suc;
            if (IsANodeOfGraph(node) && !IsFinalState(node)) {
                suc = m_FAInfo.CastGraphInfo<FAInfo>().M_Final.Add(node);
            }
        }

        public bool IsFinalState(CGraphNode state) {
            return m_FAInfo.CastGraphInfo<FAInfo>().M_Final.Contains(state);
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
            m_FAStateInfo = new CGraphQueryInfo(this,m_FASTATEINFOKEY);
            m_FAEdgeInfo = new CGraphQueryInfo(this,m_FAEDGEINFOKEY);
            m_FAInfo = new CGraphQueryInfo(this, m_FAINFOKEY);
            m_FAInfo.CreateInfo(new FAInfo(this));
            m_alphabet = new CCharRangeSet(false);
        }

        public override N CreateGraphNode<N>() {
            N newNode= base.CreateGraphNode<N>();
            m_FAStateInfo.CreateInfo(newNode,new FAStateInfo());
            return newNode;
        }

        public override N CreateGraphNode<N>(string label) {
            return base.CreateGraphNode<N>(label);
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
            return m_FAEdgeInfo.Info(e) as CCharRangeSet;
        }

        /// <summary>
        /// Sets the character set associated with an FA transition
        /// </summary>
        /// <param name="e"></param>
        /// <param name="transitionInfo"></param>
        public void SetFAEdgeInfo(CGraphEdge e, CCharRangeSet transitionInfo) {
            m_FAEdgeInfo.CastEdgeInfo<FAEdgeInfo>(e).M_TransitionCharSet = transitionInfo;
        }

        /// <summary>
        /// Returns the string used as a prefix to label the nodes of the FA
        /// </summary>
        /// <returns></returns>
        public string GetFANodePrefix(CGraphNode node) {
            return m_FAStateInfo.CastGraphInfo<FAStateInfo>().M_NodeLabelsPrefix;
        }

        /// <summary>
        /// Sets the string used as a prefix to label the nodes of the FA
        /// </summary>
        /// <param name="prefix"></param>
        public void SetFANodePrefix(string prefix,CGraphNode node) {
            m_FAStateInfo.CastGraphInfo<FAStateInfo>().M_NodeLabelsPrefix = prefix;
        }
        public void SetFANodePrefix(string prefix) {
            foreach (CGraphNode node in m_graphNodes) {
                m_FAStateInfo.CastNodeInfo<FAStateInfo>(node).M_NodeLabelsPrefix = prefix;
            }
            PrefixGraphElementLabels(prefix,GraphElementType.ET_NODE);
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
