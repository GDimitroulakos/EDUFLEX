using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using Parser;
using Parser.UOPCore;
using SeekableStreamReader;

namespace DFASimulator {

    public interface IEDUFlexStream {
        int NextChar();

        int GetChar(int index);

        int SeekPosition(int index);

        int Size { get; }

        string SourceName { get; }
    }

    /// <summary>
    /// This class offers buffering for the EduFlex input file. StreamReader class is not
    /// seekable by design. Although it is possible to random access data in a streamreader through
    /// its embedding stream interface it inquires significant overhead .
    /// We need support efficient seeking operations on character data in our EduFlexStream class.
    /// StreamReader is the easiest way to access text data from a stream however it doen't support
    /// seeking. Thus to efficiently
    /// </summary>
    public class EDUFlexStream : BufferedStreamTextReader {
        public EDUFlexStream(Stream mIstream, int bufferSize = 4096,
                             Encoding mStreamEncoding = null) : base(mIstream, bufferSize, mStreamEncoding) {

        }

    }

    public partial class DFASimulator {
        private EDUFlexStream m_inputCharStream;
        private FA m_dfa;
        Stack<CGraphNode> m_stateStack = new Stack<CGraphNode>();
        CGraphNode m_currentState;
        StringBuilder m_lexeme = new StringBuilder();
        private bool m_deadend;

        public DFASimulator(FA mDfa, EDUFlexStream inputStream) {
            m_dfa = mDfa;
            m_inputCharStream = inputStream;
        }

        public bool IsFinalState(CGraphNode state) {
            return m_dfa.IsFinalState(state);
        }

        public int yylex() {
            CGraphNode nextState;

            // Initialization
            int nextChar = -1;
            m_lexeme.Clear();
            m_currentState = m_dfa.M_Initial;


            while (!m_deadend && nextChar!=0) {
                if (IsFinalState(m_currentState)) {
                    m_stateStack.Clear();
                }
                m_stateStack.Push(m_currentState);
                nextChar = m_inputCharStream.NextChar();
                m_lexeme.Append((char)nextChar);
                m_currentState = m_dfa.GetTransitionTarget(m_currentState, nextChar);
            }

            while (!IsFinalState(m_currentState)) {
                m_currentState = m_stateStack.Pop();
                m_lexeme.Remove(m_lexeme.Length - 1, 1);
                m_inputCharStream.GoBackwards();
            }

            if (IsFinalState(m_currentState)) {
                return 1;
            }
            else {
                return -1;
            }
        }
    }



    class Program {
        static void Main(string[] args) {
            EDUFlexStream istream = new EDUFlexStream(new FileStream("source.txt",FileMode.Open));

            Facade.VerifyRegExp(args);

            DFASimulator dfaSimulator = new DFASimulator(Facade.MsMinDfa,istream);
            dfaSimulator.yylex();

        }
    }
}
