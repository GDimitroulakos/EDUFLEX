using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;

namespace Parser.UOPCore
{
    /// <summary>
    /// This class represents a finite automaton. It is a graph with additional information
    /// regarding the labels' transitions.
    /// </summary>
    /// <seealso cref="GraphLibrary.CGraph" />
    public class FA: CGraph
    {
        private CGraphNode m_initial=null;
        private HashSet<CGraphNode> m_final=null;
        private CGraphQueryInfo m_transitionsAlphabetInfo=null;
        private CCharRangeSet m_alphabet;

        public CCharRangeSet M_Alphabet {
            get { return m_alphabet; }
            set { m_alphabet = value; }
        }

        // The key by which we can access edge transition info
        public const string m_TRANSITIONSKEY = "transitions";


        public CGraphNode M_Initial
        {
            get { return m_initial; }
            set { m_initial = value; }
        }

        public CGraphNode InitialState() {
            return m_initial;
        }

        public List<CGraphNode> GetFinalStates() {
            return new List<CGraphNode>(m_final);
        }

        public void SetFinalState(CGraphNode node) {
            bool suc;
            if (IsANodeOfGraph(node) && !m_final.Contains(node)) {
                suc =m_final.Add(node);
            }
        }

        public CGraphNode GetTransitionTarget(CGraphNode source, Int32 character) {
            // Search every successor of source node and find the one with
            // with an edge labeled by the given character
            foreach (CGraphEdge edge in source.OutgoingEdges) {
                CCharRangeSet set = GetEdgeInfo(edge);
                if (set.IsCharInSet(character)) {
                    return edge.M_Target;
                }
            }
            return null;
        }

        public FA(){
            m_transitionsAlphabetInfo = new CGraphQueryInfo(this,m_TRANSITIONSKEY);
            m_alphabet = new CCharRangeSet(false);
            m_final= new HashSet<CGraphNode>();
        }

        public HashSet<CGraphNode> Delta(CGraphNode node, Int32 alphabetChar) {
            HashSet<CGraphNode> d = new HashSet<CGraphNode>();
            CGraphEdge e;
            CCharRangeSet transitionlbl;

            // 1. Find all outgoing edges of the node
            CIt_NodeOutgoingEdges it = new CIt_NodeOutgoingEdges(node);
            for (it.Begin();!it.End(); it.Next()) {
                e = it.M_CurrentItem;

                // 2. Check which outgoing edges include the alphabetChar
                transitionlbl = GetEdgeInfo(e);
                if (transitionlbl.IsCharInSet(alphabetChar)) {
                    // 2a. Add the target nodes to the output set
                    d.Add(e.M_Target);
                }
            }
            return d;
        }

        public void UpdateAlphabet() {
            CCharRangeSet set;
            foreach (CGraphEdge edge in m_graphEdges) {
                set = GetEdgeInfo(edge);
                if (set != null) {
                    m_alphabet.AddSet(set);
                }
            }

        }

        public CCharRangeSet GetEdgeInfo(CGraphEdge e) {
            return m_transitionsAlphabetInfo.Info(e) as CCharRangeSet;
        }

        public override string ToString()
        {
            return null;

        }
    }
}
