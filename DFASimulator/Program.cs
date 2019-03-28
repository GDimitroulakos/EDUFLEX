using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using Parser.UOPCore;

namespace DFASimulator
{

    public class DFASimulator {
        private FA m_dfa;
        private bool m_deadend;

        public DFASimulator(FA mDfa) {
            m_dfa = mDfa;
        }

        public int yylex() {
            char nextChar=' ';  
            Stack<CGraphNode> m_stateStack = new Stack<CGraphNode>();
            CGraphNode currentState = m_dfa.M_Initial;
            CGraphNode nextState;
            StringBuilder lexeme = new StringBuilder();

            while ( !m_deadend ) {
                if (m_dfa.MFinal.Contains(currentState)) {
                    m_stateStack.Clear();
                }
                m_stateStack.Push(currentState);
                // nextChar = NextChar();
                lexeme.Append(nextChar);
                nextState = m_dfa.GetTransitionTarget(currentState, nextChar);
            }

            while (true) {
                
            }




            return 0;
        }
    }



    class Program{
        static void Main(string[] args)
        {
        }
    }
}
