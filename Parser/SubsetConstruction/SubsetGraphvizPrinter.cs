using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using Parser.UOPCore;

namespace Parser.SubsetConstruction {
    class SubsetGraphvizPrinter :CGraphVizPrinter {
        /// <summary>
        /// Key that provides access to the information stored in the graph by the
        /// subset construction algorithm
        /// </summary>
        private object m_subsetInfoKey;
        private FAGraphQueryInfo m_FAInfo;
        public SubsetGraphvizPrinter(CGraph graph, object subsetinfoKey,CGraphLabeling<CGraphNode> nodeLabeling = null, CGraphLabeling<CGraphEdge> edgeLabeling = null) : base(graph, nodeLabeling, edgeLabeling) {
            m_FAInfo = new FAGraphQueryInfo(graph, FA.m_FAINFOKEY);
            m_subsetInfoKey = subsetinfoKey;
        }

        public override StringBuilder Print() {
            // Allocate a stringbuiler object and allocate space for 1000 characters
            StringBuilder graphvizStringBuilder = new StringBuilder(1000);
            CIt_GraphNodes itn = new CIt_GraphNodes(m_graph);
            CIt_GraphEdges itg = new CIt_GraphEdges(m_graph);
            CSubsetConstructionInfo subsetInfo = new CSubsetConstructionInfo(m_graph, m_subsetInfoKey);
            Dictionary<int, List<CGraphNode>> closuresMap;
            FA fa = m_graph as FA;
            string graphedge_operator = "->";

            closuresMap = subsetInfo.GetClosureNodesMapping();

            //1. generate header
            string header = "digraph G" + m_graph.M_SerialNumber + "{\r\n";
            graphvizStringBuilder.Append(header);

            foreach (KeyValuePair<int, List<CGraphNode>> closure in closuresMap) {
                string subgraphHeader = "\tsubgraph cluster" + closure.Key + " {\r\n";
                string subgraphBody = "\t\tnode [style=filled];\r\n" +
                                      "\t\tstyle=filled;\r\n" +
                                      "\t\tcolor=lightgrey;\r\n" +
                                      "\t\tlabel =\"" + subsetInfo.Info(closure.Value[0]).M_ClosureExpression +
                                      "\";\r\n\t\t";
                graphvizStringBuilder.Append(subgraphHeader + subgraphBody);
                foreach (CGraphNode node in closure.Value) {
                    graphvizStringBuilder.Append(node.M_Label + ";");
                }

                graphvizStringBuilder.AppendLine();
                graphvizStringBuilder.Append("\t}");
            }

            // Print all  nodes
            for (itn.Begin(); !itn.End(); itn.Next()) {
                if (fa.GetFinalStates().Count != 0 && fa.GetFinalStates().Contains(itn.M_CurrentItem)) {
                    graphvizStringBuilder.Append("\"" + itn.M_CurrentItem.M_Label + "\" [peripheries=2];\n");
                }
                else if (itn.M_CurrentItem == fa.M_Initial) {
                    graphvizStringBuilder.Append("\"" + itn.M_CurrentItem.M_Label +
                                                 "\" [style=filled,fillcolor=green];\n");

                }
                else {
                    graphvizStringBuilder.Append("\"" + itn.M_CurrentItem.M_Label + "\";\n");
                }
            }

            // Print all edges of the graph
            for (itg.Begin(); !itg.End(); itg.Next()) {
                CGraphEdge g = itg.M_CurrentItem;

                string source, target;
                source = g.M_Source.M_Label;
                target = g.M_Target.M_Label;

                graphvizStringBuilder.AppendFormat("\"{0}\"" + graphedge_operator + "\"{1}\"",
                    source, target);
                string s = m_FAInfo.Info(g).M_TransitionCharSet?.ToString();
                if (s != null) {
                    graphvizStringBuilder.AppendFormat(" [style = bold, label = \"" + m_FAInfo.Info(g).M_TransitionCharSet?.ToString() + "\"]");
                }
                else {

                }
                graphvizStringBuilder.Append(";\r\n");
            }

            graphvizStringBuilder.Append("}\r\n");
            return graphvizStringBuilder;
        }

        public override void Generate(string filepath, bool executeGenerator = true) {
            // Open a streamwriter
            using (StreamWriter fstream = new StreamWriter(filepath)) {
                fstream.WriteLine(Print());
                fstream.Close();
            }
            // Prepare the process dot to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = "-Tgif " +
                              Path.GetFileName(filepath) + " -o " +
                              Path.GetFileNameWithoutExtension(filepath) + ".gif";
            // Enter the executable to run, including the complete path
            start.FileName = "dot";
            // Do you want to show a console window?
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            int exitCode;

            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start)) {
                proc.WaitForExit();

                // Retrieve the app's exit code
                exitCode = proc.ExitCode;
            }
        }
    }
}
