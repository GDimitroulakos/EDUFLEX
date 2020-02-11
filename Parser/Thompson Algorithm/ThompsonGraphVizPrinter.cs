using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.UOPCore;

namespace Parser.Thompson_Algorithm {
    class ThompsonGraphVizPrinter : CGraphPrinter {
        private UOPCore.Options<ThompsonOptions> m_options;
        private object m_thompsonKeyInfo;
        private ThompsonInfo m_thompsonInfo;
        private FAGraphQueryInfo m_FAInfo;

        internal ThompsonGraphVizPrinter(CGraph graph, UOPCore.Options<ThompsonOptions> options,
            object thompsonkeyinfo,
            CGraphLabeling<CGraphNode> nodeLabeling = null,
            CGraphLabeling<CGraphEdge> edgeLabeling = null
            ) : base(graph, nodeLabeling, edgeLabeling) {
            m_options = options;
            m_thompsonKeyInfo = thompsonkeyinfo;
            m_thompsonInfo = new ThompsonInfo(graph, m_thompsonKeyInfo);
            m_FAInfo = new FAGraphQueryInfo(graph, FA.m_FAINFOKEY);
        }

        public override StringBuilder Print() {
            StringBuilder graphvizStringBuilder = new StringBuilder(1000);
            string header = "digraph G" + m_graph.M_SerialNumber + "{\r\n";
            string graphedge_operator = "->";
            CIt_GraphNodes itn = new CIt_GraphNodes(m_graph);
            CIt_GraphEdges itg = new CIt_GraphEdges(m_graph);
            FA fa = m_graph as FA;

            // Print header if necessary
            // Print the header if the graph is printed alone independently and not
            // in the context for example of a multigraph printing
            if (!m_options.IsSet(ThompsonOptions.TO_COMBINEGRAPHS)) {
                graphvizStringBuilder.Append(header);
            }

            for (itn.Begin(); !itn.End(); itn.Next()) {
                if (fa.GetFinalStates().Count != 0 && fa.GetFinalStates().Contains(itn.M_CurrentItem)) {
                    graphvizStringBuilder.Append("\"" + itn.M_CurrentItem.M_Label + "\" [peripheries=2];\n");
                }
                else if (itn.M_CurrentItem == fa.M_Initial) {
                    graphvizStringBuilder.Append("\"" + itn.M_CurrentItem.M_Label + "\" [style=filled,fillcolor=green];\n");
                }
                else if (m_thompsonInfo.IsNodeClosureEntrance(itn.M_CurrentItem)) {
                    graphvizStringBuilder.Append("\"" + itn.M_CurrentItem.M_Label + "\" [style=filled,fillcolor=red];\n");
                }
                else if (m_thompsonInfo.IsNodeClosureExit(itn.M_CurrentItem)) {
                    graphvizStringBuilder.Append("\"" + itn.M_CurrentItem.M_Label + "\" [style=filled,fillcolor=purple];\n");
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
                    if (m_thompsonInfo.IsNodeClosureExit(g.M_Source) &&
                        m_thompsonInfo.IsNodeClosureEntrance(g.M_Target)) {
                        graphvizStringBuilder.AppendFormat(" [color=red,style = bold, label = \"" + m_FAInfo.Info(g).M_TransitionCharSet?.ToString() + "\"]");
                    }
                }

                graphvizStringBuilder.Append(";\r\n");
            }

            // Print footer if necessary
            // Print the header if the graph is printed alone independently and not
            // in the context for example of a multigraph printing
            if (!m_options.IsSet(ThompsonOptions.TO_COMBINEGRAPHS)) {
                graphvizStringBuilder.Append("}\r\n");
            }

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
