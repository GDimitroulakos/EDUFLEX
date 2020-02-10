using System;
using System.Collections.Generic;
using System.IO;
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
        private string m_label = null; // Regular Expression label;
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

    [Serializable]
    public class FALoop {
        public enum ClosureType { CT_NA,CT_NONEORMULT, CT_ONEORMULT, CT_FINITE}
        private CGraphNode m_entryNode=null;
        private CGraphNode m_exitNode=null;
        private List<CGraphNode> m_participatingNodes=new List<CGraphNode>();
        private Range<int> m_closureRange=null;
        private ClosureType m_ClosureType=ClosureType.CT_NA;
        private int m_loopSerial=-1;

        public CGraphNode MEntryNode {
            get => m_entryNode;
            set => m_entryNode = value;
        }

        public CGraphNode MExitNode {
            get => m_exitNode;
            set => m_exitNode = value;
        }

        public List<CGraphNode> MParticipatingNodes {
            get => m_participatingNodes;
            set => m_participatingNodes = value;
        }

        public Range<int> MClosureRange {
            get => m_closureRange;
            set => m_closureRange = value;
        }

        public ClosureType MClosureType {
            get => m_ClosureType;
            set => m_ClosureType = value;
        }

        public int MLoopSerial {
            get => m_loopSerial;
            set => m_loopSerial = value;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append("Closure Serial : " + MLoopSerial +"\n");
            s.Append("Closure Type : " + MClosureType + "\n");
            s.Append("Closure Range : " + MClosureRange + "\n");
            s.Append("Entry Node : " + MEntryNode.M_Label + "\n");
            s.Append("Exit Node : " + MExitNode.M_Label + "\n");
            s.Append("Participating Nodes (" + MParticipatingNodes.Count  +"): ");
            foreach (CGraphNode node in MParticipatingNodes) {
                s.Append(" "+node.M_Label+" ");
            }
            s.Append("\n");
            return s.ToString();
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

        public void AddFALoop(FALoop loop) {
            Info().AddFALoop(loop);
        }

        public FALoop GetFaLoop(int serial) {
            return Info().GetFALoop(serial);
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
        ///  This member lists the closures' information+ in the FA 
        /// </summary>
        private Dictionary<int,FALoop> m_loopsInFA=new Dictionary<int, FALoop>();

        /// <summary>
        /// Records the information referring to each separate regular expression
        /// </summary>
        


        internal FAInfo(FA fa) {
            m_fa = fa;
        }

        public Dictionary<int, FALoop> MLoopsInFa {
            get => m_loopsInFA;
        }

        public void AddFALoop(FALoop loop) {
            if (!m_loopsInFA.ContainsKey(loop.MLoopSerial)) {
                m_loopsInFA[loop.MLoopSerial] = loop;
            }
            else {
                throw new Exception("loop already added");
            }
        }

        public FALoop GetFALoop(int serial) {
            return m_loopsInFA[serial];
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

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append("Initial: "+ M_Initial.M_Label+"\n");
            s.Append("Final:");
            foreach (CGraphNode node in M_Final) {
                s.Append(" " + node.M_Label + " ");
                s.Append("\n");
            }
            s.Append("\n");
            foreach (FALoop faLoop in MLoopsInFa.Values) {
                s.Append(faLoop.ToString());
                s.Append("\n"); 
            }
            return s.ToString();
        }
    }

    /// <summary>
    /// Encapsulates information of FA nodes. Provides access to 
    /// the information through methods and properties
    /// </summary>
    [Serializable]
    public class FAStateInfo {
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
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append("State Type: " + m_stateType);
            s.Append("Prefixes: ");
            foreach (string s1 in m_nodeLabelsPrefix) {
                s.Append(" " + s1 + " ");
            }
            s.Append("Line Dependencies: ");
            foreach (uint s1 in m_LineDependencies) {
                s.Append(" " + s1 + " ");
            }
            return s.ToString();
        }
    }

    [Serializable]
    public class FAEdgeInfo {
        
        private CCharRangeSet m_transitionCharSet;

        internal CCharRangeSet M_TransitionCharSet {
            get => m_transitionCharSet;
            set => m_transitionCharSet = value;
        }

        public override string ToString() {
            return m_transitionCharSet?.ToString()??"";
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

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="init"></param>
        public FA(FA init):base(init) {
            m_FAInfo = new FAGraphQueryInfo(this, m_FAINFOKEY);
            m_alphabet = new CCharRangeSet(init.M_Alphabet);
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

        public FAInfo GetFAInfo() {
            return m_FAInfo.Info();
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
        /// <summary>
        /// All FA nodes are prefixed with the given string
        /// </summary>
        /// <param name="prefix"></param>
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

        /// <summary>
        /// All nodes of the FA are marked dependent on the given input line
        /// of the regular expressions input file
        /// </summary>
        /// <param name="line"></param>
        public void SetFANodesLineDependency(uint line) {
            foreach (CGraphNode node in m_graphNodes) {
                m_FAInfo.Info(node).AddLineDependency(line);
            }
        }

        public void EmmitToFile(string filename, object[] infokeys=null) {
            StreamWriter outfile = new StreamWriter(filename);
            int i;
            outfile.WriteLine("**************************************");
            outfile.WriteLine(ToString());
            outfile.WriteLine("**************************************");
            outfile.WriteLine("Graph:");
            for (i = 0; infokeys != null && i < infokeys.Length; i++) {
                outfile.WriteLine(ToInfoString(infokeys[i]));
            }

            CIt_GraphNodes it = new CIt_GraphNodes(m_graph);
            outfile.WriteLine("Nodes:");
            for (i = 0; infokeys != null && i < infokeys.Length; i++) {
                for (it.Begin();!it.End();it.Next()) {
                    outfile.WriteLine(it.M_CurrentItem.ToInfoString(infokeys[i]));
                }
            }

            CIt_GraphEdges itg = new CIt_GraphEdges(m_graph);
            outfile.WriteLine("Edges:");
            for (i = 0; infokeys != null && i < infokeys.Length; i++) {
                for (itg.Begin(); !itg.End(); itg.Next()) {
                    outfile.WriteLine(itg.M_CurrentItem.ToInfoString(infokeys[i]));
                }
            }



            outfile.Close();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Initial State: " + M_Initial.M_Label +"\n");
            str.Append("Final States: ");
            foreach (CGraphNode finalState in GetFinalStates()) {
                str.Append(finalState.M_Label + ";");
            }

            str.Append("\nFA edges: \n");
            foreach (CGraphEdge edge in m_graphEdges) {
                str.Append(edge.M_Source.M_Label + "->" + edge.M_Target.M_Label+";\n");
            }
            return str.ToString();

        }
    }
}
