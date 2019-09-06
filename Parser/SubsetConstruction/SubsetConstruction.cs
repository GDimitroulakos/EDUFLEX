using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.SubsetConstruction.Parser.SubsetConstruction;
using Parser.Thompson_Algorithm;
using Parser.UOPCore;

namespace Parser.SubsetConstruction
{

    public class CSubsetConstructionAlgorithm:CGraphAlgorithm<int>{
        //inputs
        private FA m_NFA;
        //outputs
        private FA m_DFA;

        private FAGraphQueryInfo m_DFAInfo;

        private CSubsetConstructionReporting m_reporting;

        
        public FA Nfa {
            get { return m_NFA; }
        }

       private object m_key;
        //internals
        private CConfigurations m_configurations;
        private List<string> m_alphabet;
        private Queue<HashSet<CGraphNode>> m_workList = new Queue<HashSet<CGraphNode>>();

        public FA Dfa {
            get { return m_DFA; }
        }

        public HashSet<CGraphNode> Start() {
            GraphLibrary.CGraphNode Qprime,Q;
            CGraphEdge e;
            CCharRangeSet set;
            //q0 ← -closure({n0});
            HashSet<CGraphNode> q0 = CEclosureAlgorithm.Init(m_NFA, new HashSet<CGraphNode>() { m_NFA.M_Initial }).Start();

            m_NFA.UpdateAlphabet();

            // Create DFA
            m_DFA = new FA();

            // DEBUG 
            m_reporting.AddDFA(0, m_DFA);

            m_DFAInfo = new FAGraphQueryInfo(m_DFA,FA.m_FAINFOKEY);
            m_configurations = new CConfigurations(m_DFA,m_NFA);
            m_configurations.CreateDFANode(q0);
           
            //WorkList ← { q0};
            m_workList.Enqueue(q0);

            while (m_workList.Count != 0){
                HashSet<CGraphNode> q = m_workList.Dequeue();
                Q = m_configurations.GetDFANode(q);

                // ********* DEBUG *************
                CSubsetConstructionReporting.IterationRecord cRecord = m_reporting.AddIteration(0, q, Q);

                // for each NFA alphabet character
                foreach (CCharRange range in m_NFA.M_Alphabet) {
                    foreach (Int32 i in range) {
                        CDeltaAlgorithm delta = CDeltaAlgorithm.Init(m_NFA, i, q);
                        HashSet<CGraphNode> deltaResult = delta.Start();
                        CEclosureAlgorithm eClosure = CEclosureAlgorithm.Init(m_NFA, deltaResult);
                        HashSet<CGraphNode> qprime = eClosure.Start();
                        if (qprime.Count != 0) {
                            Qprime = m_configurations.GetDFANode(qprime);
                            if (Qprime == null) {
                                m_workList.Enqueue(qprime);
                                Qprime = m_configurations.CreateDFANode(qprime);
                            }
                            // Check if an edge between Q and Qprime alredy exists
                            e=m_DFA.Edge(Q, Qprime);
                            if (e == null) {
                                e = m_DFA.AddGraphEdge<CGraphEdge, CGraphNode>(Q, Qprime, GraphType.GT_DIRECTED);
                                set = new CCharRangeSet(false);
                                m_DFAInfo.SetDFAEdgeTransitionCharacterSet(e, set);
                            }
                            else {
                                set = m_DFAInfo.GetDFAEdgeTransitionCharacterSet(e);
                            }
                            set.AddRange(i);

                            // ********** DEBUG ************
                            CSubsetConstructionReporting.CharacterRecord charRecord = cRecord.AddCharacterRecord(deltaResult, qprime, e);
                            charRecord.AddCharacterCode(i);
                        }
                    }
                }
            }
            m_DFA.UpdateAlphabet();
            m_DFA.RegisterGraphPrinter(new ThompsonGraphVizPrinter(m_DFA,new UOPCore.Options<ThompsonOptions>()));
            m_DFA.Generate(@"../Debug/mergeDFA.dot", true);

            // DEBUG
            m_reporting.Report("SubsetREPORT.txt");
            return null;
        }
        public override void Init(){
            //initial configuration
        }
        public static CSubsetConstructionAlgorithm Init(FA g) {
            //Make some checks on the input arguments

            // Create algorithm
            CSubsetConstructionAlgorithm newObjectAlgorithm = new CSubsetConstructionAlgorithm(g);
                return newObjectAlgorithm;
        }
        public CSubsetConstructionAlgorithm(FA mNfa){
            m_NFA = mNfa;
            m_reporting = new CSubsetConstructionReporting();
        }

        public override int Run() {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// This class records the correspondence between the DFA and NFA nodes  
    /// </summary>
    public class CConfigurations {
        private FA m_DFA;
        private FA m_NFA;
        /// <summary>
        /// Maps a DFA node(key) to a set of NFA nodes(value)
        /// </summary>
        private Dictionary<CGraphNode, HashSet<CGraphNode>> m_mappings;

        private FAGraphQueryInfo m_NFAStateInfo;
        private FAGraphQueryInfo m_DFAStateInfo;

        public CConfigurations(FA DFA, FA NFA) {
            m_DFA = DFA;
            m_NFA = NFA;
            m_mappings = new Dictionary<CGraphNode, HashSet<CGraphNode>>();
            m_NFAStateInfo=new FAGraphQueryInfo(m_NFA,FA.m_FAINFOKEY);
            m_DFAStateInfo= new FAGraphQueryInfo(m_DFA,FA.m_FAINFOKEY);
        }

        /// <summary>
        /// Creates a unique dfa node for a given configuration
        /// The method assures that the configuration-dfa node mapping
        /// is unique
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns></returns>
        public CGraphNode CreateDFANode(HashSet<CGraphNode> q) {
            CGraphNode DFAnode = null;
            HashSet<string> prefixes=new HashSet<string>();
            string prefix="";
            //check if q configuration corresponds to a DFA state
            DFAnode = GetDFANode(q);
            //if not create a new DFA node
            if (DFAnode == null) {
                DFAnode = m_DFA.CreateGraphNode<CGraphNode>();
                
                foreach (CGraphNode node in q) {
                    if (!prefixes.Contains(m_NFAStateInfo.Info(node).M_NodeLabelPrefix)) {
                        prefixes.Add(m_NFAStateInfo.Info(node).M_NodeLabelPrefix);
                        m_DFAStateInfo.Info(DFAnode).M_NodeLabelPrefix = m_NFAStateInfo.Info(node).M_NodeLabelPrefix;
                    }

                    foreach (uint lineDependency in m_NFAStateInfo.Info(node).M_LineDependencies) {
                        m_DFA.SetFANodeLineDependency(lineDependency,DFAnode);
                    }
                }

                foreach (string s in prefixes) {
                    prefix += s;
                }
                m_DFA.PrefixElementLabel(prefix,DFAnode);

                
                if (ContainsFinalState(q)) {
                    m_DFA.SetFinalState(DFAnode);
                }

                if (ContainsInitialState(q)) {
                    m_DFA.M_Initial = DFAnode;
                }
                m_mappings[DFAnode] = q;
            }
            //else return the existing DFA node
            return DFAnode;
        }
        /// <summary>
        /// Checks to find existing NFA configurations mapped to
        /// existing DFA states with the given set of NFA nodes.
        /// If it finds equivalent configuration it returns the
        /// corresponding DFA state
        /// </summary>
        /// <param name="q">NFA nodes set</param>
        /// <returns>null or the corresponding DFA state</returns>
        public CGraphNode GetDFANode(HashSet<CGraphNode> q) {
            CGraphNode DFAnode = null;
            foreach (CGraphNode dfanode in m_mappings.Keys) {
                if (m_mappings[dfanode].SetEquals(q)) {
                    DFAnode = dfanode;
                }
            }
            return DFAnode;
        }

        public bool ContainsFinalState(HashSet<CGraphNode> q) {
            List<CGraphNode> finalStates = m_NFA.GetFinalStates();
            foreach (CGraphNode node in q) {
                if (finalStates.Contains(node)) {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsInitialState(HashSet<CGraphNode> q) {
            CGraphNode initiaNFAState = m_NFA.M_Initial;
            foreach (CGraphNode node in q) {
                if (initiaNFAState == node) {
                    return true;
                }
            }
            return false;
        }

    }

    public class CEclosureAlgorithm :CGraphAlgorithm<HashSet<CGraphNode>>{

        private FA m_graph;
        private HashSet<CGraphNode> m_inputSet;
        private HashSet<CGraphNode> m_outputSet=new HashSet<CGraphNode>();

        private CEclosureAlgorithm(FA mGraph, HashSet<CGraphNode> mInputSet){
            m_graph = mGraph;
            m_inputSet = mInputSet;
        }

        public override void Init(){
            throw new NotImplementedException();
        }

        public static CEclosureAlgorithm Init(FA g,HashSet<CGraphNode> set){
            //Make some checks on the input arguments

            // Create algorithm
            CEclosureAlgorithm newObjectAlgorithm = new CEclosureAlgorithm(g,set);



            return newObjectAlgorithm;
        }

        public HashSet<CGraphNode> Start(){
            foreach (CGraphNode node in m_inputSet){
                m_outputSet = Visit(node);
            }
            return m_outputSet;
        }
        public override HashSet<CGraphNode> Visit(CGraphNode node){

            m_outputSet.Add(node);

            //1.For each successor scan the nodes that have outgoing e transitions
            CIt_Successors it = new CIt_Successors(node);
            for (it.Begin(); !it.End(); it.Next()){
                //Find the edge between node and its successor
                CGraphEdge edge = m_graph.Edge(node, it.M_CurrentItem);

                //if the edge label is e then visit the successor and append the output set including this successor
                if (TransitionLabel(edge)==null){
                    Visit(it.M_CurrentItem);
                }
            }
            //
            return m_outputSet;
        }

        public override HashSet<CGraphNode> Run() {
            throw new NotImplementedException();
        }

        public CCharRangeSet TransitionLabel(CGraphEdge edge){
            return m_graph.GetFAEdgeInfo(edge);
        }
    }

    public class CDeltaAlgorithm: CGraphAlgorithm<HashSet<CGraphNode>>{
        private FA m_graph;
        private Int32 m_character;
        private HashSet<CGraphNode> m_inputSet;
        private HashSet<CGraphNode> m_outputSet = new HashSet<CGraphNode>();

        public override void Init(){
            throw new NotImplementedException();
        }

        public CDeltaAlgorithm(FA mGraph, Int32 character, HashSet<CGraphNode> mInputSet){
            m_graph = mGraph;
            m_inputSet = mInputSet;
            m_character = character;
        }

        public static CDeltaAlgorithm Init(FA g, Int32 c, HashSet<CGraphNode> set) {
            //Make some checks on the input arguments

            // Create algorithm
            CDeltaAlgorithm newObjectAlgorithm = new CDeltaAlgorithm(g,c,set);

            return newObjectAlgorithm;
        }

        public HashSet<CGraphNode> Start() {
            foreach (CGraphNode node in m_inputSet) {
                m_outputSet = Visit(node);
            }
            return m_outputSet;
        }

        public override HashSet<CGraphNode> Visit(CGraphNode node) {
            CCharRangeSet set;
            CIt_Successors it = new CIt_Successors(node);
            for (it.Begin(); !it.End(); it.Next()){
                CGraphEdge edge = m_graph.Edge(node, it.M_CurrentItem);
                set = m_graph.GetFAEdgeInfo(edge);
                if (set != null && set.IsCharInSet(m_character)){
                    m_outputSet.Add(it.M_CurrentItem);
                }
            }
            return m_outputSet;
        }

        public override HashSet<CGraphNode> Run() {
            throw new NotImplementedException();
        }
    }
}