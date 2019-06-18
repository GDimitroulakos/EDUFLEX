using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;

namespace Parser.Hopcroft
{
    class DFASerializer : CGraphPrinter{
        internal DFASerializer(CGraph graph, AbstractGraphLabeling<CGraphNode> nodeLabeller = null, AbstractGraphLabeling<CGraphEdge> edgeLabeller = null) : base(graph, nodeLabeller, edgeLabeller) {
        }

        public override StringBuilder Print() {





            return null;
        }

        public override void Generate(string filepath, bool executeGenerator = true) {
            throw new NotImplementedException();
        }
    }

    class DFADeserializer {

    }
}
