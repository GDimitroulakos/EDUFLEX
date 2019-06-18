using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.UOPCore;

namespace Parser.Hopcroft {

    // This class represented the Hopcroft Algorithm. The initial DFA has nodes
    // that store the configuration information under the key m_CONFIGURATIONKEY.
    // The result DFA has nodes representing the algorithm's configuration and
    // store the nodes that represent under the same key m_CONFIGURATIONKEY.
    public class CHopcroftAlgorithm {
        private FA m_DFA;
        private FA m_minimizedDFA;

        private CGraphQueryInfo m_nodeConfiguration;
        private CGraphQueryInfo m_minDFANodeConfiguration;

        // The key by which we can access edge transition info. One key
        // per Hopecoft algorithm instance
        public readonly string m_CONFIGURATIONKEY = "configuration";

        public FA MinimizedDfa => m_minimizedDFA;

        public CHopcroftAlgorithm(FA dfa) {
            m_DFA = dfa;
            m_minimizedDFA = new FA();
            m_nodeConfiguration = new CGraphQueryInfo(m_DFA,m_CONFIGURATIONKEY);
            m_minDFANodeConfiguration = new CGraphQueryInfo(m_minimizedDFA,m_CONFIGURATIONKEY);
        }

        public void Init() {

            List<CGraphNode> configurationAcc,configurationNonAcc;

            // Create two distinct configurations for accepted and non-accepted nodes
            CGraphNode acceptedConf= m_minimizedDFA.CreateGraphNode<CGraphNode>();
            configurationAcc = new List<CGraphNode>();
            SetMinDFANodeConfiguration(acceptedConf,configurationAcc);
            CGraphNode non_acceptedConf = m_minimizedDFA.CreateGraphNode<CGraphNode>();
            configurationNonAcc = new List<CGraphNode>();
            SetMinDFANodeConfiguration(non_acceptedConf, configurationNonAcc);

            // Separate accepted from non-accepted nodes into two distinct
            // configurations
            CIt_GraphNodes it = new CIt_GraphNodes(m_DFA);
            for (it.Begin(); !it.End(); it.Next()) {
                if (m_DFA.GetFinalStates().Contains(it.M_CurrentItem)) {
                    SetNodeConfiguration(it.M_CurrentItem, acceptedConf);
                    configurationAcc.Add(it.M_CurrentItem);
                }
                else {
                    SetNodeConfiguration(it.M_CurrentItem, non_acceptedConf);
                    configurationNonAcc.Add(it.M_CurrentItem);
                }
            }

            int nodeCount = 0;
            CIt_GraphNodes minDFA_it = new CIt_GraphNodes(m_minimizedDFA);

            while (nodeCount != m_minimizedDFA.M_NumberOfNodes) {

                nodeCount = m_minimizedDFA.M_NumberOfNodes;

                for (minDFA_it.Begin(); !minDFA_it.End(); minDFA_it.Next()) {
                    Split(minDFA_it.M_CurrentItem);
                }
            }

            // Draw the final edges
            CIt_GraphNodes minit1 = new CIt_GraphNodes(m_minimizedDFA);
            CIt_GraphNodes minit2 = new CIt_GraphNodes(m_minimizedDFA);
            List<CGraphNode> confs,conft;
            CGraphEdge edge,newedge;
            for (minit1.Begin(); !minit1.End(); minit1.Next()) {
                for (minit2.Begin(); !minit2.End(); minit2.Next()) {
                    confs = GetMinDFANodeConfiguration(minit1.M_CurrentItem);
                    conft = GetMinDFANodeConfiguration(minit2.M_CurrentItem);
                    foreach (CGraphNode snode in confs) {
                        foreach (CGraphNode tnode in conft) {
                            edge = m_DFA.Edge(snode, tnode);
                            if (edge != null) {
                                // Disallow dublicate edges between nodes of the minimized DFA
                                newedge = m_minimizedDFA.Edge(minit1.M_CurrentItem, minit2.M_CurrentItem);
                                if (newedge == null) {
                                    newedge = m_minimizedDFA.AddGraphEdge<CGraphEdge, CGraphNode>(minit1.M_CurrentItem, minit2.M_CurrentItem, GraphType.GT_DIRECTED);
                                }

                                // Add transition characters
                                if (newedge[FA.m_TRANSITIONSKEY] == null) {
                                    newedge[FA.m_TRANSITIONSKEY] = new CCharRangeSet(false);
                                }
                                CCharRangeSet charset = newedge[FA.m_TRANSITIONSKEY] as CCharRangeSet;
                                charset.AddSet(edge[FA.m_TRANSITIONSKEY] as CCharRangeSet);

                            }
                        }
                    }
                }
            }

            // Detect accepted nodes in minimized DFA
            CIt_GraphNodes it1 = new CIt_GraphNodes(m_minimizedDFA);
            for (it1.Begin(); !it1.End(); it1.Next()) {
                List<CGraphNode> configuration = GetMinDFANodeConfiguration(it1.M_CurrentItem);
                List<CGraphNode> finals = m_DFA.GetFinalStates();
                foreach (CGraphNode node in configuration) {
                    if (finals.Contains(node)) {
                        m_minimizedDFA.SetFinalState(it1.M_CurrentItem);
                    }
                }
            }

            // Detect initial state of minimized DFA
            for (it1.Begin(); !it1.End(); it1.Next()) {
                List<CGraphNode> configuration = GetMinDFANodeConfiguration(it1.M_CurrentItem);
                CGraphNode initial = m_DFA.M_Initial;
                foreach (CGraphNode node in configuration) {
                    if (initial == node) {
                        m_minimizedDFA.M_Initial = it1.M_CurrentItem;
                    }
                }
            }

            FASerializer serializer =new FASerializer(m_minimizedDFA);
            serializer.Print();

            m_DFA.RegisterGraphPrinter(new ThompsonGraphVizPrinter(m_minimizedDFA));
            m_DFA.Generate(@"../Debug/minimizedDFA.dot", true);
        }
        /// <summary>
        /// Splits the specified node.
        /// </summary>
        /// <param name="node">Represents a minimized DFA's node (current configuration)</param>
        public void Split(CGraphNode node) {
            // Holds the nodes of the initial DFA that exist in the current partition
            List<CGraphNode> currentConfiguration = GetMinDFANodeConfiguration(node);

            List<CGraphNode> newConfiguration;

            // Holds the mapping of initial DFA for a given character
            // <source(DFA),target(minimizedDFA)>
            // <Considered as input >
            Dictionary<CGraphNode, CGraphNode> m_CharConfigurationMappings = new Dictionary<CGraphNode, CGraphNode>();

            // <targetPartition,sourcePartition>
            // This dictionary monitors the splitted partitions that create new source partitions
            // Source partitions are created as keys in this dictionary when the original source
            // partition's node exhibit different behaviour between eachother. When subsequent nodes
            // of the initial DFA are investigated their behaviour is compared with the behaviours
            // already identified in this dictionary. If the behaviour of the node is not identified
            // in the dictionary the partition is splitted < Considered as output >
            Dictionary<CGraphNode, CGraphNode> m_NodeConfigurationMappings = new Dictionary<CGraphNode, CGraphNode>();


            Dictionary<CGraphNode,CGraphNode> m_  = new Dictionary<CGraphNode, CGraphNode>();

            CGraphNode sourcePartition,targetPartition, NULLPartition = m_minimizedDFA.CreateGraphNode<CGraphNode>();

            foreach (CCharRange range in m_DFA.M_Alphabet) {
                foreach (Int32 ch in range) {
                    int i = 0;
                    foreach (CGraphNode Node in currentConfiguration) {
                        // Cache initial DFA transitions for the given character from initial DFA's 
                        // nodes in the current considered partition to the min DFA partitions  
                        targetPartition = GetTargetNodeConfiguration(Node, ch);

                        if (targetPartition != null) {
                            m_CharConfigurationMappings[Node] = targetPartition;
                            if (i == 0) {
                                m_NodeConfigurationMappings[targetPartition] = GetNodeConfiguration(Node);
                            }
                        }
                        else {
                            m_CharConfigurationMappings[Node] = NULLPartition;
                            if (i == 0) {
                                m_NodeConfigurationMappings[NULLPartition] = GetNodeConfiguration(Node);
                            }
                        }
                        i++;

                    }

                    // Anchor the first element of current configuration to its partition

                    

                    // Verify if all Nodes map to the same configuration for the same character.
                    foreach (CGraphNode n in m_CharConfigurationMappings.Keys) {
                        // If not Split and return
                        if (m_CharConfigurationMappings[n] != m_CharConfigurationMappings[currentConfiguration[0]]) {
                            if (!m_NodeConfigurationMappings.ContainsKey(m_CharConfigurationMappings[n])){
                                // a. Create a new node in the minimized DFA
                                CGraphNode newp = m_minimizedDFA.CreateGraphNode<CGraphNode>();
                                newConfiguration = new List<CGraphNode>();
                                SetMinDFANodeConfiguration(newp, newConfiguration);

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
                                sourcePartition = m_NodeConfigurationMappings[m_CharConfigurationMappings[n]];

                                List<CGraphNode> c = GetMinDFANodeConfiguration(sourcePartition);

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
            m_minimizedDFA.RemoveNode(NULLPartition);
        }

        // Works only for the initial DFA's nodes
        public CGraphNode GetTargetNodeConfiguration(CGraphNode node, Int32 character) {

            if (m_DFA.IsANodeOfGraph(node)) {

                CGraphNode target = m_DFA.GetTransitionTarget(node, character);
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

        public CGraphNode GetNodeConfiguration(CGraphNode node) {
            return m_nodeConfiguration.Info(node) as CGraphNode;
        }

        public void SetNodeConfiguration(CGraphNode node, object conf) {
            m_nodeConfiguration.CreateInfo(node,conf);
        }

        public List<CGraphNode> GetMinDFANodeConfiguration(CGraphNode node) {
            return m_minDFANodeConfiguration.Info(node) as List<CGraphNode>;
        }

        public void SetMinDFANodeConfiguration(CGraphNode node, object conf) {
            m_minDFANodeConfiguration.CreateInfo(node, conf);
        }
    }


}
