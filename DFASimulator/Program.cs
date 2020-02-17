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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


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

    /// <summary>
    /// Represents the current state of a state machine assigned the 
    /// objective to recognize a string satisfying a specific regular expression
    /// </summary>
    public class DFAState {
        private Stack<CGraphNode> m_stateStack=new Stack<CGraphNode>();
        // Current state machine state that is a DFA state
        private CGraphNode m_currentState;
        private bool m_deadend;
        private bool m_match;
        private bool m_EOFreached;
        // Current stream position
        private int m_streamPosition;
        // Current buffered string
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

    public partial class DFASimulatorFinite {
        private Dictionary<uint, RERecord> m_reRecords;
        private Dictionary<uint, DFAState> m_dfaMultiStates = new Dictionary<uint, DFAState>();

        private int m_streamPointer = 0;
        private EDUFlexStream m_inputCharStream;

        public DFASimulatorFinite(Dictionary<uint, RERecord> reRecords, EDUFlexStream inputCharStream) {
            m_reRecords = DeSerializeEDUFLEXOutput("EDUFLEX.out");
            m_inputCharStream = inputCharStream;

            /*foreach (KeyValuePair<uint, RERecord> pair in reRecords) {
                m_dfaMultiStates[pair.Key] = new DFAState();
                ResetDFASimulatorState(pair.Key);
            }*/
        }

        public Dictionary<uint, RERecord> DeSerializeEDUFLEXOutput(string filename) {

            BinaryFormatter res = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                return (Dictionary<uint, RERecord>)(res.Deserialize(stream));
            }
        }

        public int yylex() {
            uint? renum;
            int nextChar = 0;
            m_streamPointer = 0;
            while (!m_inputCharStream.M_EOF) {

            }

            return 0;
        }
    }

    public partial class DFASimulatorMulti {
        /// <summary>
        /// Holds the DFAs and regular expression related information from the parser
        /// </summary>
        private Dictionary<uint, RERecord> m_reRecords;

        private Dictionary<uint, DFAState> m_dfaMultiStates=new Dictionary<uint, DFAState>();

        private int m_streamPointer=0;

        private EDUFlexStream m_inputCharStream;
        
        public DFASimulatorMulti(Dictionary<uint, RERecord> reRecords, EDUFlexStream inputStream) {
            m_reRecords = DeSerializeEDUFLEXOutput("EDUFLEX.out");
            m_inputCharStream = inputStream;
                      
            foreach (KeyValuePair<uint, RERecord> pair in reRecords) {
                m_dfaMultiStates[pair.Key] = new DFAState();
                ResetDFASimulatorState(pair.Key);
            }
        }

        public Dictionary<uint, RERecord> DeSerializeEDUFLEXOutput(string filename) {

            BinaryFormatter res = new BinaryFormatter();
            using ( Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                return (Dictionary<uint, RERecord>)(res.Deserialize(stream));
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
                    
                    // 1. Initialize stream start point to be the point where
                    // we left from the previous call of yylex()
                    m_inputCharStream.SeekChar(m_streamPointer);

                    // 1a. Initialize DFAState for current DFA Simulator
                    nextChar = 0;

                    // Reset DFA Simulator state means: 
                    // 1. Current state is the initial DFA state 
                    // 2. Stream position is the position of the next character to be retrieved
                    // 3. Clear the current buffered string
                    // 4. Clear the stack
                    // 5. Reset Deadend flag that indicates potential lexical error
                    // 6. Reset the M_Match flag that indicates a match
                    ResetDFASimulatorState(pair.Key, m_streamPointer);

                    // draw characters from the stream until a deadend or the EOF is reached
                    while (!dfastate.M_Deadend && nextChar != -1) {
                        // If the current state is Final clear the stack to trace state
                        // from the current final(accepted) state and beyong. That is, we ignore 
                        // any accepted states before the current accept state
                        if (IsFinalState(reRecord.M_MinDfa, dfastate.M_CurrentState)) {
                            dfastate.M_StateStack.Clear();
                        }
                        dfastate.M_StateStack.Push(dfastate.M_CurrentState);
                        // Get next character from stream and advance the stream position
                        nextChar = m_inputCharStream.NextChar();
                        // Append the character to the current buffered string
                        dfastate.M_Lexeme.Append((char) nextChar);
                        // Calculate the new simulator state
                        dfastate.M_CurrentState =
                            reRecord.M_MinDfa.GetTransitionTarget(dfastate.M_CurrentState, nextChar);
                        // If there is no transition for the given character indicate a dead end state
                        // By setting this flag the while loop exits without updating the simulator 
                        // state meaning that the current simulator state corresponds to the last valid 
                        // state
                        if (dfastate.M_CurrentState == null) {
                            dfastate.M_Deadend = true;
                        }
                    }

                    // BACKTRACKING TO THE MOST RECENT ACCEPTED STATE
                    // If previous loop ended in a non-accepted state the current loop 
                    // goes backward until the most recent accepted state
                    while (!IsFinalState(reRecord.M_MinDfa, dfastate.M_CurrentState) &&
                           dfastate.M_StateStack.Count != 0) {
                        // Remove the last state from the stack
                        dfastate.M_CurrentState = dfastate.M_StateStack.Pop();
                        // Remove the last character from the buffered stream
                        dfastate.M_Lexeme.Remove(dfastate.M_Lexeme.Length - 1, 1);
                        // Go back one character on the input stream
                        m_inputCharStream.GoBackwards();
                    }

                    // DETECT MATCH OR MISMATCH FOR THE CURRENT REGULAR EXPRESSION
                    // If after backtracking the simulator is not in an accepted state 
                    // set the Deadend flag that indicates a mismatch for the current 
                    // regular expression. Otherwise set the Match flag indicating a
                    // match of the current buffered string with the current regular
                    // expression
                    if (!IsFinalState(reRecord.M_MinDfa, dfastate.M_CurrentState)) {
                        dfastate.M_Match = false;
                        dfastate.M_Deadend = true;
                    }
                    else {
                        dfastate.M_Match = true;
                    } 
                    // Loop until all regular expressions have been studied
                }

                // DETECT MATCH OR LEXICAL ERROR
                // Find the regular expression that is attributed the match according the 
                // regular expression language policies
                renum = DetectMatchRE();
                
                // If a match is found, execute the code corresponding to the regular expression 
                // that is attributed the match. Otherwise if a match is not found report a 
                // lexical error to the user.
                if (renum != null) {
                    Console.WriteLine("Match {{{0}}} Detected with RE {1}", m_dfaMultiStates[(uint)renum].M_Lexeme, renum);
                    nextChar = 0;
                    if (m_inputCharStream.SeekChar(m_streamPointer + m_dfaMultiStates[(uint)renum].M_Lexeme.Length) != -1) {
                        m_streamPointer = m_streamPointer + m_dfaMultiStates[(uint)renum].M_Lexeme.Length;
                    }
                } else {
                    Console.WriteLine("Lexical Error !!!");
                    break;
                }
                /*
                 * switch (renum ){
                 *  case 1: 
                 *      @1
                 *  break;
                 *  case 2:
                 *      @2
                 *  break;
                 *  ...
                 * }
                 * 
                 * */
            }
            
            return 0;
        }

        // The DetectMatchRE() method enforces the policies 
        // 1. Among the matches select the longer match ( most characters ) 
        // 2. Among matches with the same length select the match from the 
        // regular expression that appears first in the input regular expression file
        public uint? DetectMatchRE() {
            int matchLength = -1;
            uint? renum=null;
            foreach (KeyValuePair<uint, DFAState> valuePair in m_dfaMultiStates) {
                // maximum length policy
                if (valuePair.Value.M_Match && matchLength < valuePair.Value.M_Lexeme.Length) {
                    matchLength = valuePair.Value.M_Lexeme.Length;
                    renum = valuePair.Key;
                }
                // regular expression spatial priority policy
                else if (valuePair.Value.M_Match && matchLength == valuePair.Value.M_Lexeme.Length) {
                    if  ( valuePair.Key < renum) {
                        matchLength = valuePair.Value.M_Lexeme.Length;
                        renum = valuePair.Key;
                    }
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

            if (Facade.GetOperationModeCode()) {
                DFASimulatorMulti dfaSimulator = new DFASimulatorMulti(Facade.M_ReRecords, istream);
                dfaSimulator.yylex();
            }
            else {
                DFASimulator dfaSimulator = new DFASimulator(Facade.M_ReRecords[0].M_MinDfa, istream);
                dfaSimulator.yylex();
            }



        }
    }
}
