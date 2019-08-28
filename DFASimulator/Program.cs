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

    public class DFAState {
        private Stack<CGraphNode> m_stateStack=new Stack<CGraphNode>();
        private CGraphNode m_currentState;
        private bool m_deadend;
        private bool m_match;
        private bool m_EOFreached;
        private int m_streamPosition;
        private StringBuilder m_lexeme=new StringBuilder();

        public Stack<CGraphNode> M_StateStack {
            get => m_stateStack;
            set => m_stateStack = value;
        }

        public CGraphNode M_CurrentState {
            get => m_currentState;
            set => m_currentState = value;
        }

        public bool M_Deadend {
            get => m_deadend;
            set => m_deadend = value;
        }

        public bool M_Match {
            get => m_match;
            set => m_match = value;
        }

        public int M_StreamPosition {
            get => m_streamPosition;
            set => m_streamPosition = value;
        }

        public StringBuilder M_Lexeme {
            get => m_lexeme;
            set => m_lexeme = value;
        }

        public bool M_EOF {
            get => m_EOFreached;
            set => m_EOFreached = value;
        }

    }

    public partial class DFASimulatorMulti {
        
        private Dictionary<uint, RERecord> m_reRecords;
        private Dictionary<uint, DFAState> m_dfaMultiStates=new Dictionary<uint, DFAState>();

        private int m_streamPointer=0;

        private EDUFlexStream m_inputCharStream;

        StringBuilder m_lexeme = new StringBuilder();


        public DFASimulatorMulti(Dictionary<uint, RERecord> reRecords, EDUFlexStream inputStream) {
            m_reRecords = reRecords;
            m_inputCharStream = inputStream;

            foreach (KeyValuePair<uint, RERecord> pair in reRecords) {
                m_dfaMultiStates[pair.Key] = new DFAState();
                ResetDFASimulatorState(pair.Key);
            }
        }


        public int yylex() {
            uint? renum;
            int nextChar = 0;
            m_streamPointer = 0;
            while (! m_inputCharStream.M_EOF ) {

                foreach (KeyValuePair<uint, DFAState> pair in m_dfaMultiStates) {
                    // For each DFA Simulator applied to each of the regular expressions
                    // retrieve characters until a Dead end state or EOF is reached 

                    DFAState dfastate = pair.Value;
                    RERecord reRecord = m_reRecords[pair.Key];
                    
                    // 1. Initialize start point 
                    m_inputCharStream.SeekChar(m_streamPointer);

                    // 1a. Initialize DFAState for current DFA Simulator
                    nextChar = 0;
                    ResetDFASimulatorState(pair.Key, m_streamPointer);

                    while (!dfastate.M_Deadend && nextChar != -1) {
                        if (IsFinalState(reRecord.M_MinDfa, dfastate.M_CurrentState)) {
                            dfastate.M_StateStack.Clear();
                        }

                        dfastate.M_StateStack.Push(dfastate.M_CurrentState);
                        nextChar = m_inputCharStream.NextChar();
                        dfastate.M_Lexeme.Append((char) nextChar);
                        dfastate.M_CurrentState =
                            reRecord.M_MinDfa.GetTransitionTarget(dfastate.M_CurrentState, nextChar);
                        if (dfastate.M_CurrentState == null) {
                            dfastate.M_Deadend = true;
                        }
                    }

                    while (!IsFinalState(reRecord.M_MinDfa, dfastate.M_CurrentState) &&
                           dfastate.M_StateStack.Count != 0) {
                        dfastate.M_CurrentState = dfastate.M_StateStack.Pop();
                        dfastate.M_Lexeme.Remove(dfastate.M_Lexeme.Length - 1, 1);
                        m_inputCharStream.GoBackwards();
                    }

                    if (!IsFinalState(reRecord.M_MinDfa, dfastate.M_CurrentState)) {
                        dfastate.M_Match = false;
                        dfastate.M_Deadend = true;
                    }
                    else {
                        dfastate.M_Match = true;
                    }                    
                }

                renum = DetectMatchRE();
                if (renum != null) {
                    Console.WriteLine("Match Detected with RE {0}",renum);
                    nextChar = 0;
                    if (m_inputCharStream.SeekChar(m_streamPointer + m_dfaMultiStates[(uint)renum].M_Lexeme.Length) != -1) {
                        m_streamPointer = m_streamPointer + m_dfaMultiStates[(uint)renum].M_Lexeme.Length;
                    }

                }
                else {
                    Console.WriteLine("Lexical Error !!!");
                }



            }
            
            return 0;
        }

        public uint? DetectMatchRE() {
            int matchLength = -1;
            uint? renum=null;
            foreach (KeyValuePair<uint, DFAState> valuePair in m_dfaMultiStates) {
                if (valuePair.Value.M_Match && matchLength < valuePair.Value.M_Lexeme.Length) {
                    matchLength = valuePair.Value.M_Lexeme.Length;
                    renum = valuePair.Key;
                }
            }
            return renum;
        }


        public bool IsFinalState(FA dfa, CGraphNode state) {
            return dfa.IsFinalState(state);
        }

        private void ResetDFASimulatorState(uint DFAKey,int streamPosition=0) {

            DFAState dfastate = m_dfaMultiStates[DFAKey];
            RERecord reRecord = m_reRecords[DFAKey];

            dfastate.M_CurrentState = reRecord.M_MinDfa.M_Initial;
            dfastate.M_StreamPosition = streamPosition;
            dfastate.M_Lexeme.Clear();
            dfastate.M_StateStack.Clear();
            dfastate.M_Deadend = false;
            dfastate.M_Match = false;

        }

        private bool DeadEnd() {
            foreach (KeyValuePair<uint, DFAState> dfaMultiState in m_dfaMultiStates) {
                if (!dfaMultiState.Value.M_Deadend) {
                    return false;
                }
            }
            return true;
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

        public void SeekInputStream(int pos) {
            
        }

        public int yylex() {
            CGraphNode nextState;

            // Initialization
            int nextChar = 0;
            m_lexeme.Clear();
            m_currentState = m_dfa.M_Initial;
            m_deadend = false;

            while (!m_deadend && nextChar!=-1) {
                if (IsFinalState(m_currentState)) {
                    m_stateStack.Clear();
                }
                m_stateStack.Push(m_currentState);
                nextChar = m_inputCharStream.NextChar();
                m_lexeme.Append((char)nextChar);
                m_currentState = m_dfa.GetTransitionTarget(m_currentState, nextChar);
                if (m_currentState == null) {
                    m_deadend = true;
                }
            }

            while (!IsFinalState(m_currentState) &&
                   m_stateStack.Count!=0) {
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

            DFASimulatorMulti dfaSimulator = new DFASimulatorMulti(Facade.M_ReRecords,istream);
            dfaSimulator.yylex();

        }
    }
}
