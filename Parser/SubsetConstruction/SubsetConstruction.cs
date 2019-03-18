using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.UOPCore;

namespace Parser.SubsetConstruction
{
    public class CSubsetConstructionAlgorithm:CGraphAlgorithm<int>{
        //inputs
        private FA m_NFA;
        //private CGraphQueryInfo m_NFATransitions;
        //outputs
        private FA m_DFA;

        //private CGraphQueryInfo m_DFATransitions;
        public FA Nfa {
            get { return m_NFA; }
        }

       private object m_key;
        //internals
        //private CGraphQueryInfo m_configurations;
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
            m_configurations = new CConfigurations(m_DFA,m_NFA);
            m_configurations.CreateDFANode(q0);
           
            //WorkList ← { q0};
            m_workList.Enqueue(q0);

            while (m_workList.Count != 0){
                HashSet<CGraphNode> q = m_workList.Dequeue();
                Q = m_configurations.GetDFANode(q);


                // for each NFA alphabet character
                foreach (CCharRange range in m_NFA.M_Alphabet) {
                    foreach (Int32 i in range) {
                        CDeltaAlgorithm delta = CDeltaAlgorithm.Init(m_NFA, i, q);
                        CEclosureAlgorithm eClosure = CEclosureAlgorithm.Init(m_NFA,delta.Start());
                        HashSet<CGraphNode> qprime = eClosure.Start();
                        if (qprime.Count != 0) {
                            Qprime = m_configurations.GetDFANode(qprime);
                            if (Qprime == null) {
                                m_workList.Enqueue(qprime);
                                Qprime = m_configurations.CreateDFANode(qprime);
                            }
                            e= m_DFA.AddGraphEdge(Q, Qprime, GraphType.GT_DIRECTED);
                            set = new CCharRangeSet(false);
                            set.AddRange(range);
                            e[FA.m_TRANSITIONSKEY] = set;
                        }
                    }
                }
            }
            m_DFA.UpdateAlphabet();
            m_DFA.RegisterGraphPrinter(new ThompsonGraphVizPrinter(m_DFA));
            m_DFA.Generate(@"../Debug/mergeDFA.dot", true);
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
        }

    }

    public class CConfigurations {
        private FA m_DFA;
        private FA m_NFA;
        private Dictionary<CGraphNode, HashSet<CGraphNode>> m_mappings;

        public CConfigurations(FA DFA, FA NFA) {
            m_DFA = DFA;
            m_NFA = NFA;
            m_mappings = new Dictionary<CGraphNode, HashSet<CGraphNode>>();
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

            DFAnode = GetDFANode(q);

            if (DFAnode == null) {
                DFAnode = m_DFA.CreateGraphNode();
                if (ContainsFinalState(q)) {
                    m_DFA.SetFinalState(DFAnode);
                }
                m_mappings[DFAnode] = q;
            }

            return DFAnode;
        }

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

        public CCharRangeSet TransitionLabel(CGraphEdge edge){
            return m_graph.GetEdgeInfo(edge);
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
                set = m_graph.GetEdgeInfo(edge);
                if (set != null && set.IsCharInSet(m_character)){
                    m_outputSet.Add(it.M_CurrentItem);
                }
            }
            return m_outputSet;
        }
    }
}