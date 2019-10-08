using System;
using System.Collections.Generic;
using System.IO;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.ASTVisitor;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.Hopcroft;
using Parser.SubsetConstruction;
using Parser.UOPCore;

namespace Parser.Thompson_Algorithm
{

    [Flags]
    public enum ThompsonOptions {
        TO_DEFAULT=0,
        TO_STEPS =1, // The outcome of each Thompson algorithm execution step is reported
                     // either in a single file or multiple
        TO_PROPAGATELABELS =2,
        TO_COMBINEGRAPHS =4, // The outcome of the Thomson algorithm steps are combined in 
                            // a single file
        // This options is in accordance to the PO_OPERATION_SIMPLECHECK_VS_CODE option
        // in the Parser Facade class. Flatten corresponds to the simple lexical analyzer
        // generation where the a the whole NFA produced by the whole set of regular expressions
        // is fed to the Subset Construction and Hopcroft algorithms. Whereas STRUCTURED
        // refers to the separated NFA to min-DFA generation for each separate regular expression and
        // then their combination through the alternation operator of the Thompson algorithm.
        // When this option is set it refers to the structured conversion
        TO_NFAGENERATION_FLATTEN_VS_STRUCTURED=8
    }

    public class ThompsonReporting {
        /// <summary>
        /// This fields stores Thompson Algorithm execution and reporting options
        /// </summary>
        private UOPCore.Options<ThompsonOptions> m_options;

        /// <summary>
        /// This field holds the printers of the outcome of the Thompson Algorithm steps
        /// </summary>
        private ThomsonMultiGraphGraphVizPrinter m_ThomsonStepsPrinter;

        public ThompsonReporting(UOPCore.Options<ThompsonOptions> options) {
            m_options = options;

            // Initialize m_ThomsonStepsPrinter field
            m_ThomsonStepsPrinter = new ThomsonMultiGraphGraphVizPrinter("ThompsonSteps.dot");
            m_ThomsonStepsPrinter.e_prelude += new Func<object, string>(p => "digraph Total { ");
            m_ThomsonStepsPrinter.e_epilogue += new Func<object, string>(p => "} ");
            m_ThomsonStepsPrinter.e_intermediate_after += new Func<object, string>(p => {
                FAGraphVizPrinter pp = p as FAGraphVizPrinter;
                return "//" + pp.M_Graph.M_Label;
            });
        }

        /// <summary>
        /// Extracts the outcome of one of the steps of the Thompson algorithm to a file
        /// </summary>
        public void ExctractThompsonStep(FA NFA,string filename) {
            FAGraphVizPrinter gp = new FAGraphVizPrinter(NFA, new UOPCore.Options<ThompsonOptions>());
            NFA.RegisterGraphPrinter(gp);
            NFA.Generate(filename, true);
        }

        /// <summary>
        /// Extracts all the steps of the Thompson algorithm to a file
        /// </summary>
        public void ExtractThompsonSteps() {
            m_ThomsonStepsPrinter.Generate();
        }

        public void AddThompsonStepToReporting(FA NFA, bool generateReport=false) {
            FAGraphVizPrinter gp1 = new FAGraphVizPrinter(NFA, new UOPCore.Options<ThompsonOptions>(ThompsonOptions.TO_COMBINEGRAPHS));
            m_ThomsonStepsPrinter.Add(gp1);
            if (generateReport) {
                m_ThomsonStepsPrinter.Generate();
            }
        }

        public void ThompsonStepsGenerate() {
            m_ThomsonStepsPrinter.Generate();
        }
    }

    [Serializable]
    public class ThompsonNodeFAInfo {
        // True if the node has non-e outward edges
        private bool m_characterOutwardEdges=false;
        // True if the node is a closure entrance node
        private bool m_closureEntrance=false;

        public ThompsonNodeFAInfo() {
        }

        public bool M_CharacterOutwardEdges {
            get => m_characterOutwardEdges;
            set => m_characterOutwardEdges = value;
        }

        public bool M_ClosureEntrance {
            get => m_closureEntrance;
            set => m_closureEntrance = value;
        }
    }

    [Serializable]
    public class ThompsonEdgeFAInfo {

    }

    [Serializable]
    public class ThompsonFAInfo {

    }

    [Serializable]
    public class ThompsonInfo : CGraphQueryInfo<ThompsonNodeFAInfo,ThompsonEdgeFAInfo,ThompsonFAInfo> {
        public ThompsonInfo(CGraph graph, object key) : base(graph, key) {
           
        }

        public bool IsNodeWithNon_e_Edges(CGraphNode node) {
            return Info(node).M_CharacterOutwardEdges;
        }
        public bool IsNodeClosureEntrance(CGraphNode node) {
            return Info(node).M_ClosureEntrance;
        }
        public void SetNodeWithNon_e_Edges(CGraphNode node, bool hasNon_e_edges) {
            if (Info(node) == null) {
                InitNodeInfo(node,new ThompsonNodeFAInfo());
            }
            Info(node).M_CharacterOutwardEdges = hasNon_e_edges;
        }
        public void SetNodeClosureEntrance(CGraphNode node, bool closureEntrance) {
            if (Info(node) == null) {
                InitNodeInfo(node, new ThompsonNodeFAInfo());
            }
            Info(node).M_ClosureEntrance = closureEntrance;
        }
        public void InitNodeInfo(CGraphNode node, ThompsonNodeFAInfo info) {
            CreateInfo(node,new ThompsonNodeFAInfo());
        }

    }


    // This class represents the Thompson algorithm. It depends on the classes
    // defined in the ThompsonHelper.cs which take the responsibility of FA
    // transformation during the Thompson algorithm steps
    public class ThompsonVisitorFlatten : CASTAbstractConcreteVisitor<FA>{
        /// <summary>
        /// This field stores the final output NFA when the algorithm execution completes
        /// </summary>
        private FA m_NFA=null;

        /// <summary>
        /// This dictionary holds thompson-related information for every FA
        /// produced during the application of the Thompson algorithm
        /// </summary>
        Dictionary<FA,ThompsonInfo> m_thompsonInfo = new Dictionary<FA, ThompsonInfo>();

        // While the algorithm traverses the AST the following field keeps the 
        // Regular expression owning the current AST subtree
        private CRegexpStatement m_currentRegularExpression = null;

        private FA m_resultFA;

        /// <summary>
        /// This fields stores Thompson Algorithm execution and reporting options
        /// </summary>
        private UOPCore.Options<ThompsonOptions> m_options=new UOPCore.Options<ThompsonOptions>((int)ThompsonOptions.TO_DEFAULT);

        private ThompsonReporting m_ReportingServices;
        
        public FA M_Nfa{
            get{ return m_NFA; }
        }

        public ThompsonVisitorFlatten(ThompsonOptions options) : base(){
            // Initialize Thompson options 
            m_options = new UOPCore.Options<ThompsonOptions>(options);
            m_ReportingServices = new ThompsonReporting(m_options);
            
        }

        public override FA VisitLexerDescription(CASTElement currentNode) {
            int i = 0;
            FA leftFa=null, rightFa;
            CGraph.CMergeGraphOperation.MergeOptions mergeOptions=CGraph.CMergeGraphOperation.MergeOptions.MO_DEFAULT;
            CSubsetConstructionAlgorithm subcon;
            CHopcroftAlgorithm hopmin;
            CLexerDescription lexerDescription=currentNode as CLexerDescription;
            List<CASTElement> rExpStatements=lexerDescription.GetContextChildren(ContextType.CT_LEXERDESCRIPTION_BODY);

            // Preserve labels of RegExp-derived NFA
            if (!m_options.IsSet(ThompsonOptions.TO_STEPS)) {
                mergeOptions = CGraph.CMergeGraphOperation.MergeOptions.MO_PRESERVELABELS;
            }

            //1. Create FA 
            foreach (var rExpStatement in rExpStatements) {
                if (i > 0) {
                    rightFa = Visit(rExpStatement);
                    //2.Synthesize the two FAs to a new one
                    CThompsonAlternationTemplate alttempSyn = new CThompsonAlternationTemplate(this);
                    leftFa = alttempSyn.Sythesize(leftFa, rightFa, mergeOptions);

                    // Prefix node elements of the resulting graph with 
                    CIt_GraphNodes it = new CIt_GraphNodes(leftFa);
                    for (it.Begin(); !it.End(); it.Next()) {
                        leftFa.PrefixElementLabel(leftFa.GetFANodePrefix(it.M_CurrentItem), it.M_CurrentItem);
                    }
                }
                else {
                    leftFa = Visit(rExpStatement);
                }
                i++;
            }

            m_NFA = leftFa;
            m_ReportingServices.ExctractThompsonStep(m_NFA,@"../Debug/merge.dot");
            if (i > 1) {
                m_ReportingServices.AddThompsonStepToReporting(m_NFA, true);
            }
            else {
                m_ReportingServices.ThompsonStepsGenerate();
            }

            //return the final-synthesized FA
            return m_NFA;
        }

        public override FA VisitRegexpbasicParen(CASTElement currentNode){
            CASTComposite curNode = currentNode as CASTComposite;

            m_NFA = Visit(curNode.GetChild(ContextType.CT_RGEXPBASIC_PAREN, 0));

            return m_NFA;
        }

        public override FA AggregateResult(FA intermediateResult){
            if  (intermediateResult != null) {
                m_resultFA = intermediateResult;
            }
            return m_resultFA;
        }

        public override FA VisitRegexpStatement(CASTElement currentNode)
        {
            CRegexpStatement curNode = currentNode as CRegexpStatement;

            m_currentRegularExpression = curNode;

            // Generate the FA for the current regular expression
            FA fa =  base.VisitRegexpStatement(currentNode);

            // Name the nodes of current branch of the FA with the name of the current regular expression
            fa.SetFANodePrefix(curNode.M_StatementID);
            fa.SetFANodesLineDependency(curNode.M_Line);

            m_currentRegularExpression = null;

            return fa;
        }

        public override FA VisitRegexpAlternation(CASTElement currentNode)
        {
            CRegexpAlternation altNode = currentNode as CRegexpAlternation;
            CSubsetConstructionAlgorithm subcon;
            CHopcroftAlgorithm hopmin;
            //1. Create FA 
            CThompsonAlternationTemplate alttempSyn = new CThompsonAlternationTemplate(this);
            FA leftFa = Visit(altNode.GetChild(ContextType.CT_REGEXPALTERNATION_TERMS, 0));
            
            CIt_GraphNodes it = new CIt_GraphNodes(leftFa);
            for (it.Begin(); !it.End(); it.Next()) {
                leftFa.PrefixElementLabel(leftFa.GetFANodePrefix(it.M_CurrentItem), it.M_CurrentItem);
            }

            FA rightFa = Visit(altNode.GetChild(ContextType.CT_REGEXPALTERNATION_TERMS, 1));

            it = new CIt_GraphNodes(rightFa);
            for (it.Begin(); !it.End(); it.Next()) {
                rightFa.PrefixElementLabel(rightFa.GetFANodePrefix(it.M_CurrentItem), it.M_CurrentItem);
            }

            //2.Synthesize the two FAs to a new one
            m_NFA = alttempSyn.Sythesize(leftFa, rightFa,CGraph.CMergeGraphOperation.MergeOptions.MO_DEFAULT);


            m_ReportingServices.ExctractThompsonStep(m_NFA, @"../Debug/Alternation_" + m_NFA.M_Label + ".dot");
            m_ReportingServices.AddThompsonStepToReporting(m_NFA);

            
            //return the final-synthesized FA
            return m_NFA;
        }

        public override FA VisitRegexpConcatenation(CASTElement currentNode)
        {
            CRegexpConcatenation altNode = currentNode as CRegexpConcatenation;
            
            //1. Create FA 
            CThompsonConcatenationTemplate alttempSyn = new CThompsonConcatenationTemplate(this);
            FA leftFa = Visit(altNode.GetChild(ContextType.CT_REGEXPCONCATENATION_TERMS, 0));
            FA rightFa = Visit(altNode.GetChild(ContextType.CT_REGEXPCONCATENATION_TERMS, 1));
            //2.Synthesize the two FAs to a new one
            m_NFA = alttempSyn.Synthesize(leftFa, rightFa);

            CIt_GraphNodes it = new CIt_GraphNodes(m_NFA);
            for (it.Begin(); !it.End(); it.Next()) {
                m_NFA.PrefixElementLabel(m_currentRegularExpression.M_StatementID, it.M_CurrentItem);
            }

            m_ReportingServices.ExctractThompsonStep(m_NFA, @"../Debug/Concatenation_" + m_NFA.M_Label + ".dot");
            m_ReportingServices.AddThompsonStepToReporting(m_NFA);

            return m_NFA;
        }

        public override FA VisitRegexpClosure(CASTElement currentNode) {

            CRegexpClosure closNode = currentNode as CRegexpClosure;
            
            //1.Create FA
            CThompsonClosureTemplate newFA = new CThompsonClosureTemplate(this);
            //2.Check the type of the closure
            if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_NONEORMULTIPLE) {
                FA customFA = Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP,0));
                m_NFA = newFA.SynthesizeNoneOrMul(customFA);
            }

            else if(closNode.M_ClosureType==CRegexpClosure.ClosureType.CLT_ONEORMULTIPLE){
                FA customFA = Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP, 0));
                m_NFA = newFA.SynthesisOneOrMul(customFA);
            }
            else if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_ONEORZERO){
                FA customFA = Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP, 0));
                m_NFA = newFA.SynthesizeOneOrNone(customFA);
            }
            else if ( closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_FINITECLOSURE) {
                CClosureRange rangeNode = closNode.GetChild(ContextType.CT_REGEXPCLOSURE_QUANTIFIER, 0) as CClosureRange;
                FA customFA = Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP, 0));
                m_NFA = newFA.SynthesizeFinite(customFA, rangeNode.M_ClosureMultiplicityLB,
                    rangeNode.M_ClosureMultiplicityUB);
            }
            else if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_NONEORMULTIPLE_NONGREEDY) {
                //TODO 
            }
            else if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_ONEORMULTIPLE_NONGREEDY) {
                //TODO 
            }
            else {
                Console.WriteLine("No proper input");
            }

            CIt_GraphNodes it = new CIt_GraphNodes(m_NFA);
            for (it.Begin(); !it.End(); it.Next()) {
                m_NFA.PrefixElementLabel(m_currentRegularExpression.M_StatementID, it.M_CurrentItem);
            }
            m_ReportingServices.ExctractThompsonStep(m_NFA, @"../Debug/Closure_" + m_NFA.M_Label + ".dot");
            m_ReportingServices.AddThompsonStepToReporting(m_NFA);

            //4.Pass FA to the predecessor
            return m_NFA;
        }

        public override FA VisitRegexpbasicChar(CASTElement currentNode)
        {
            CRegexpbasicChar charNode = currentNode as CRegexpbasicChar;
            FAGraphQueryInfo FAInfo;
            //1.Create FA
            m_NFA = new FA();
            FAInfo = new FAGraphQueryInfo(m_NFA, FA.m_FAINFOKEY);
            //2.Create nodes initial-final
            CGraphNode init = m_NFA.CreateGraphNode<CGraphNode>();
            CGraphNode final = m_NFA.CreateGraphNode<CGraphNode>();
            m_NFA.M_Initial = init;
            m_NFA.SetFinalState(final);
            m_NFA.M_Alphabet.AddSet(charNode.M_CharRangeSet);
            
            //3.Draw the edge including the character
            CGraphEdge newEdge = m_NFA.AddGraphEdge<CGraphEdge, CGraphNode>(init, final,GraphType.GT_DIRECTED);
            FAInfo.Info(newEdge).M_TransitionCharSet= charNode.M_CharRangeSet;
            //4.Pass FA to the predecessor

            m_NFA.PrefixGraphElementLabels(m_currentRegularExpression.M_StatementID, GraphElementType.ET_NODE);

            m_ReportingServices.ExctractThompsonStep(m_NFA, @"../Debug/BasicChar_" + charNode.M_CharRangeSet.ToString() + ".dot");
            m_ReportingServices.AddThompsonStepToReporting(m_NFA);
            
            return m_NFA;
        }

        public override FA VisitRegexpbasicSet(CASTElement currentNode){
            CRegexpbasicSet setNode = currentNode as CRegexpbasicSet;
            FAGraphQueryInfo FAInfo;

            //Create FA
            m_NFA = new FA();
            FAInfo = new FAGraphQueryInfo(m_NFA, FA.m_FAINFOKEY);
            CGraphNode init = m_NFA.CreateGraphNode<CGraphNode>();
            CGraphNode final = m_NFA.CreateGraphNode<CGraphNode>();
            m_NFA.M_Initial = init;
            m_NFA.SetFinalState(final);
            m_NFA.M_Alphabet.AddSet(setNode.MSet);

            CGraphEdge newEdge = m_NFA.AddGraphEdge<CGraphEdge, CGraphNode>(init, final, GraphType.GT_DIRECTED);
            FAInfo.Info(newEdge).M_TransitionCharSet = setNode.MSet;
            //4.Pass FA to the predecessor
            
            m_NFA.PrefixGraphElementLabels(m_currentRegularExpression.M_StatementID,GraphElementType.ET_NODE);

            m_ReportingServices.ExctractThompsonStep(m_NFA, @"../Debug/BasicSet_" + setNode.MSet.ToString() + ".dot");
            m_ReportingServices.AddThompsonStepToReporting(m_NFA);

            return m_NFA;
        }

        public override FA VisitRange(CASTElement currentNode){
            CRange rangeNode = currentNode as CRange;
            FAGraphQueryInfo FAInfo;

            //1.Create FA
            m_NFA = new FA();
            FAInfo = new FAGraphQueryInfo(m_NFA, FA.m_FAINFOKEY);
            //2.Create nodes initial-final
            CGraphNode init = m_NFA.CreateGraphNode<CGraphNode>();
            CGraphNode final = m_NFA.CreateGraphNode<CGraphNode>();
            m_NFA.M_Initial = init;
            m_NFA.SetFinalState(final);
            m_NFA.M_Alphabet.AddRange(rangeNode.MRange);

            //3.Draw the edge including the character
            CGraphEdge newEdge = m_NFA.AddGraphEdge<CGraphEdge, CGraphNode>(init, final, GraphType.GT_DIRECTED);
            FAInfo.Info(newEdge).M_TransitionCharSet= (CCharRangeSet)rangeNode.MRange;
            newEdge.SetLabel(rangeNode.MRange.ToString());

            m_ReportingServices.ExctractThompsonStep(m_NFA, @"../Debug/Range_" + rangeNode.MRange.ToString() + ".dot");
            m_ReportingServices.AddThompsonStepToReporting(m_NFA);
            //4.Pass FA to the predecessor
            return m_NFA;
        }
    }
}
