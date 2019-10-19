using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;

namespace Parser.Thompson_Algorithm {
    [Serializable]
    public class ThompsonNodeFAInfo {
        // True if the node is a closure entrance node
        private bool m_closureEntrance = false;
        // True if the node is a closure exit node
        private bool m_closureExit = false;
        // Unique integer identifying the closure
        private int? m_closureSerialNumber = null;
        // Refers to the closure subexpression text in a regular expression
        private string m_closureExpression = null;

        public ThompsonNodeFAInfo() {
        }
        public bool M_ClosureEntrance {
            get => m_closureEntrance;
            set => m_closureEntrance = value;
        }
        public bool M_ClosureExit {
            get => m_closureExit;
            set => m_closureExit = value;
        }
        public string M_ClosureExpression {
            get => m_closureExpression;
            set => m_closureExpression = value;
        }
        public int? M_ClosureSerialNumber {
            get => m_closureSerialNumber;
            set => m_closureSerialNumber = value;
        }
    }

    [Serializable]
    public class ThompsonEdgeFAInfo {
        public ThompsonEdgeFAInfo() { }
    }

    [Serializable]
    public class ThompsonFAInfo {
        private static int m_closureCounter = 0;

        // Refers to the closure subexpression text in a regular expression
        private string m_closureExpression = null;

        public static int M_ClosureCounter => m_closureCounter;

        public string M_ClosureExpression {
            get => m_closureExpression;
            set => m_closureExpression = value;
        }

        public static int GetNewClosureSerial() {
            return m_closureCounter++;
        }

        public ThompsonFAInfo() { }
    }

    [Serializable]
    public class ThompsonInfo : CGraphQueryInfo<ThompsonNodeFAInfo, ThompsonEdgeFAInfo, ThompsonFAInfo> {
        public ThompsonInfo(CGraph graph, object key) : base(graph, key) {

        }

        public string GetNodeClosureExpression() {
            return Info().M_ClosureExpression;
        }
        public void SetNodeClosureExpression(string expression) {
            Info().M_ClosureExpression = expression;
        }
        public string GetNodeClosureExpression(CGraphNode node) {
            return Info(node).M_ClosureExpression;
        }

        public int GetClosureSerial(CGraphNode node) {
            return (int)Info(node).M_ClosureSerialNumber;
        }
        public void SetNodeClosureExpression(CGraphNode node,string expression) {
            Info(node).M_ClosureExpression = expression;
        }

        public bool IsNodeClosureEntrance(CGraphNode node) {
            return Info(node).M_ClosureEntrance;
        }
        public bool IsNodeClosureExit(CGraphNode node) {
            return Info(node).M_ClosureExit;
        }
        public void SetNodeClosureEntrance(CGraphNode node, int closureSerialNumber) {
            if (Info(node) == null) {
                InitNodeInfo(node, new ThompsonNodeFAInfo());
            }
            Info(node).M_ClosureEntrance = true;
            Info(node).M_ClosureSerialNumber = closureSerialNumber;
        }
        public void SetNodeClosureExit(CGraphNode node, int closureSerialNumber) {
            if (Info(node) == null) {
                InitNodeInfo(node, new ThompsonNodeFAInfo());
            }
            Info(node).M_ClosureExit = true;
            Info(node).M_ClosureSerialNumber = closureSerialNumber;
        }
        public void InitNodeInfo(CGraphNode node, ThompsonNodeFAInfo info) {
            CreateInfo(node, info);
        }

        public void InitFAInfo(ThompsonFAInfo info) {
            CreateInfo(info);
        }

    }
}
