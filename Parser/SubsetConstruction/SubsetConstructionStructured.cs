using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.Thompson_Algorithm;
using Parser.UOPCore;

namespace Parser.SubsetConstruction {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraphLibrary;
    using GraphLibrary.Generics;
   

    namespace Parser.SubsetConstruction {

        [Serializable]
        public class CSubsetConstructionReporting {
            private Dictionary<uint,CCharRangeSet> m_alphabet= new Dictionary<uint, CCharRangeSet>();

            Dictionary<uint, List<IterationRecord>> m_iterations = new Dictionary<uint, List<IterationRecord>>();

            private Dictionary<uint, FA> m_resultDFAs=new Dictionary<uint, FA>();

            /// <summary>
            /// This dictionary records the configurations derived from each algorithm's iteration
            /// </summary>
            private Dictionary<uint,CConfigurations> m_configurations = new Dictionary<uint, CConfigurations>();

            public class IterationRecord {
                HashSet<CGraphNode> m_currentConfiguration;
                CGraphNode m_currentDFAnode;
                Dictionary<CGraphEdge,CharacterRecord> m_characterAnalysis = new Dictionary<CGraphEdge, CharacterRecord>(); 


                public HashSet<CGraphNode> M_CurrentConfiguration {
                    get => m_currentConfiguration;
                    set => m_currentConfiguration = value;
                }

                public CGraphNode M_CurrentDFANode {
                    get => m_currentDFAnode;
                    set => m_currentDFAnode = value;
                }

                internal Dictionary<CGraphEdge, CharacterRecord> M_CharacterAnalysis {
                    get => m_characterAnalysis;
                    set => m_characterAnalysis = value;
                }

                internal CharacterRecord AddCharacterRecord(HashSet<CGraphNode> edelta,
                    HashSet<CGraphNode> eclos, CGraphEdge DFAedge) {
                    if (!m_characterAnalysis.ContainsKey(DFAedge)) {
                        m_characterAnalysis[DFAedge] = new CharacterRecord() { M_ClosureOutcome = eclos, M_DeltaOutcome = edelta, M_DfAedge = DFAedge};
                    }
                    return m_characterAnalysis[DFAedge];
                }

            }

            public class CharacterRecord {
                private CCharRangeSet m_characterRangeCode = new CCharRangeSet(false);
                HashSet<CGraphNode> m_deltaOutcome;
                HashSet<CGraphNode> m_closureOutcome;
                private CGraphEdge m_DFAedge;

                public void AddCharacterCode(Int32 character) {
                    m_characterRangeCode.AddRange(character);
                }

                public CCharRangeSet M_CharacterRangeCode {
                    get => m_characterRangeCode;
                    set => m_characterRangeCode = value;
                }

                public HashSet<CGraphNode> M_DeltaOutcome {
                    get => m_deltaOutcome;
                    set => m_deltaOutcome = value;
                }

                public HashSet<CGraphNode> M_ClosureOutcome {
                    get => m_closureOutcome;
                    set => m_closureOutcome = value;
                }

                public CGraphNode M_SourceDfaNode {
                    get => m_DFAedge.M_Source;
                    
                }
                public CGraphNode M_TargetDfaNode {
                    get => m_DFAedge.M_Target;
                }
                public CGraphEdge M_DfAedge {
                    get => m_DFAedge;
                    set => m_DFAedge = value;
                }
            }

            public CSubsetConstructionReporting() {
            }

            public Dictionary<uint, FA> M_ResultDfAs {
                get => m_resultDFAs;
            }

            internal void AddNewRegularExpression(uint RE,FA mNFA) {
                if (!m_alphabet.ContainsKey(RE)) {
                    m_alphabet[RE] = mNFA.M_Alphabet;
                }
            }
            
            internal IterationRecord AddIteration(uint RE,HashSet<CGraphNode> configuration, CGraphNode DFANode) {
                if (!m_iterations.ContainsKey(RE)) {
                    m_iterations[RE] = new List<IterationRecord>();
                }
                IterationRecord newRecord = new IterationRecord() { M_CurrentConfiguration = configuration,
                                                                    M_CurrentDFANode = DFANode };
                m_iterations[RE].Add(newRecord);
                return newRecord;
            }

            public void AddDFA(uint RE, FA DFA) {
                if (!m_resultDFAs.ContainsKey(RE)) {
                    m_resultDFAs[RE] = DFA;
                }
            }

            /// <summary>
            /// At the end of subset construction for each regular expression when configurations
            /// have been produced call this method to update the configurations field in
            /// the current class object
            /// </summary>
            /// <param name="conf"></param>
            public void UpdateConfigurations(CConfigurations conf, uint iteration) {
                m_configurations[iteration] = conf;
            }

            public void Report(string filename) {

                StreamWriter wstream = new StreamWriter(filename);
                int it = 0;

                foreach (KeyValuePair<uint, List<IterationRecord>> iteration in m_iterations) {
                    wstream.WriteLine("\n\nRE : {0}",iteration.Key);
                    it = 0;
                    foreach (IterationRecord iterationRecord in iteration.Value) {
                        wstream.WriteLine("\n\n\tIteration : {0}",it);
                        wstream.Write("\tConfiguration : ");
                        foreach (CGraphNode graphNode in iterationRecord.M_CurrentConfiguration) {
                            wstream.Write("{0},",graphNode.M_Label);
                        }
                        wstream.WriteLine();
                        wstream.WriteLine("\tDFA Node : {0}", iterationRecord.M_CurrentDFANode.M_Label);
                        foreach (KeyValuePair<CGraphEdge, CharacterRecord> charRecord in iterationRecord.M_CharacterAnalysis) {
                            wstream.WriteLine("\t\tCharacter : {0}", charRecord.Value.M_CharacterRangeCode);
                            wstream.Write("\t\t\tDelta Configuration : ");
                            foreach (CGraphNode graphNode in charRecord.Value.M_DeltaOutcome) {
                                wstream.Write("{0},", graphNode.M_Label);
                            }
                            wstream.WriteLine();
                            wstream.Write("\t\t\tClosure Configuration : ");
                            foreach (CGraphNode graphNode in charRecord.Value.M_ClosureOutcome) {
                                wstream.Write("{0},", graphNode.M_Label);
                            }
                            wstream.WriteLine();
                            wstream.WriteLine("\t\t\tTarget DFA Node : {0}", charRecord.Value.M_DfAedge.M_Target.M_Label);
                        }
                        it++;
                    }
                    wstream.Write("\n\nNet RE {0} Result :",iteration.Key);
                    CIt_GraphNodes itf =  new CIt_GraphNodes(M_ResultDfAs[iteration.Key]);
                    for (itf.Begin(); !itf.End(); itf.Next()) {
                        wstream.Write("{0} (",itf.M_CurrentItem.M_Label);
                        if (Facade.GetOperationModeCode()) {
                            foreach (CGraphNode node in m_configurations[iteration.Key]
                                .GetDFANodeConfiguration(itf.M_CurrentItem)) {
                                wstream.Write("{0},", node.M_Label);
                            }

                            wstream.Write("),");
                        }
                        else {
                            wstream.Write("{0} (", itf.M_CurrentItem.M_Label);
                            if (Facade.GetOperationModeCode()) {
                                foreach (CGraphNode node in m_configurations[0]
                                    .GetDFANodeConfiguration(itf.M_CurrentItem)) {
                                    wstream.Write("{0},", node.M_Label);
                                }
                                wstream.Write("),");
                            }
                        }
                    }
                }
                wstream.Close();
            }
        }

        public class CSubsetConstructionStructuredAlgorithm : CGraphAlgorithm<int> {
            //inputs
            private Dictionary<uint,RERecord> m_NFAs;
            //outputs
            private FA m_currentDFA;

            private FA m_currentNFA;

            private uint m_currentRE;

            private FAGraphQueryInfo m_DFAInfo;

            private CSubsetConstructionReporting m_reporting;

            public Dictionary<uint,RERecord> M_RERecords{
                get { return m_NFAs; }
            }

            private object m_key;
            //internals
            private CConfigurations m_configurations;
            private List<string> m_alphabet;
            private Queue<HashSet<CGraphNode>> m_workList = new Queue<HashSet<CGraphNode>>();

            
            public void Start() {
                foreach (KeyValuePair<uint, RERecord> keyValuePair in m_NFAs) {
                    m_currentNFA = keyValuePair.Value.M_Nfa;
                    m_currentRE = keyValuePair.Key;
                    m_NFAs[m_currentRE].M_Dfa= Start0();

                    // DEBUG
                    m_reporting.AddNewRegularExpression(m_currentRE,m_currentNFA);
                }
            }
            
            public FA Start0() {
                GraphLibrary.CGraphNode Qprime, Q;
                CGraphEdge e;
                CCharRangeSet set;
                //q0 ← -closure({n0});

                HashSet<CGraphNode> q0 = CEclosureAlgorithm.Init(m_currentNFA, new HashSet<CGraphNode>() { m_currentNFA.M_Initial }).Start();

                m_currentNFA.UpdateAlphabet();

                // Create DFA
                m_currentDFA = new FA();
                // DEBUG 
                m_reporting.AddDFA(m_currentRE,m_currentDFA);

                m_DFAInfo = new FAGraphQueryInfo(m_currentDFA, FA.m_FAINFOKEY);
                m_configurations = new CConfigurations(m_currentDFA, m_currentNFA);
                m_configurations.CreateDFANode(q0);

                /// ******************** DEBUG *************************
                m_reporting.UpdateConfigurations(m_configurations, m_currentRE);
                
                //WorkList ← { q0};
                m_workList.Enqueue(q0);

                while (m_workList.Count != 0) {
                    // Dequeue one of the resulted states set for subsequent analysis
                    HashSet<CGraphNode> q = m_workList.Dequeue();
                    Q = m_configurations.GetDFANode(q);

                    // ********* DEBUG *************
                    CSubsetConstructionReporting.IterationRecord cRecord = m_reporting.AddIteration(m_currentRE,q,Q);
                    
                    // for each NFA alphabet character
                    foreach (CCharRange range in m_currentNFA.M_Alphabet) {
                        // For each character-range in the NFA alphabet
                        foreach (Int32 i in range) {
                            // For each character in the current character range

                            // Calculate the the target nodes given the current states (q) where the 
                            // NFA is in and the characters in the current character-range
                            CDeltaAlgorithm delta = CDeltaAlgorithm.Init(m_currentNFA, i, q);

                            // Calculate the NFA states we arrive from the states resulted from the delta algorithm
                            // through e-edges
                            HashSet<CGraphNode> deltaResult = delta.Start();
                            CEclosureAlgorithm eClosure = CEclosureAlgorithm.Init(m_currentNFA, deltaResult);
                            HashSet<CGraphNode> qprime = eClosure.Start();

                            
                            if (qprime.Count != 0) {
                                // Check if the resulted state has already being recorded...
                                Qprime = m_configurations.GetDFANode(qprime);

                                if (Qprime == null) {
                                    // ... if not record the new state and store for subsequent
                                    // analysis
                                    m_workList.Enqueue(qprime);
                                    Qprime = m_configurations.CreateDFANode(qprime);
                                }

                                // Check if an edge between Q and Qprime alredy exists...
                                e = m_currentDFA.Edge(Q, Qprime);
                                if (e == null) {
                                    /// ... if not add a new edge to the resulting DFA 
                                    e = m_currentDFA.AddGraphEdge<CGraphEdge, CGraphNode>(Q, Qprime, GraphType.GT_DIRECTED);
                                    set = new CCharRangeSet(false);
                                    m_DFAInfo.SetDFAEdgeTransitionCharacterSet(e, set);
                                }
                                else {
                                    set = m_DFAInfo.GetDFAEdgeTransitionCharacterSet(e);
                                }
                                // append the edge with the current character
                                set.AddRange(i);

                                // ********** DEBUG ************
                                CSubsetConstructionReporting.CharacterRecord charRecord =cRecord.AddCharacterRecord( deltaResult, qprime, e);
                                charRecord.AddCharacterCode(i);
                            }
                        }
                    }
                }
                // Update the DFA alphabet to reflect all the characters appearing 
                // on the edges
                m_currentDFA.UpdateAlphabet();
                
                m_currentDFA.RegisterGraphPrinter(new FAGraphVizPrinter(m_currentDFA, new UOPCore.Options<ThompsonOptions>()));
                m_currentDFA.Generate(@"mergeDFA"+m_currentRE+".dot", true);
                

                // DEBUG
                m_reporting.Report("SubsetREPORT.txt");

                return m_currentDFA;
            }
            public override void Init() {
                //initial configuration
            }
            
            public CSubsetConstructionStructuredAlgorithm(Dictionary<uint,RERecord> NFAs) {
                m_NFAs = NFAs;
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
                m_NFAStateInfo = new FAGraphQueryInfo(m_NFA, FA.m_FAINFOKEY);
                m_DFAStateInfo = new FAGraphQueryInfo(m_DFA, FA.m_FAINFOKEY);
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
                HashSet<string> prefixes = new HashSet<string>();
                string prefix = "";
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
                            m_DFA.SetFANodeLineDependency(lineDependency, DFAnode);
                        }
                    }

                    foreach (string s in prefixes) {
                        prefix += s;
                    }
                    m_DFA.PrefixElementLabel(prefix, DFAnode);


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

            /// <summary>
            /// Returns the NFA nodes included in a DFA node
            /// </summary>
            /// <param name="node">The DFA node</param>
            /// <returns></returns>
            public List<CGraphNode> GetDFANodeConfiguration(CGraphNode node) {
                return new List<CGraphNode>(m_mappings[node]);
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

        public class CEclosureAlgorithm : CGraphAlgorithm<HashSet<CGraphNode>> {

            private FA m_graph;
            private HashSet<CGraphNode> m_inputSet;
            private HashSet<CGraphNode> m_outputSet = new HashSet<CGraphNode>();

            private CEclosureAlgorithm(FA mGraph, HashSet<CGraphNode> mInputSet) {
                m_graph = mGraph;
                m_inputSet = mInputSet;
            }

            public override void Init() {
                throw new NotImplementedException();
            }

            public static CEclosureAlgorithm Init(FA g, HashSet<CGraphNode> set) {
                //Make some checks on the input arguments

                // Create algorithm
                CEclosureAlgorithm newObjectAlgorithm = new CEclosureAlgorithm(g, set);



                return newObjectAlgorithm;
            }

            public HashSet<CGraphNode> Start() {
                foreach (CGraphNode node in m_inputSet) {
                    m_outputSet = Visit(node);
                }
                return m_outputSet;
            }
            public override HashSet<CGraphNode> Visit(CGraphNode node) {

                m_outputSet.Add(node);

                //1.For each successor scan the nodes that have outgoing e transitions
                CIt_Successors it = new CIt_Successors(node);
                for (it.Begin(); !it.End(); it.Next()) {
                    //Find the edge between node and its successor
                    CGraphEdge edge = m_graph.Edge(node, it.M_CurrentItem);

                    //if the edge label is e then visit the successor and append the output set including this successor
                    if (TransitionLabel(edge) == null) {
                        Visit(it.M_CurrentItem);
                    }
                }
                //
                return m_outputSet;
            }

            public override HashSet<CGraphNode> Run() {
                throw new NotImplementedException();
            }

            public CCharRangeSet TransitionLabel(CGraphEdge edge) {
                return m_graph.GetFAEdgeInfo(edge);
            }
        }

        public class CDeltaAlgorithm : CGraphAlgorithm<HashSet<CGraphNode>> {
            private FA m_graph;
            private Int32 m_character;
            private HashSet<CGraphNode> m_inputSet;
            private HashSet<CGraphNode> m_outputSet = new HashSet<CGraphNode>();

            public override void Init() {
                throw new NotImplementedException();
            }

            public CDeltaAlgorithm(FA mGraph, Int32 character, HashSet<CGraphNode> mInputSet) {
                m_graph = mGraph;
                m_inputSet = mInputSet;
                m_character = character;
            }

            public static CDeltaAlgorithm Init(FA g, Int32 c, HashSet<CGraphNode> set) {
                //Make some checks on the input arguments

                // Create algorithm
                CDeltaAlgorithm newObjectAlgorithm = new CDeltaAlgorithm(g, c, set);

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
                for (it.Begin(); !it.End(); it.Next()) {
                    CGraphEdge edge = m_graph.Edge(node, it.M_CurrentItem);
                    set = m_graph.GetFAEdgeInfo(edge);
                    if (set != null && set.IsCharInSet(m_character)) {
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
}
