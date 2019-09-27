using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using System.IO;

namespace Parser.UOPCore {
    public class FATextPrinter : CGraphPrinter{
        private FAGraphQueryInfo m_FAInfo;

        internal FATextPrinter(CGraph graph, AbstractGraphLabeling<CGraphNode> nodeLabeller = null, AbstractGraphLabeling<CGraphEdge> edgeLabeller = null) : base(graph, nodeLabeller, edgeLabeller) {
            m_FAInfo = new FAGraphQueryInfo(graph,FA.m_FAINFOKEY);
        }

        public override StringBuilder Print() {
           StringBuilder FAText = new StringBuilder(1000);

           // Output Nodes
           CIt_GraphNodes itn = new CIt_GraphNodes(m_graph);
           for (itn.Begin(); !itn.End(); itn.Next()) {
               FAText.Append(itn.M_CurrentItem.M_Label);
               if (!itn.M_LastItem) {
                   FAText.Append(", ");
               }
               else {
                   FAText.Append("\n\n");
               }
           }
           
           // Output edges
           CIt_GraphEdges ite = new CIt_GraphEdges(m_graph);
           for (ite.Begin(); !ite.End(); ite.Next()) {
               FAText.Append("\n"+ite.M_CurrentItem.M_Source.M_Label +"---"+ m_FAInfo.Info(ite.M_CurrentItem).M_TransitionCharSet.ToString() +"---->"+
                   ite.M_CurrentItem.M_Target.M_Label);
           }
           
           return FAText;
        }

        public override void Generate(string filepath, bool executeGenerator = true) {
            // Open a streamwriter
            using (StreamWriter fstream = new StreamWriter(filepath)) {
                fstream.WriteLine(Print());
                fstream.Close();
            }
        }
    }
}
