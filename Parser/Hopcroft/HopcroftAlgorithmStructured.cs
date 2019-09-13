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

namespace Parser.Hopcroft {

    public class CHopcroftReporting {
        List<CGraphNode> m_initialAcceptedStates;
        List<CGraphNode> m_initialNonAcceptedStates;

        public void SetInitialMinDFAStatesContents(List<CGraphNode> accStates, List<CGraphNode> nonaccStates) {
            m_initialAcceptedStates = new List<CGraphNode>(accStates);
            m_initialNonAcceptedStates = new List<CGraphNode>(nonaccStates);
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

        public Dictionary<uint,RERecord> MinimizedDfas => m_inputDFAs;

        CHopcroftReporting m_reporting;

        public CHopcroftAlgorithmStructured(Dictionary<uint,RERecord> dfas) {
            m_inputDFAs = dfas;
            m_reporting = new CHopcroftReporting();
        }

        public void Start() {
            // Apply Hopcroft algorithm separately for each Regular Expression
            foreach (KeyValuePair<uint,RERecord> rec in m_inputDFAs) {
                m_currentDFA = rec.Value.M_Dfa;
                m_currentRE = rec.Value.M_Line;
                rec.Value.M_MinDfa= m_currentMinimizedDFA = new FA();
                m_currentNodeConfiguration = new HopcroftInputGraphQueryInfo(m_currentDFA, m_HOPCROFTCONFIGURATIONKEY);
                m_currentMinDFANodeConfiguration = new HopcroftOutputGraphQueryInfo(m_currentMinimizedDFA, m_HOPCROFTCONFIGURATIONKEY);
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
            m_reporting.SetInitialMinDFAStatesContents(configurationAcc, configurationNonAcc);


            int nodeCount = 0;
            CIt_GraphNodes minDFA_it = new CIt_GraphNodes(m_currentMinimizedDFA);
            // Iterate while the algorithm reaches a fixed point state
            while (nodeCount != m_currentMinimizedDFA.M_NumberOfNodes) {

                // keep the number of nodes before applying a new split
                nodeCount = m_currentMinimizedDFA.M_NumberOfNodes;

                for (minDFA_it.Begin(); !minDFA_it.End(); minDFA_it.Next()) {
                    Split(minDFA_it.M_CurrentItem);
                }
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



            FASerializer serializer = new FASerializer(m_currentMinimizedDFA);
            serializer.Print();

            m_currentDFA.RegisterGraphPrinter(new ThompsonGraphVizPrinter(m_currentMinimizedDFA, new UOPCore.Options<ThompsonOptions>()));
            m_currentDFA.Generate(@"../bin/Debug/minimizedDFA"+m_currentRE+".dot", true);
        }
        /// <summary>
        /// Splits the specified minimized-DFA node.
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
            // this dictionary is updated
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

            CGraphNode sourcePartition, sourceConfiguration, targetConfigurationNode, NULLPartition = m_currentMinimizedDFA.CreateGraphNode<CGraphNode>();

            // For each character range in the initial DFA alphabet character set
            foreach (CCharRange range in m_currentDFA.M_Alphabet) {
                // For each character in the given range
                foreach (Int32 ch in range) {
                    int i = 0;
                    // For each initial DFA node in the current minimized-DFA node check
                    // the transitions for each given character. 
                    foreach (CGraphNode iNode in currentConfiguration) {
                        // Cache initial DFA transitions for the given character from initial DFA's 
                        // nodes in the current considered configuration to the min DFA configurations  
                        targetConfigurationNode = GetTargetNodeConfiguration(iNode, ch);

                        if (targetConfigurationNode != null) {
                            // Node (of the initial-DFA) maps to the targetPartition of the
                            // minimized-DFA for the given character ch (Node,ch)->targetPartition
                            m_CharConfigurationMappings[iNode] = targetConfigurationNode;
                            if (i == 0) {
                                sourceConfiguration = GetNodeConfiguration(iNode);
                                m_NodeConfigurationMappings[targetConfigurationNode] = sourceConfiguration;
                                //                      (first iNode, ch)
                                // targetConfiguration ----------------> sourceConfiguration
                                // m_NodeConfigurationMappings data structure records configurations that
                                // exhibit the following behaviour:
                                // They are targets in transitions from configurations for the given character
                                // This behaviour is exhibited by the first node in the currentConfiguration
                                // So we will look how many other nodes will exhibit the same behaviour in the
                                // following loop. 
                            }
                        }
                        else {
                            m_CharConfigurationMappings[iNode] = NULLPartition;
                            if (i == 0) {
                                sourceConfiguration = GetNodeConfiguration(iNode);
                                m_NodeConfigurationMappings[NULLPartition] = sourceConfiguration;
                            }
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
                            }
                        }
                    }

                    // If yes, do nothing and proceed to the next character

                    // Clear dictionary for the next iteration
                    m_CharConfigurationMappings.Clear();
                }
            }
            m_currentMinimizedDFA.RemoveNode(NULLPartition);
        }

        // Works only for the initial DFA's nodes. Returns the target minDFA node that contains
        // in its configuration the target of the transition from the given node and character
        public CGraphNode GetTargetNodeConfiguration(CGraphNode node, Int32 character) {

            if (m_currentDFA.IsANodeOfGraph(node)) {

                CGraphNode target = m_currentDFA.GetTransitionTarget(node, character);
                if (target != null) {
                    return GetNodeConfiguration(target) as CGraphNode;
                }
                else {
                    return null;
                }
            }
            else {
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
