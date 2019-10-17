using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;

namespace Parser.SubsetConstruction {

    [Serializable]
    public class CSubsetConstructionNodeInfo {
        private bool m_isClosureNode;

        private string m_closureExpression;

        public bool M_IsClosureNode {
            get => m_isClosureNode;
            set => m_isClosureNode = value;
        }
        public string M_ClosureExpression {
            get => m_closureExpression;
            set => m_closureExpression = value;
        }
    }

    [Serializable]
    public class CSubsetConstructionEdgeInfo {

    }

    [Serializable]
    public class CSubsetConstructionGraphInfo {

    }

    [Serializable]
    public class CSubsetConstructionInfo :CGraphQueryInfo<CSubsetConstructionNodeInfo, CSubsetConstructionEdgeInfo, CSubsetConstructionGraphInfo> {
        public CSubsetConstructionInfo(CGraph graph, object key) : base(graph, key) {
        }

        public bool IsClosureNode(CGraphNode node) {
            return Info(node).M_IsClosureNode;
        }

        public void SetClosureNode(CGraphNode node, bool isclosure=true) {
            Info(node).M_IsClosureNode = isclosure;
        }

        public string GetNodeClosureExpression(CGraphNode node) {
            return Info(node).M_ClosureExpression;
        }

        public void SetNodeClosureExpression(CGraphNode node, string expression) {
            Info(node).M_ClosureExpression = expression;
        }


    }
}
