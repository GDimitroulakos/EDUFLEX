using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;

namespace Parser.SubsetConstruction {

    [Serializable]
    public class CSubsetConstructionNodeInfo {
        private bool m_isClosureNode =false;

        private string m_closureExpression=null;

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
        // The following dictionary records which DFA nodes belong to each closure.
        // The key is an integer referring to the closure and the value is the 
        // list of DFA nodes participating in the closure
        private Dictionary<int, List<CGraphNode>> m_closureNodesMapping = new Dictionary<int, List<CGraphNode>>();

        public Dictionary<int, List<CGraphNode>> M_ClosureNodesMapping => m_closureNodesMapping;

        public void AddClosureNode(int serial, CGraphNode node) {
            if (!m_closureNodesMapping.ContainsKey(serial)) {
                m_closureNodesMapping[serial] = new List<CGraphNode>();
            }
            m_closureNodesMapping[serial].Add(node);
        }
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

        public Dictionary<int, List<CGraphNode>> GetClosureNodesMapping() {
            return Info().M_ClosureNodesMapping;
        }

        public void AddClosureNode(int serial, CGraphNode node) {
            Info().AddClosureNode(serial,node);
        }

        public void SetNodeClosureExpression(CGraphNode node, string expression) {
            Info(node).M_ClosureExpression = expression;
        }

        public void InitNodeInfo(CGraphNode node, CSubsetConstructionNodeInfo info) {
            CreateInfo(node,info);
        }

        public void InitEdgeInfo(CGraphEdge edge, CSubsetConstructionEdgeInfo info) {
            CreateInfo(edge, info);
        }

        public void InitGraphInfo(CSubsetConstructionGraphInfo info) {
            CreateInfo(info);
        }
    }
}
