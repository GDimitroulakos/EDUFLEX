using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.Thompson_Algorithm;
using Parser.UOPCore;
using System.IO;

namespace Parser.Hopcroft {
    
    public abstract class CHopcroftSplitEvent {
        protected int m_level;

        public enum HopcroftEventType { ET_BEHAVIOURRECORDING, ET_SPLITACTION }

        // Node under inspection for splitting
        private CGraphNode m_inspectionNode;

        private HopcroftEventType m_eventType;
        public HopcroftEventType M_EventType {
            get => m_eventType;
            set => m_eventType = value;
        }
        public CGraphNode M_InspectionNode {
            get => m_inspectionNode;
            set => m_inspectionNode = value;
        }

        protected CHopcroftSplitEvent(CGraphNode inspectionNode, HopcroftEventType eventType, int level) {
            m_inspectionNode = inspectionNode;
            m_eventType = eventType;
            m_level = level;
        }

    }

    public class CHopcroftBehaviourRecording : CHopcroftSplitEvent {
        // Behaviour exhibited by nodes inside the inspection node
        // <source configuration node,character,target configuration node, target configuration>
        private Tuple<CGraphNode, Int32, CGraphNode, CGraphNode> m_behaviour;

        public CHopcroftBehaviourRecording(Tuple<CGraphNode, int, CGraphNode, CGraphNode> behaviour , CGraphNode inspectionNode, int level):base(inspectionNode, HopcroftEventType.ET_BEHAVIOURRECORDING,level) {
            m_behaviour = behaviour;
        }

        public CGraphNode MSourceInternalNode {
            get => m_behaviour.Item1;
        }

        public char MCharacter {
            get => (char)m_behaviour.Item2;
        }

        public CGraphNode MTargetInternalNode {
            get => m_behaviour.Item3;
        }

        public CGraphNode MTargetNode {
            get => m_behaviour.Item4;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append("\n");
            for (int i=0; i<m_level; i++) {
                s.Append("\t");
            }
            s.Append(M_InspectionNode.M_Label + "(" + MSourceInternalNode.M_Label +
                   ")" + "----" + MCharacter + "--->" + MTargetNode.M_Label + "(" +
                   (MTargetInternalNode?.M_Label ?? "") + ")");
            return s.ToString();
        }
    }

    public class CHopcroftSplitAction : CHopcroftSplitEvent {
        /// <summary>
        /// Represents the character causing the split, the new node of minimized DFA
        /// that is born
        /// </summary>
        private Tuple<Int32, CGraphNode> m_split;

        /// <summary>
        /// The following collections reflect the contents of the minimized DFA nodes
        /// taking part to the split
        /// </summary>
        private List<CGraphNode> m_resultingInitialNodeConfiguration;
        private List<CGraphNode> m_resultingSplittedNodeConfiguration;

        public CHopcroftSplitAction(Tuple<int, CGraphNode> split, CGraphNode inspectionNode,
            List<CGraphNode> resultingInitialNodeConfiguration, List<CGraphNode> resultingSplittedNodeConfiguration,
            int level) : base(inspectionNode, HopcroftEventType.ET_SPLITACTION, level) {
            m_split = split;
            m_resultingInitialNodeConfiguration = new List<CGraphNode>(resultingInitialNodeConfiguration);
            m_resultingSplittedNodeConfiguration = new List<CGraphNode>(resultingSplittedNodeConfiguration);
        }


        public char MCharacter {
            get => (char)m_split.Item1;
        }

        public CGraphNode M_NewSplittedNode {
            get => m_split.Item2;
        }

        public List<CGraphNode> M_ResultingInitialNodeConfiguration {
            get => m_resultingInitialNodeConfiguration;
            set => m_resultingInitialNodeConfiguration = value;
        }

        public List<CGraphNode> M_ResultingSplittedNodeConfiguration {
            get => m_resultingSplittedNodeConfiguration;
            set => m_resultingSplittedNodeConfiguration = value;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append("\n");
            for (int i = 0; i < m_level; i++) {
                s.Append("\t");
            }
            s.Append("Split:"+M_InspectionNode.M_Label + "----" + MCharacter + "--->" + M_NewSplittedNode.M_Label);
            s.Append("\n");
            for (int i = 0; i < m_level; i++) {
                s.Append("\t\t");
            }
            s.Append("Resulting Nodes Configuration:" + M_InspectionNode.M_Label + "(");
            foreach (CGraphNode node in m_resultingInitialNodeConfiguration) {
                s.Append(node.M_Label + ",");
            }
            s.Append(")   "+ M_NewSplittedNode.M_Label + "(");
            foreach (CGraphNode node in m_resultingSplittedNodeConfiguration) {
                s.Append(node.M_Label + ",");
            }
            s.Append(")");
            return s.ToString();
        }
    }

    public class CHopcroftIterationEvent {
        protected int m_level;
        public enum ITerationEvent { IT_ADDINSPECTIONODE, IT_ADDITERATION, IT_SPLITPROCESS }

        private ITerationEvent m_IterationEventType;

        public ITerationEvent M_IterationEventType {
            get => m_IterationEventType;
            set => m_IterationEventType = value;
        }

        public CHopcroftIterationEvent(ITerationEvent iterationEventType, int level) {
            this.m_level = level;
            m_IterationEventType = iterationEventType;
        }
    }

    public class CHopcroftSplitRecord : CHopcroftIterationEvent {

        // FIELDS
        // Node under inspection for splitting
        private CGraphNode m_inspectionNode;

        // Events during the split process
        private List<CHopcroftSplitEvent> m_splitEvents = new List<CHopcroftSplitEvent>();

        public CHopcroftSplitRecord(int level, CGraphNode inspectionNode) : base(ITerationEvent.IT_SPLITPROCESS, level) {
            m_inspectionNode = inspectionNode;
        }

        // PROPERTIES
        public CGraphNode M_InspectionNode {
            get => m_inspectionNode;
            set => m_inspectionNode = value;
        }
        public List<CHopcroftSplitEvent> M_SplitEvents => m_splitEvents;

        public void AddBehaviour(Tuple<CGraphNode, int, CGraphNode, CGraphNode> behaviour) {
            m_splitEvents.Add(new CHopcroftBehaviourRecording(behaviour,m_inspectionNode,m_level+1));
        }

        public CHopcroftSplitAction AddSplit(Tuple<int, CGraphNode> split,
            List<CGraphNode> resultingInitialNodeConfiguration, List<CGraphNode> resultingSplittedNodeConfiguration) {
            CHopcroftSplitAction res;
            m_splitEvents.Add(res =new CHopcroftSplitAction(split,m_inspectionNode,resultingInitialNodeConfiguration,resultingSplittedNodeConfiguration,m_level+1));
            return res;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append("\n\n\n");
            for (int i = 0; i < m_level; i++) {
                s.Append("\t");
            }
            s.Append("Node to Inspect for Splitting: "+ m_inspectionNode.M_Label);
            foreach (CHopcroftSplitEvent splitEvent in m_splitEvents) {
                s.Append(splitEvent.ToString());
            }

            return s.ToString();
        }
    }


    public class CIterationRecord {
        protected int m_level;
        private int m_iteration;

        private Dictionary<CGraphNode, List<CGraphNode>> m_initialNodesConfiguration = new Dictionary<CGraphNode, List<CGraphNode>>();
        // Nodes to inspect for splitting in the current iteration
        private List<CGraphNode> m_nodesToInspect = new List<CGraphNode>();

        // Splitting actions record during the iteration
        private List<CHopcroftIterationEvent> m_IterationEvents = new List<CHopcroftIterationEvent>();

        private CHopcroftSplitRecord m_currentSplitAction;

        public CHopcroftSplitRecord M_CurrentSplitAction => m_currentSplitAction;

        public List<CGraphNode> M_NodesToInspect {
            get => m_nodesToInspect;
        }

        public List<CHopcroftIterationEvent> M_IterationEvents => m_IterationEvents;

        public Dictionary<CGraphNode, List<CGraphNode>> M_InitialNodesConfiguration => m_initialNodesConfiguration;

        public int M_Iteration {
            get => m_iteration;
        }

        public CIterationRecord(int iteration, int level) {
            m_level = level;
            m_iteration = iteration;
        }

        public void SaveInitialNodesConfiguration(CGraphNode node, List<CGraphNode> nodeConfiguration) {
            m_initialNodesConfiguration[node] = nodeConfiguration;
        }

        public void AddNodeToInpect(CGraphNode node) {
            m_nodesToInspect.Add(node);
        }

        public CHopcroftSplitRecord AddSplitAction(CGraphNode nodeToSplit) {
            m_IterationEvents.Add(m_currentSplitAction = new CHopcroftSplitRecord(m_level+1,nodeToSplit));
            return m_currentSplitAction;
        }

        public void AddBehaviourRecording(Tuple<CGraphNode, int, CGraphNode, CGraphNode> behaviour) {
            M_CurrentSplitAction.AddBehaviour(behaviour);
        }

        public CHopcroftSplitAction AddSplitRecording(Tuple<int, CGraphNode> split,
            List<CGraphNode> resultingInitialNodeConfiguration, List<CGraphNode> resultingSplittedNodeConfiguration) {
            return M_CurrentSplitAction.AddSplit(split,resultingInitialNodeConfiguration,resultingSplittedNodeConfiguration);
        }

    }


    public class CHopcroftReporting {
        private CGraphNode m_InitialConfigurationForAcceptedNodes;
        private List<CGraphNode> m_initialAcceptedStates;
        private CGraphNode m_InitialConfigurationForNonAcceptedNodes;
        private List<CGraphNode> m_initialNonAcceptedStates;


        private HopcroftOutputGraphQueryInfo m_HopcroftOutputInfo;
        private HopcroftInputGraphQueryInfo m_HopcroftInputInfo;

        /// <summary>
        /// This dictionary marks the nodes in the minimized DFA that result from the spliting
        /// operation
        /// </summary>
        private List<CIterationRecord> m_ActivityRecord = new List<CIterationRecord>();

        private CIterationRecord m_currentIterationRecord;

        public CHopcroftReporting(HopcroftOutputGraphQueryInfo outinfo, HopcroftInputGraphQueryInfo ininfo) {
            m_HopcroftOutputInfo = outinfo;
            m_HopcroftInputInfo = ininfo;
        }


        public CIterationRecord AddIteration(int iteration) {
            m_ActivityRecord.Add(m_currentIterationRecord = new CIterationRecord(iteration,1));
            return m_currentIterationRecord;
        }

        public CHopcroftSplitRecord AddSplitRecord(CGraphNode nodeToSplit) {
            return m_currentIterationRecord.AddSplitAction(nodeToSplit);
        }


        public void AddBehaviour(Tuple<CGraphNode, int, CGraphNode, CGraphNode> behaviour) {
            m_currentIterationRecord.AddBehaviourRecording(behaviour);
        }

        public CHopcroftSplitAction AddSplit(Tuple<int, CGraphNode> split,
            List<CGraphNode> resultingInitialNodeConfiguration, List<CGraphNode> resultingSplittedNodeConfiguration) {
            return m_currentIterationRecord.AddSplitRecording(split,resultingInitialNodeConfiguration,resultingSplittedNodeConfiguration);
        }


        public void SetInitialMinDFAStatesContents(CGraphNode accConf, List<CGraphNode> accStates,
            CGraphNode nonAccConf, List<CGraphNode> nonaccStates) {
            m_initialAcceptedStates = new List<CGraphNode>(accStates);
            m_initialNonAcceptedStates = new List<CGraphNode>(nonaccStates);
            m_InitialConfigurationForAcceptedNodes = accConf;
            m_InitialConfigurationForNonAcceptedNodes = nonAccConf;
        }

        private string GetNodesinConfiguration(CGraphNode node) {
            StringBuilder s = new StringBuilder();
            List<CGraphNode> conf = m_HopcroftOutputInfo.NodesInConfiguration(node);
            foreach (CGraphNode n in conf) {
                s.Append(n.M_Label + ",");
            }
            return s.ToString();
        }

        private string GetNodesinInitialConfiguration(CGraphNode node, CIterationRecord itrec) {
            StringBuilder s = new StringBuilder();
            foreach (CGraphNode n in itrec.M_InitialNodesConfiguration[node]) {
                s.Append(n.M_Label + ",");
            }
            return s.ToString();
        }

        public void Report(string filename) {
            StreamWriter ostream = new StreamWriter(filename);

            ostream.WriteLine("***************** Initialization *****************");
            ostream.Write("Initial Accepted States ({0}): ", m_InitialConfigurationForAcceptedNodes.M_Label);
            foreach (var state in m_initialAcceptedStates) {
                ostream.Write("{0}, ", state.M_Label);
            }
            ostream.Write("\n\nInitial Non-Accepted States ({0}): ", m_InitialConfigurationForNonAcceptedNodes.M_Label);
            foreach (var state in m_initialNonAcceptedStates) {
                ostream.Write("{0}, ", state.M_Label);
            }


            foreach (CIterationRecord itrec in m_ActivityRecord) {
                ostream.Write("\n\nIteration: {0} ", itrec.M_Iteration);
                ostream.Write("\nNodes to inspect for splitting: ");
                foreach (CGraphNode node in itrec.M_NodesToInspect) {
                    ostream.Write(node.M_Label + "(" + GetNodesinInitialConfiguration(node, itrec) + "), ");
                }

                foreach (CHopcroftIterationEvent splitRecord in itrec.M_IterationEvents) {
                    ostream.Write(splitRecord.ToString());
                }

            }


            ostream.Close();
        }
    }

    // This class represented the Hopcroft Algorithm. The initial DFA has nodes
    // that store the configuration information under the key m_CONFIGURATIONKEY.
    // The result DFA has nodes representing the algorithm's configuration and
    // store the nodes that represent under the same key m_CONFIGURATIONKEY.
    public class CHopcroftAlgorithmStructured {
        private FA m_currentDFA;
        private uint m_currentRE;
        private FA m_currentMinimizedDFA;

        private Dictionary<uint, RERecord> m_inputDFAs;
        // ???
        private HopcroftInputGraphQueryInfo m_currentNodeConfiguration;
        // ???
        private HopcroftOutputGraphQueryInfo m_currentMinDFANodeConfiguration;

        // The key by which we can access edge transition info. One key
        // per Hopecoft algorithm instance
        public readonly string m_HOPCROFTCONFIGURATIONKEY = "configuration";

        public Dictionary<uint, RERecord> MinimizedDfas => m_inputDFAs;

        CHopcroftReporting m_reporting;

        public CHopcroftAlgorithmStructured(Dictionary<uint, RERecord> dfas) {
            m_inputDFAs = dfas;
        }

        public void Start() {
            // Apply Hopcroft algorithm separately for each Regular Expression
            foreach (KeyValuePair<uint, RERecord> rec in m_inputDFAs) {
                m_currentDFA = rec.Value.M_Dfa;
                m_currentRE = rec.Value.M_Line;
                rec.Value.M_MinDfa = m_currentMinimizedDFA = new FA();
                m_currentNodeConfiguration = new HopcroftInputGraphQueryInfo(m_currentDFA, m_HOPCROFTCONFIGURATIONKEY);
                m_currentMinDFANodeConfiguration = new HopcroftOutputGraphQueryInfo(m_currentMinimizedDFA, m_HOPCROFTCONFIGURATIONKEY);
                m_reporting = new CHopcroftReporting(m_currentMinDFANodeConfiguration, m_currentNodeConfiguration);
                Init();
            }
        }

        public void Init() {

            // List of DFA nodes in minimized DFA node
            List<CGraphNode> configurationAcc, configurationNonAcc;

            // Create configuration (min-DFA node) for accepted nodes
            CGraphNode acceptedConf = m_currentMinimizedDFA.CreateGraphNode<CGraphNode>();
            // Create a list of initial-DFA nodes that are registered as accepted nodes
            configurationAcc = new List<CGraphNode>();
            // Store the list for initial-DFA accepted nodes in the min-DFA for accepted nodes
            SetNodesInConfiguration(acceptedConf, configurationAcc);

            // Create configuration (min-DFA node) for non-accepted nodes
            CGraphNode non_acceptedConf = m_currentMinimizedDFA.CreateGraphNode<CGraphNode>();
            // Create a list for initial-DFA nodes that are registered as non-accepted nodes
            configurationNonAcc = new List<CGraphNode>();
            // Store the list of initial-DFA non-accepted node in the min-DFA for accepted nodes
            SetNodesInConfiguration(non_acceptedConf, configurationNonAcc);

            // Separate accepted from non-accepted nodes into two distinct
            // configurations. Iterate over the input DFA and place the
            // accepted node into configurationAcc and none accepted nodes 
            // into configurationNonAcc
            CIt_GraphNodes it = new CIt_GraphNodes(m_currentDFA);
            for (it.Begin(); !it.End(); it.Next()) {
                if (m_currentDFA.GetFinalStates().Contains(it.M_CurrentItem)) {
                    // Record in the input DFA node the in which minimized DFA node
                    // will be placed
                    SetNodeConfiguration(it.M_CurrentItem, acceptedConf);
                    configurationAcc.Add(it.M_CurrentItem);
                }
                else {
                    // Record in the input DFA node the in which minimized DFA node
                    // will be placed
                    SetNodeConfiguration(it.M_CurrentItem, non_acceptedConf);
                    configurationNonAcc.Add(it.M_CurrentItem);
                }
            }

            // ************************* Debug Initialization ***************************
            m_currentDFA.RegisterGraphPrinter(new FATextPrinter(m_currentDFA));
            m_currentDFA.Generate("HopCroft_InitialDFA_" + m_currentRE + ".txt");
            m_reporting.SetInitialMinDFAStatesContents(acceptedConf, configurationAcc, non_acceptedConf, configurationNonAcc);


            int nodeCount = 0;
            int iteration_count = 0;

            
            // Iterate while the algorithm reaches a fixed point state
            while (nodeCount != m_currentMinimizedDFA.M_NumberOfNodes) {

                // keep the number of nodes before applying a new split
                nodeCount = m_currentMinimizedDFA.M_NumberOfNodes;
                // ************************* Debug  ***************************
                CIterationRecord currentIteration = m_reporting.AddIteration(iteration_count);
                CIt_GraphNodes minDFA_it = new CIt_GraphNodes(m_currentMinimizedDFA, true);
                for (minDFA_it.Begin(); !minDFA_it.End(); minDFA_it.Next()) {

                    // ************************* Debug  ***************************
                    currentIteration.M_NodesToInspect.Add(minDFA_it.M_CurrentItem);
                    currentIteration.M_InitialNodesConfiguration.Add(minDFA_it.M_CurrentItem, new List<CGraphNode>(GetNodesInConfiguration(minDFA_it.M_CurrentItem)));

                    Split(minDFA_it.M_CurrentItem);
                }
                iteration_count++;
            }

            // Draw the final edges
            // Edges between nodes of the initial DFA are mapped to edges between
            // configurations in minimized-DFA. Thus, edges are drawn between two
            // min-DFA related configurations when their corresponding nodes are
            // connected and their transition character set is combined 
            CIt_GraphNodes minit1 = new CIt_GraphNodes(m_currentMinimizedDFA);
            CIt_GraphNodes minit2 = new CIt_GraphNodes(m_currentMinimizedDFA);
            List<CGraphNode> confs, conft;
            CGraphEdge edge, newedge;
            for (minit1.Begin(); !minit1.End(); minit1.Next()) {
                for (minit2.Begin(); !minit2.End(); minit2.Next()) {
                    confs = GetNodesInConfiguration(minit1.M_CurrentItem);
                    conft = GetNodesInConfiguration(minit2.M_CurrentItem);
                    foreach (CGraphNode snode in confs) {
                        foreach (CGraphNode tnode in conft) {
                            edge = m_currentDFA.Edge(snode, tnode);
                            if (edge != null) {
                                // Disallow duplicate edges between nodes of the minimized DFA
                                newedge = m_currentMinimizedDFA.Edge(minit1.M_CurrentItem, minit2.M_CurrentItem);
                                if (newedge == null) {
                                    newedge = m_currentMinimizedDFA.AddGraphEdge<CGraphEdge, CGraphNode>(minit1.M_CurrentItem, minit2.M_CurrentItem, GraphType.GT_DIRECTED);
                                }

                                // Add transition characters
                                if (GetMinDFAStateTransitionCharacterSet(newedge) == null) {
                                    SetMinDFAStateTransitionCharacterSet(newedge, new CCharRangeSet(false));
                                    m_currentMinimizedDFA.SetFAEdgeInfo(newedge, new CCharRangeSet(false));
                                }
                                CCharRangeSet charset = GetMinDFAStateTransitionCharacterSet(newedge);
                                m_currentMinimizedDFA.GetFAEdgeInfo(newedge).AddSet(m_currentDFA.GetFAEdgeInfo(edge));
                            }
                        }
                    }
                }
            }

            // Detect accepted nodes in minimized DFA
            CIt_GraphNodes it1 = new CIt_GraphNodes(m_currentMinimizedDFA);
            for (it1.Begin(); !it1.End(); it1.Next()) {
                List<CGraphNode> configuration = GetNodesInConfiguration(it1.M_CurrentItem);
                List<CGraphNode> finals = m_currentDFA.GetFinalStates();
                foreach (CGraphNode node in configuration) {
                    if (finals.Contains(node)) {
                        m_currentMinimizedDFA.SetFinalState(it1.M_CurrentItem);
                    }
                }
            }

            // Detect initial state of minimized DFA
            for (it1.Begin(); !it1.End(); it1.Next()) {
                List<CGraphNode> configuration = GetNodesInConfiguration(it1.M_CurrentItem);
                CGraphNode initial = m_currentDFA.M_Initial;
                foreach (CGraphNode node in configuration) {
                    if (initial == node) {
                        m_currentMinimizedDFA.M_Initial = it1.M_CurrentItem;
                    }
                }
            }

            // Set Final minimized-DFA labels
            for (it1.Begin(); !it1.End(); it1.Next()) {
                List<CGraphNode> conf = GetNodesInConfiguration(it1.M_CurrentItem);
                foreach (CGraphNode iNode in conf) {
                    HashSet<string> prefs = m_currentDFA.GetFANodePrefixLabels(iNode);
                    foreach (string s in prefs) {
                        m_currentMinimizedDFA.SetFANodePrefix(s, it1.M_CurrentItem);
                    }

                    foreach (uint dependency in m_currentDFA.GetFANodeLineDependencies(iNode)) {
                        m_currentMinimizedDFA.SetFANodeLineDependency(dependency, it1.M_CurrentItem);
                    }

                }
                m_currentMinimizedDFA.PrefixElementLabel(m_currentMinimizedDFA.GetFANodePrefix(it1.M_CurrentItem), it1.M_CurrentItem);
            }

            m_reporting.Report("HOPCROFTReport" + m_currentRE + ".txt");

            FASerializer serializer = new FASerializer(m_currentMinimizedDFA);
            serializer.Print();

            m_currentDFA.RegisterGraphPrinter(new FAGraphVizPrinter(m_currentMinimizedDFA, new UOPCore.Options<ThompsonOptions>()));
            m_currentDFA.Generate(@"minimizedDFA" + m_currentRE + ".dot", true);
        }
        /// <summary>
        /// Splits the specified minimized-DFA node in as many nodes as necessary.
        /// The resulting nodes exhibit the property that every internal nodes 
        /// exhibit the same behaviour for all alphabet characters.
        /// </summary>
        /// <param name="node">Represents a minimized DFA's node (current configuration)</param>
        public void Split(CGraphNode node) {
            // Holds the nodes of the initial DFA that represent the configuration 
            // for the given node (Split method parameter) of min-DFA (Minimized-DFA node)
            List<CGraphNode> currentConfiguration = GetNodesInConfiguration(node);

            List<CGraphNode> newConfiguration;

            // Holds the mapping of initial DFA for a given character
            //                   character
            // <source(DFA node)----------->target(minimizedDFA node)>
            // Each iteration of the following loops considers the transition for
            // each distinct character of the alphabet. Thus, in every iteration 
            // concerning character this dictionary is updated
            // <Considered as input >
            Dictionary<CGraphNode, CGraphNode> m_CharConfigurationMappings = new Dictionary<CGraphNode, CGraphNode>();

            // <targetPartition,sourcePartition>
            // This dictionary monitors the splitted partitions that create new source partitions
            // Source partitions are created as keys in this dictionary when the original source
            // partition's nodes exhibit different behaviour between each other. When subsequent nodes
            // of the initial DFA are investigated their behaviour is compared with the behaviors
            // already identified in this dictionary. If the behaviour of the node is not identified
            // in the dictionary the partition is splitted < Considered as output >
            Dictionary<CGraphNode, CGraphNode> m_NodeConfigurationMappings = new Dictionary<CGraphNode, CGraphNode>();

            // Create a dummy node for nodes that don't have outward edges for a given character
            CGraphNode sourcePartition, sourceConfiguration, targetConfiguration, NULLPartition = m_currentMinimizedDFA.CreateGraphNode<CGraphNode>("NULL");

            // ********************** debug ***********************
            m_reporting.AddSplitRecord(node);


            // For each character range in the initial DFA alphabet character set
            foreach (CCharRange range in m_currentDFA.M_Alphabet) {
                // For each character in the given range
                foreach (Int32 ch in range) {
                    int i = 0;
                    // For each initial DFA node in the current minimized-DFA node check
                    // the transitions for each given character. 
                    foreach (CGraphNode iNode in currentConfiguration) {

                        CGraphNode targetConfigurationNode;

                        // Cache initial DFA transitions for the given character from initial DFA's 
                        // nodes in the current considered configuration to the min DFA configurations  
                        targetConfiguration = GetTargetNodeConfiguration(iNode, ch, out targetConfigurationNode);

                        if (targetConfiguration != null) {
                            // Node (of the initial-DFA) maps to the targetPartition of the
                            // minimized-DFA for the given character ch (Node,ch)->targetPartition
                            m_CharConfigurationMappings[iNode] = targetConfiguration;
                            if (i == 0) {
                                m_NodeConfigurationMappings[targetConfiguration] = node;
                                //                      (first iNode, ch)
                                // targetConfiguration ----------------> sourceConfiguration
                                // m_NodeConfigurationMappings data structure records configurations that
                                // exhibit the following behaviour:
                                // They are targets in transitions from configurations for the given character
                                // This behaviour is exhibited by the first node in the currentConfiguration
                                // So we will look how many other nodes will exhibit the same behaviour in the
                                // current loop. 
                                // m_NodeConfigurationMappings : Holds valid values that refers to the current
                                // configuration for a specific alphabet character
                            }
                            // ********************** debug ***********************
                            m_reporting.AddBehaviour(new Tuple<CGraphNode, int, CGraphNode, CGraphNode>(iNode, ch, targetConfigurationNode, targetConfiguration));
                        }
                        else {
                            m_CharConfigurationMappings[iNode] = NULLPartition;
                            if (i == 0) {
                                m_NodeConfigurationMappings[NULLPartition] = node;
                            }

                            // ********************** debug ***********************
                            m_reporting.AddBehaviour(new Tuple<CGraphNode, int, CGraphNode, CGraphNode>(iNode, ch, targetConfigurationNode, NULLPartition));
                        }
                        i++;
                    }

                    // Verify if all Nodes map to the same configuration for the same character.
                    foreach (CGraphNode n in m_CharConfigurationMappings.Keys) {
                        // If not Split and return
                        // Anchor the first element of current configuration to its partition
                        if (m_CharConfigurationMappings[n] != m_CharConfigurationMappings[currentConfiguration[0]]) {
                            // m_CharConfigurationMappings[n] : configuration where n node (of the currentConfiguration)
                            // maps for the given character
                            // m_CharConfigurationMappings[currentConfiguration[0]] : configuration where the first node
                            // of the currentConfiguration maps for the given character
                            // Check if node n exhibits the same behaviour with the first node (currentConfiguration[0]) of
                            // currentConfiguration. The expected behaviour is to have the same 
                            // target configuration. If not, then they must be separated

                            // ********************** debug ***********************
                            CHopcroftSplitAction splitAct;

                            if (!m_NodeConfigurationMappings.ContainsKey(m_CharConfigurationMappings[n])) {
                                // Check if a configuration has already being identified that has 
                                // the same behaviour with the one identified for node n. That is,
                                // There exists a configuration that contains nodes that map to the 
                                // same configuration as node n for the given character
                                // If not a new configuration has to created and node n has to transferred there

                                // Check if node n destination partition already exists. If not created
                                // and pull node n from the current partition to the newly created one
                                // here

                                // a. Create a new node in the minimized DFA
                                CGraphNode newp = m_currentMinimizedDFA.CreateGraphNode<CGraphNode>();


                                // ********************** debug ***********************
                                

                                newConfiguration = new List<CGraphNode>();
                                SetNodesInConfiguration(newp, newConfiguration);

                                // b. Remove n from the current configuration
                                // b1. Update n
                                SetNodeConfiguration(n, newp);
                                // b2. Update current configuration
                                currentConfiguration.Remove(n);
                                // b3. Update new configuration
                                newConfiguration.Add(n);
                                // b4. Update partitions' configuration
                                m_NodeConfigurationMappings[m_CharConfigurationMappings[n]] = newp;

                                // ********************** debug ***********************
                                splitAct = m_reporting.AddSplit(new Tuple<int, CGraphNode>(ch, newp), GetNodesInConfiguration(node), GetNodesInConfiguration(newp));
                                
                            }
                            else {
                                //.. else of the partition that n node goes to exists just 
                                // make the transfer without creating a new partition
                                sourcePartition = m_NodeConfigurationMappings[m_CharConfigurationMappings[n]];

                                List<CGraphNode> c = GetNodesInConfiguration(sourcePartition);

                                // b. Remove n from the current configuration
                                // b1. Update n
                                SetNodeConfiguration(n, sourcePartition);
                                // b2. Update current configuration
                                currentConfiguration.Remove(n);
                                // b3. Update new configuration
                                c.Add(n);

                                // ********************** debug ***********************
                                splitAct = m_reporting.AddSplit(new Tuple<int, CGraphNode>(ch, sourcePartition), GetNodesInConfiguration(node), GetNodesInConfiguration(sourcePartition));
                            }
                        }
                    }

                    // If yes, do nothing and proceed to the next character

                    // Clear dictionaries for the next iteration
                    m_CharConfigurationMappings.Clear();
                    m_NodeConfigurationMappings.Clear();
                }
            }
            m_currentMinimizedDFA.RemoveNode(NULLPartition);
        }

        // Works only for the initial DFA's nodes. Returns the target minDFA node that contains
        // in its configuration the target of the transition from the given node and character
        public CGraphNode GetTargetNodeConfiguration(CGraphNode node, Int32 character, out CGraphNode targetConfigurationNode) {

            if (m_currentDFA.IsANodeOfGraph(node)) {

                targetConfigurationNode = m_currentDFA.GetTransitionTarget(node, character);
                if (targetConfigurationNode != null) {
                    return GetNodeConfiguration(targetConfigurationNode) as CGraphNode;
                }
                else {
                    return null;
                }
            }
            else {
                targetConfigurationNode = null;
                return null;
            }
        }

        /// <summary>
        /// The method returns which node of the minimized-DFA contains the
        /// given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public CGraphNode GetNodeConfiguration(CGraphNode node) {
            return m_currentNodeConfiguration.GetNodeConfiguration(node);
        }

        public void SetNodeConfiguration(CGraphNode node, CGraphNode conf) {
            m_currentNodeConfiguration.SetNodeConfiguration(node, conf);
        }

        public void SetMinDFAStateTransitionCharacterSet(CGraphEdge edge, CCharRangeSet set) {
            m_currentMinDFANodeConfiguration.CreateInfo(edge, set);
        }

        public CCharRangeSet GetMinDFAStateTransitionCharacterSet(CGraphEdge edge) {
            return m_currentMinDFANodeConfiguration.Info(edge);
        }

        public void SetInputDFAStateTransitionCharacterSet(CGraphEdge edge, CCharRangeSet set) {
            m_currentNodeConfiguration.CreateInfo(edge, set);
        }

        public CCharRangeSet GetInputDFAStateTransitionCharacterSet(CGraphEdge edge) {
            return m_currentNodeConfiguration.Info(edge);
        }

        /// <summary>
        /// The method returns a list of nodes of the initial DFA that exist
        /// in the given minimized-DFA node
        /// </summary>
        /// <param name="node">minimized-DFA node</param>
        /// <returns>list of initial-DFA nodes contained in node</returns>
        public List<CGraphNode> GetNodesInConfiguration(CGraphNode node) {
            return m_currentMinDFANodeConfiguration.NodesInConfiguration(node);
        }

        public void SetNodesInConfiguration(CGraphNode node, List<CGraphNode> conf) {
            m_currentMinDFANodeConfiguration.StoreNodesInConfiguration(node, conf);
        }
    }


}
