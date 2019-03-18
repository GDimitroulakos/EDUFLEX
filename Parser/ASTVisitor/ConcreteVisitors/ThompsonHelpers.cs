using System;
using System.Collections.Generic;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.UOPCore;

internal abstract class CThompsonTemplates
{
}

internal class CThompsonClosureTemplate : CThompsonTemplates
{
    internal FA SynthesizeNoneOrMul(FA synth){
        FA tempFA = new FA();
        //2.Merge graph
        CGraph.CMergeGraphOperation merged = tempFA.Merge(synth);
        merged.MergeGraphInfo(synth, GraphElementType.ET_EDGE, FA.m_TRANSITIONSKEY);


        CGraphNode il = tempFA.CreateGraphNode();
        tempFA.AddGraphEdge(il, merged.GetMirrorNode(synth.M_Initial), GraphType.GT_DIRECTED);
        CGraphNode fl = tempFA.CreateGraphNode();
        tempFA.AddGraphEdge(merged.GetMirrorNode(synth.GetFinalStates()[0]) , fl, GraphType.GT_DIRECTED);

        //4.Create the initial and the final node
        tempFA.M_Initial = il;
        tempFA.SetFinalState(fl);
        tempFA.UpdateAlphabet();

        tempFA.AddGraphEdge(il, fl, GraphType.GT_DIRECTED);
        tempFA.AddGraphEdge(merged.GetMirrorNode(synth.GetFinalStates()[0]), merged.GetMirrorNode(synth.M_Initial),
            GraphType.GT_DIRECTED);

        //7.Return result
        return tempFA;
    }

    internal FA SynthesisOneOrMul(FA synth){
        FA tempFA = new FA();
        //2.Merge graph
        CGraph.CMergeGraphOperation merged = tempFA.Merge(synth);
        merged.MergeGraphInfo(synth, GraphElementType.ET_EDGE, FA.m_TRANSITIONSKEY);


        CGraphNode il = tempFA.CreateGraphNode();
        tempFA.AddGraphEdge(il, merged.GetMirrorNode(synth.M_Initial), GraphType.GT_DIRECTED);
        CGraphNode fl = tempFA.CreateGraphNode();
        tempFA.AddGraphEdge(merged.GetMirrorNode(synth.GetFinalStates()[0]), fl, GraphType.GT_DIRECTED);

        //4.Create the initial and the final node
        tempFA.M_Initial = il;
        tempFA.SetFinalState(fl);
        tempFA.UpdateAlphabet();

        tempFA.AddGraphEdge(merged.GetMirrorNode(synth.GetFinalStates()[0]), merged.GetMirrorNode(synth.M_Initial),
            GraphType.GT_DIRECTED);

        //7.Return result
        return tempFA;
    }

    internal FA SynthesizeOneOrNone(FA synth){
        FA tempFA = new FA();
        //2.Merge graph
        CGraph.CMergeGraphOperation merged = tempFA.Merge(synth);
        merged.MergeGraphInfo(synth, GraphElementType.ET_EDGE, FA.m_TRANSITIONSKEY);


        CGraphNode il = tempFA.CreateGraphNode();
        tempFA.AddGraphEdge(il, merged.GetMirrorNode(synth.M_Initial), GraphType.GT_DIRECTED);
        CGraphNode fl = tempFA.CreateGraphNode();
        tempFA.AddGraphEdge(merged.GetMirrorNode(synth.GetFinalStates()[0]), fl, GraphType.GT_DIRECTED);

        //4.Create the initial and the final node
        tempFA.M_Initial = il;
        tempFA.SetFinalState(fl);
        tempFA.UpdateAlphabet();

        tempFA.AddGraphEdge(il, fl, GraphType.GT_DIRECTED);

        //7.Return result
        return tempFA;
    }
}
internal class CThompsonConcatenationTemplate : CThompsonTemplates
{
    internal FA Synthesize(FA l, FA r){
        FA tempFA = new FA();
        //2.Merge left graph
        CGraph.CMergeGraphOperation lmerge = tempFA.Merge(l);
        lmerge.MergeGraphInfo(l, GraphElementType.ET_EDGE, FA.m_TRANSITIONSKEY);

        CGraphNode il = lmerge.GetMirrorNode(l.M_Initial);
        CGraphNode fl = lmerge.GetMirrorNode(l.GetFinalStates()[0]);

        //3.Merge right graph
        CGraph.CMergeGraphOperation rmerge = tempFA.Merge(r);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_EDGE, FA.m_TRANSITIONSKEY);

        CGraphNode ir = rmerge.GetMirrorNode(r.M_Initial);
        CGraphNode fr = rmerge.GetMirrorNode(r.GetFinalStates()[0]);

        //4.Create the initial and the final node
        tempFA.M_Initial = il;
        tempFA.SetFinalState(fr);

        tempFA.AddGraphEdge(fl, ir, GraphType.GT_DIRECTED);

        //7.Return result
        return tempFA;

    }
}
internal class CThompsonAlternationTemplate : CThompsonTemplates
{
    internal FA Sythesize(FA l, FA r)
    {
        //1.Create FA
        FA templateFA = new FA();


        //2.Merge left graph
        CGraph.CMergeGraphOperation lmerge =templateFA.Merge(l);
        lmerge.MergeGraphInfo(l,GraphElementType.ET_EDGE,FA.m_TRANSITIONSKEY);
        CGraphNode il = lmerge.GetMirrorNode(l.M_Initial);
        CGraphNode fl = lmerge.GetMirrorNode(l.GetFinalStates()[0]);

        //3.Merge right graph
        CGraph.CMergeGraphOperation rmerge = templateFA.Merge(r);
        rmerge.MergeGraphInfo(r, GraphElementType.ET_EDGE, FA.m_TRANSITIONSKEY);

        CGraphNode ir = rmerge.GetMirrorNode(r.M_Initial);
        CGraphNode fr = rmerge.GetMirrorNode(r.GetFinalStates()[0]);

        //4.Create the initial and the final node
        CGraphNode FAinit = templateFA.CreateGraphNode();
        CGraphNode FAfinal = templateFA.CreateGraphNode();
        templateFA.M_Initial = FAinit;
        templateFA.SetFinalState(FAfinal);
        templateFA.UpdateAlphabet();


        templateFA.AddGraphEdge(FAinit, il, GraphType.GT_DIRECTED);
        templateFA.AddGraphEdge(FAinit, ir, GraphType.GT_DIRECTED);
        templateFA.AddGraphEdge(fr, FAfinal, GraphType.GT_DIRECTED);
        templateFA.AddGraphEdge(fl, FAfinal, GraphType.GT_DIRECTED);

        //7.Return result
        return templateFA;
    }
}


