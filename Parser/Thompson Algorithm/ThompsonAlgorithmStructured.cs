using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.ASTVisitor;
using Parser.Hopcroft;
using Parser.SubsetConstruction;
using Parser.UOPCore;

namespace Parser.Thompson_Algorithm {

    // This class represents the Thompson algorithm. It depends on the classes
    // defined in the ThompsonHelper.cs which take the responsibility of FA
    // transformation during the Thompson algorithm steps
    public class ThompsonAlgorithmStructured : CASTAbstractConcreteVisitor<FA> {
        /// <summary>
        /// This field stores the current the regular expression currently being transformed
        /// by the Thompson algorithm
        /// </summary>
        private FA m_currentNFA = null;

        private FA m_netVisitResult=null;
        
        /// <summary>
        /// This field stores the current line of the regular expression being transformed by
        /// the Thompson algorithm
        /// </summary>
        private uint m_currentLine;

        // While the algorithm traverses the AST the following field keeps the 
        // Regular expression owning the current AST subtree
        private CRegexpStatement m_currentRegularExpression = null;

        /// <summary>
        /// Records information for each regular expression
        /// </summary>
        private Dictionary<uint, RERecord> m_reRecords;

        /// <summary>
        /// This fields stores Thompson Algorithm execution and reporting options
        /// </summary>
        private UOPCore.Options<ThompsonOptions> m_options = new UOPCore.Options<ThompsonOptions>((int)ThompsonOptions.TO_DEFAULT);

        private ThompsonReporting m_ReportingServices;

        public Dictionary<uint, RERecord> M_ReRecords => m_reRecords;

        public ThompsonAlgorithmStructured(ThompsonOptions options, Dictionary<uint, RERecord> mReRecords) : base() {
            // Initialize Thompson options 
            m_options = new UOPCore.Options<ThompsonOptions>(options);
            m_reRecords = mReRecords;
            m_ReportingServices = new ThompsonReporting(m_options);
        }

        public override FA VisitRegexpAlternation(CASTElement currentNode) {
            CRegexpAlternation altNode = currentNode as CRegexpAlternation;
            CSubsetConstructionAlgorithm subcon;
            CHopcroftAlgorithm hopmin;
            //1. Create FA 
            CThompsonAlternationTemplate alttempSyn = new CThompsonAlternationTemplate(this.GetHashCode());
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
            m_currentNFA = alttempSyn.Sythesize(leftFa, rightFa, CGraph.CMergeGraphOperation.MergeOptions.MO_DEFAULT);

            m_ReportingServices.ExctractThompsonStep(m_currentNFA, @"Alternation_" + m_currentNFA.M_Label + ".dot",this.GetHashCode());
            m_ReportingServices.AddThompsonStepToReporting(m_currentNFA,this.GetHashCode());


            //return the final-synthesized FA
            return m_currentNFA;
        }

        public override FA VisitRegexpbasicParen(CASTElement currentNode) {
            CASTComposite curNode = currentNode as CASTComposite;

            m_currentNFA = Visit(curNode.GetChild(ContextType.CT_RGEXPBASIC_PAREN, 0));

            return m_currentNFA;
        }

        public override FA AggregateResult(FA intermediateResult) {
            if ( intermediateResult != null) {
                m_netVisitResult = intermediateResult;
            }
            return m_netVisitResult;
        }

        public override FA VisitRegexpStatement(CASTElement currentNode) {
            CRegexpStatement curNode = currentNode as CRegexpStatement;

            m_currentRegularExpression = curNode;

            // Generate the FA for the current regular expression
            FA fa = base.VisitRegexpStatement(currentNode);
            m_currentNFA = fa;
            m_currentLine = curNode.M_Line;

            m_ReportingServices.ExctractThompsonStep(fa,"merge"+m_currentLine+".dot",this.GetHashCode(),true);

            // Record the derived NFA to the RERecords 
            m_reRecords[curNode.M_Line].M_Nfa = fa;

            // Name the nodes of current branch of the FA with the name of the current regular expression
            fa.SetFANodePrefix(curNode.M_StatementID);
            fa.SetFANodesLineDependency(curNode.M_Line);

            m_currentRegularExpression = null;

            return fa;
        }

        public override FA VisitRegexpConcatenation(CASTElement currentNode) {
            CRegexpConcatenation altNode = currentNode as CRegexpConcatenation;

            //1. Create FA 
            CThompsonConcatenationTemplate alttempSyn = new CThompsonConcatenationTemplate(this.GetHashCode());
            FA leftFa = Visit(altNode.GetChild(ContextType.CT_REGEXPCONCATENATION_TERMS, 0));
            FA rightFa = Visit(altNode.GetChild(ContextType.CT_REGEXPCONCATENATION_TERMS, 1));
            //2.Synthesize the two FAs to a new one
            m_currentNFA = alttempSyn.Synthesize(leftFa, rightFa);

            CIt_GraphNodes it = new CIt_GraphNodes(m_currentNFA);
            for (it.Begin(); !it.End(); it.Next()) {
                m_currentNFA.PrefixElementLabel(m_currentRegularExpression.M_StatementID, it.M_CurrentItem);
            }

            m_ReportingServices.ExctractThompsonStep(m_currentNFA, @"Concatenation_" + m_currentNFA.M_Label + ".dot", this.GetHashCode());
            m_ReportingServices.AddThompsonStepToReporting(m_currentNFA,this.GetHashCode());

            return m_currentNFA;
        }

        public override FA VisitRegexpClosure(CASTElement currentNode) {

            CRegexpClosure closNode = currentNode as CRegexpClosure;
            ThompsonInfo tinfo;
            
            //1.Create FA
            CThompsonClosureTemplate newFA = new CThompsonClosureTemplate(this.GetHashCode(), currentNode.M_Text);
            //2.Check the type of the closure
            if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_NONEORMULTIPLE) {
                FA customFA = Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP, 0));
                m_currentNFA = newFA.SynthesizeNoneOrMul(customFA);
                tinfo = new ThompsonInfo(m_currentNFA,this.GetHashCode());
                if (currentNode.M_Text != null) {
                    tinfo.SetNodeClosureExpression(currentNode.M_Text);
                }
                else {
                    Console.WriteLine("Warning!!! Closure text representation is null");
                }
            }

            else if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_ONEORMULTIPLE) {
                FA customFA = Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP, 0));
                m_currentNFA = newFA.SynthesisOneOrMul(customFA);
                tinfo = new ThompsonInfo(m_currentNFA, this.GetHashCode());
                if (currentNode.M_Text != null) {
                    tinfo.SetNodeClosureExpression(currentNode.M_Text);
                }
                else {
                    Console.WriteLine("Warning!!! Closure text representation is null");
                }
            }
            else if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_ONEORZERO) {
                FA customFA = Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP, 0));
                m_currentNFA = newFA.SynthesizeOneOrNone(customFA);
            }
            else if (closNode.M_ClosureType == CRegexpClosure.ClosureType.CLT_FINITECLOSURE) {
                CClosureRange rangeNode= closNode.GetChild(ContextType.CT_REGEXPCLOSURE_QUANTIFIER,0) as CClosureRange;
                FA customFA=Visit(closNode.GetChild(ContextType.CT_REGEXPCLOSURE_REGEXP,0));
                m_currentNFA = newFA.SynthesizeFinite(customFA, rangeNode.M_ClosureMultiplicityLB,
                    rangeNode.M_ClosureMultiplicityUB);
                tinfo = new ThompsonInfo(m_currentNFA, this.GetHashCode());
                if (currentNode.M_Text != null) {
                    tinfo.SetNodeClosureExpression(currentNode.M_Text);
                }
                else {
                    Console.WriteLine("Warning!!! Closure text representation is null");
                }
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

            CIt_GraphNodes it = new CIt_GraphNodes(m_currentNFA);
            for (it.Begin(); !it.End(); it.Next()) {
                m_currentNFA.PrefixElementLabel(m_currentRegularExpression.M_StatementID, it.M_CurrentItem);
            }
            m_ReportingServices.ExctractThompsonStep(m_currentNFA, @"Closure_" + m_currentNFA.M_Label + ".dot", this.GetHashCode());
            m_ReportingServices.AddThompsonStepToReporting(m_currentNFA,this.GetHashCode());

            //4.Pass FA to the predecessor
            return m_currentNFA;
        }

        public override FA VisitRegexpbasicChar(CASTElement currentNode) {
            CRegexpbasicChar charNode = currentNode as CRegexpbasicChar;
            
            CThompsonCharTemplate charTemplate = new CThompsonCharTemplate(this.GetHashCode());
            m_currentNFA = charTemplate.Synthesize(charNode.M_CharRangeSet);

            m_currentNFA.PrefixGraphElementLabels(m_currentRegularExpression.M_StatementID, GraphElementType.ET_NODE);

            m_ReportingServices.ExctractThompsonStep(m_currentNFA, @"BasicChar_" + charNode.M_CharRangeSet.ToString() + ".dot", this.GetHashCode());
            m_ReportingServices.AddThompsonStepToReporting(m_currentNFA,this.GetHashCode());

            return m_currentNFA;
        }

        public override FA VisitRegexpbasicSet(CASTElement currentNode) {
            CRegexpbasicSet setNode = currentNode as CRegexpbasicSet;
            
            CThompsonBasicSet setTemplate = new CThompsonBasicSet(this.GetHashCode());
            m_currentNFA = setTemplate.Synthesize(setNode.MSet);

            m_currentNFA.PrefixGraphElementLabels(m_currentRegularExpression.M_StatementID, GraphElementType.ET_NODE);

            m_ReportingServices.ExctractThompsonStep(m_currentNFA, @"BasicSet_" + setNode.MSet.ToString() + ".dot", this.GetHashCode());
            m_ReportingServices.AddThompsonStepToReporting(m_currentNFA,this.GetHashCode());

            return m_currentNFA;
        }

        public override FA VisitRange(CASTElement currentNode) {
            CRange rangeNode = currentNode as CRange;
            CThompsonRangeTemplate rangeTemplate = new CThompsonRangeTemplate(this.GetHashCode());

            m_currentNFA = rangeTemplate.Synthesize(rangeNode);

            m_ReportingServices.ExctractThompsonStep(m_currentNFA, @"Range_" + rangeNode.MRange.ToString() + ".dot", this.GetHashCode());
            m_ReportingServices.AddThompsonStepToReporting(m_currentNFA,this.GetHashCode());
            //4.Pass FA to the predecessor
            return m_currentNFA;
        }
    }
}
