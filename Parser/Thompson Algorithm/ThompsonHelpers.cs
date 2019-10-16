using System;
using System.Collections.Generic;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.Thompson_Algorithm;
using Parser.UOPCore;

internal abstract class CThompsonTemplates {
    protected ThompsonInfo m_ThompsonInfo;
    protected object m_ThompsonInfoKey;
    protected FA m_currentFA;

    protected CGraphNode m_newFASource;
    protected CGraphNode m_newFATarget;

    protected CGraph.CMergeGraphOperation m_mergeOperation;
    
    protected CThompsonTemplates(object infokey) {
        m_ThompsonInfoKey = infokey;
    }
    
}

internal class CThompsonClosureTemplate : CThompsonTemplates
{
    public CThompsonClosureTemplate(object infokey) :base(infokey) {

    }
    private FA CreateNewFA(FA synth) {
        FA m_currentFA = new FA();
        m_ThompsonInfo = new ThompsonInfo(m_currentFA, m_ThompsonInfoKey);

        //2.Merge graph
        m_mergeOperation = m_currentFA.Merge(synth, CGraph.CMergeGraphOperation.MergeOptions.MO_DEFAULT);
        m_mergeOperation.MergeGraphInfo(synth, GraphElementType.ET_EDGE, FA.m_FAINFOKEY);
        m_mergeOperation.MergeGraphInfo(synth, GraphElementType.ET_GRAPH, FA.m_FAINFOKEY);
        m_mergeOperation.MergeGraphInfo(synth, GraphElementType.ET_NODE, FA.m_FAINFOKEY);

        m_mergeOperation.MergeGraphInfo(synth, GraphElementType.ET_EDGE, m_ThompsonInfoKey);
        m_mergeOperation.MergeGraphInfo(synth, GraphElementType.ET_GRAPH, m_ThompsonInfoKey);
        m_mergeOperation.MergeGraphInfo(synth, GraphElementType.ET_NODE, m_ThompsonInfoKey);

        // Create boundary nodes
        m_newFASource = m_currentFA.CreateGraphNode<CGraphNode>();
        m_ThompsonInfo.InitNodeInfo(m_newFASource,new ThompsonNodeFAInfo());

        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_newFASource, m_mergeOperation.GetMirrorNode(synth.M_Initial), GraphType.GT_DIRECTED);
        m_newFATarget = m_currentFA.CreateGraphNode<CGraphNode>();
        m_ThompsonInfo.InitNodeInfo(m_newFATarget, new ThompsonNodeFAInfo());

        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_mergeOperation.GetMirrorNode(synth.GetFinalStates()[0]), m_newFATarget, GraphType.GT_DIRECTED);

        //4.Create the initial and the final node
        m_currentFA.M_Initial = m_newFASource;
        m_currentFA.SetFinalState(m_newFATarget);
        m_currentFA.UpdateAlphabet();

        return m_currentFA;
    }

    internal FA SynthesizeNoneOrMul(FA synth){
        // 1. Create new FA
        FA m_currentFA = CreateNewFA(synth);
        
        // Draw closure override edge that connects the initial and the final node
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_newFASource, m_newFATarget, GraphType.GT_DIRECTED);
        // Draw the closure loop edge
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_mergeOperation.GetMirrorNode(synth.GetFinalStates()[0]), m_mergeOperation.GetMirrorNode(synth.M_Initial),
            GraphType.GT_DIRECTED);
        m_ThompsonInfo.SetNodeClosureEntrance(m_mergeOperation.GetMirrorNode(synth.M_Initial), true);
        m_ThompsonInfo.SetNodeClosureExit(m_mergeOperation.GetMirrorNode(synth.GetFinalStates()[0]), true);

        //7.Return result
        return m_currentFA;
    }

    internal FA SynthesisOneOrMul(FA synth){
        m_currentFA = CreateNewFA(synth);
        // Draw the closure loop edge
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_mergeOperation.GetMirrorNode(synth.GetFinalStates()[0]), m_mergeOperation.GetMirrorNode(synth.M_Initial),
            GraphType.GT_DIRECTED);

        m_ThompsonInfo.SetNodeClosureEntrance(m_mergeOperation.GetMirrorNode(synth.M_Initial),true);
        m_ThompsonInfo.SetNodeClosureExit(m_mergeOperation.GetMirrorNode(synth.GetFinalStates()[0]), true);
        //7.Return result
        return m_currentFA;
    }

    internal FA SynthesizeOneOrNone(FA synth){
        m_currentFA = CreateNewFA(synth);
        
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_newFASource, m_newFATarget, GraphType.GT_DIRECTED);

        //7.Return result
        return m_currentFA;
    }

    internal FA SynthesizeFinite(FA synth, int lb, int up) {
        m_currentFA = CreateNewFA(synth);
       
        if (lb == 0) {
            m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_newFASource, m_newFATarget, GraphType.GT_DIRECTED);
        }

        if (up > 1) {
            m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(m_mergeOperation.GetMirrorNode(synth.GetFinalStates()[0]), m_mergeOperation.GetMirrorNode(synth.M_Initial),
                GraphType.GT_DIRECTED);
            m_ThompsonInfo.SetNodeClosureEntrance(m_mergeOperation.GetMirrorNode(synth.M_Initial), true);
            m_ThompsonInfo.SetNodeClosureExit(m_mergeOperation.GetMirrorNode(synth.GetFinalStates()[0]), true);
        }

        return m_currentFA;
    }
}
internal class CThompsonConcatenationTemplate : CThompsonTemplates
{
    public CThompsonConcatenationTemplate(object infokey) : base(infokey) {
    }

    internal FA Synthesize(FA l, FA r){
        FA tempFA = new FA();
        m_ThompsonInfo = new ThompsonInfo(tempFA,m_ThompsonInfoKey);
        //2.Merge left graph
        CGraph.CMergeGraphOperation lmerge = tempFA.Merge(l,CGraph.CMergeGraphOperation.MergeOptions.MO_DEFAULT);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_EDGE, FA.m_FAINFOKEY);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_GRAPH, FA.m_FAINFOKEY);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_NODE, FA.m_FAINFOKEY);

        lmerge.MergeGraphInfo(l, GraphElementType.ET_EDGE, m_ThompsonInfoKey);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_GRAPH, m_ThompsonInfoKey);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_NODE, m_ThompsonInfoKey);

        CGraphNode il = lmerge.GetMirrorNode(l.M_Initial);
        CGraphNode fl = lmerge.GetMirrorNode(l.GetFinalStates()[0]);

        //3.Merge right graph
        CGraph.CMergeGraphOperation rmerge = tempFA.Merge(r,CGraph.CMergeGraphOperation.MergeOptions.MO_DEFAULT);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_EDGE, FA.m_FAINFOKEY);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_GRAPH, FA.m_FAINFOKEY);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_NODE, FA.m_FAINFOKEY);

        rmerge.MergeGraphInfo(r, GraphElementType.ET_EDGE, m_ThompsonInfoKey);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_GRAPH, m_ThompsonInfoKey);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_NODE, m_ThompsonInfoKey);

        CGraphNode ir = rmerge.GetMirrorNode(r.M_Initial);
        CGraphNode fr = rmerge.GetMirrorNode(r.GetFinalStates()[0]);

        //4.Create the initial and the final node
        tempFA.M_Initial = il;
        tempFA.SetFinalState(fr);

        tempFA.AddGraphEdge<CGraphEdge, CGraphNode>(fl, ir, GraphType.GT_DIRECTED);

        //7.Return result
        return tempFA;

    }
}
internal class CThompsonAlternationTemplate : CThompsonTemplates
{
    public CThompsonAlternationTemplate(object infokey) : base(infokey) { }

    internal FA Sythesize(FA l, FA r, CGraph.CMergeGraphOperation.MergeOptions options)
    {
        //1.Create FA
        m_currentFA = new FA();
        m_ThompsonInfo = new ThompsonInfo(m_currentFA, m_ThompsonInfoKey);

        //2.Merge left graph
        CGraph.CMergeGraphOperation lmerge = m_currentFA.Merge(l,options);
        //Console.WriteLine(l.ToString());
        lmerge.MergeGraphInfo(l,GraphElementType.ET_EDGE, FA.m_FAINFOKEY);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_GRAPH, FA.m_FAINFOKEY);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_NODE, FA.m_FAINFOKEY);

        lmerge.MergeGraphInfo(l, GraphElementType.ET_EDGE, m_ThompsonInfoKey);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_GRAPH, m_ThompsonInfoKey);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_NODE, m_ThompsonInfoKey);

        CGraphNode il = lmerge.GetMirrorNode(l.M_Initial);
        CGraphNode fl = lmerge.GetMirrorNode(l.GetFinalStates()[0]);

        //3.Merge right graph
        CGraph.CMergeGraphOperation rmerge = m_currentFA.Merge(r,options);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_EDGE, FA.m_FAINFOKEY);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_GRAPH, FA.m_FAINFOKEY);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_NODE, FA.m_FAINFOKEY);

        rmerge.MergeGraphInfo(r, GraphElementType.ET_EDGE, m_ThompsonInfoKey);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_GRAPH, m_ThompsonInfoKey);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_NODE, m_ThompsonInfoKey);

        CGraphNode ir = rmerge.GetMirrorNode(r.M_Initial);
        CGraphNode fr = rmerge.GetMirrorNode(r.GetFinalStates()[0]);

        //4.Create the initial and the final node
        CGraphNode FAinit = m_currentFA.CreateGraphNode<CGraphNode>();
        CGraphNode FAfinal = m_currentFA.CreateGraphNode<CGraphNode>();
        m_ThompsonInfo.InitNodeInfo(FAinit,new ThompsonNodeFAInfo());
        m_ThompsonInfo.InitNodeInfo(FAfinal,new ThompsonNodeFAInfo());

        m_currentFA.M_Initial = FAinit;
        m_currentFA.SetFinalState(FAfinal);
        m_currentFA.UpdateAlphabet();
        
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(FAinit, il, GraphType.GT_DIRECTED);
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(FAinit, ir, GraphType.GT_DIRECTED);
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(fr, FAfinal, GraphType.GT_DIRECTED);
        m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(fl, FAfinal, GraphType.GT_DIRECTED);

        //7.Return result
        return m_currentFA;
    }
}

internal class CThompsonRangeTemplate : CThompsonTemplates {
    public CThompsonRangeTemplate(object infokey) : base(infokey) {
    }

    internal FA Synthesize(CRange rangeNode) {
        FAGraphQueryInfo FAInfo;

        //1.Create FA
        m_currentFA = new FA();
        m_ThompsonInfo = new ThompsonInfo(m_currentFA,m_ThompsonInfoKey);
        FAInfo = new FAGraphQueryInfo(m_currentFA, FA.m_FAINFOKEY);
        
        //2.Create nodes initial-final
        CGraphNode init = m_currentFA.CreateGraphNode<CGraphNode>();
        CGraphNode final = m_currentFA.CreateGraphNode<CGraphNode>();
        m_currentFA.M_Initial = init;
        m_currentFA.SetFinalState(final);
        m_currentFA.M_Alphabet.AddRange(rangeNode.MRange);

        //3.Draw the edge including the character
        CGraphEdge newEdge = m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(init, final, GraphType.GT_DIRECTED);
        FAInfo.Info(newEdge).M_TransitionCharSet = (CCharRangeSet)rangeNode.MRange;
        newEdge.SetLabel(rangeNode.MRange.ToString());
        m_ThompsonInfo.SetNodeWithNon_e_Edges(init, true);
        m_ThompsonInfo.SetNodeWithNon_e_Edges(final, false);

        return m_currentFA;
    }
}

internal class CThompsonBasicSet : CThompsonTemplates {
    public CThompsonBasicSet(object infokey) : base(infokey) {
    }

    internal FA Synthesize(CCharRangeSet set) {
        FAGraphQueryInfo FAInfo;
        m_currentFA = new FA();
        m_ThompsonInfo = new ThompsonInfo(m_currentFA, m_ThompsonInfoKey);
        FAInfo = new FAGraphQueryInfo(m_currentFA, FA.m_FAINFOKEY);

        //Create FA
        CGraphNode init = m_currentFA.CreateGraphNode<CGraphNode>();
        CGraphNode final = m_currentFA.CreateGraphNode<CGraphNode>();
        m_currentFA.M_Initial = init;
        m_currentFA.SetFinalState(final);
        m_currentFA.M_Alphabet.AddSet(set);

        CGraphEdge newEdge = m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(init, final, GraphType.GT_DIRECTED);
        FAInfo.Info(newEdge).M_TransitionCharSet =set;
        m_ThompsonInfo.SetNodeWithNon_e_Edges(init, true);
        m_ThompsonInfo.SetNodeWithNon_e_Edges(final, false);

        return m_currentFA;
    }
}

internal class CThompsonCharTemplate : CThompsonTemplates {
    public CThompsonCharTemplate(object infokey) : base(infokey) {
    }

    internal FA Synthesize(CCharRangeSet set) {
        FAGraphQueryInfo FAInfo;
        m_currentFA = new FA();
        m_ThompsonInfo = new ThompsonInfo(m_currentFA, m_ThompsonInfoKey);
        FAInfo = new FAGraphQueryInfo(m_currentFA, FA.m_FAINFOKEY);

        //2.Create nodes initial-final
        CGraphNode init = m_currentFA.CreateGraphNode<CGraphNode>();
        CGraphNode final = m_currentFA.CreateGraphNode<CGraphNode>();
        m_currentFA.M_Initial = init;
        m_currentFA.SetFinalState(final);
        m_currentFA.M_Alphabet.AddSet(set);

        //3.Draw the edge including the character
        CGraphEdge newEdge = m_currentFA.AddGraphEdge<CGraphEdge, CGraphNode>(init, final, GraphType.GT_DIRECTED);
        FAInfo.Info(newEdge).M_TransitionCharSet = set;
        m_ThompsonInfo.SetNodeWithNon_e_Edges(init,true);
        m_ThompsonInfo.SetNodeWithNon_e_Edges(final, false);
        
        return m_currentFA;
    }
}

