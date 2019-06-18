using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;

namespace Parser.UOPCore
{

    public abstract class SerializedUnit {
       
    }
    public class DFASerializedFile {

    }

    public class HeaderUnit {

    }

    public class DFAUnit {

    }
    public class EdgeUnit {

    }
    public class FASerializer : CGraphPrinter {
        private FA m_DFA;

        internal FASerializer(FA DFA, AbstractGraphLabeling<CGraphNode> nodeLabeller = null, AbstractGraphLabeling<CGraphEdge> edgeLabeller = null) : base(DFA, nodeLabeller, edgeLabeller) {
            m_DFA = DFA;
        }

        public override StringBuilder Print() {
            StringBuilder bucket =new StringBuilder(1000);
            CIt_GraphEdges ite = new CIt_GraphEdges(m_DFA);
            bucket.Append(m_DFA.M_Initial.M_Label);
            for (ite.Begin(); !ite.End(); ite.Next()) {
                bucket.Append("\\"+ite.M_CurrentItem.M_Source.M_Label +"-"+ite.M_CurrentItem.M_Target.M_Label);
                bucket.Append(m_DFA.GetEdgeInfo(ite.M_CurrentItem).ToString());
            }

            foreach (var fstate in m_DFA.MFinal) {
                bucket.Append("("+fstate.M_Label+")");
            }
            return bucket;
        }

        public override void Generate(string filepath, bool executeGenerator = true) {
            throw new NotImplementedException();
        }
    }
}
