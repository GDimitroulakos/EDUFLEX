using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using Parser.UOPCore;

namespace DFASimulator {
    public class LexerState : IState {
        private bool m_match;
        private int m_streamPointer;
        private int m_streamPointerLine;
        private int m_streamPointerColumn;
        private int m_errorPrefix;
        private int m_errorLine;
        private int m_errorColumn;

        public int M_StreamPointer {
            get => m_streamPointer;
            set => m_streamPointer = value;
        }

        public bool M_Match {
            get => m_match;
            set => m_match = value;
        }
        public int MErrorPrefix {
            get => m_errorPrefix;
            set => m_errorPrefix = value;
        }
        public int MStreamPointerLine {
            get => m_streamPointerLine;
            set => m_streamPointerLine = value;
        }

        public int MStreamPointerColumn {
            get => m_streamPointerColumn;
            set => m_streamPointerColumn = value;
        }

        public int MErrorLine {
            get => m_errorLine;
            set => m_errorLine = value;
        }
        public int MErrorColumn {
            get => m_errorColumn;
            set => m_errorColumn = value;
        }

        private static LexerState ms_instance;

        public static LexerState GetInstance() {
            LexerState temp;
            if (ms_instance != null) {
                temp = ms_instance;
                ms_instance = null;
                return temp;
            }
            return ms_instance = new LexerState();
        }

        public override string ToString() {
            return base.ToString();
        }
    }

    public partial class LexerMulti : StateMachine<Dictionary<uint, RERecord>> {

        /// <summary>
        /// DFA Simulator Multi has one state machine per regular expression. The following
        /// dictionary is accesses using the line of declaration of the regular expression
        /// </summary>
        private Dictionary<uint, DFASimulator> m_dfaSimulators = new Dictionary<uint, DFASimulator>();
        
        /// <summary>
        /// The input stream
        /// </summary>
        private EDUFlexStream m_inputCharStream;

        private LexerState m_currentState;

        public LexerState M_CurrentState {
            get => m_currentState;
        }

        public LexerMulti(Dictionary<uint, RERecord> reRecords, EDUFlexStream inputStream) : base(reRecords, LexerState.GetInstance()) {
            m_stateModel = DeSerializeEDUFLEXOutput("EDUFLEX.out");
            m_inputCharStream = inputStream;
            m_currentState = LexerState.GetInstance();

            foreach (KeyValuePair<uint, RERecord> pair in reRecords) {
                m_dfaSimulators[pair.Key] = new DFASimulator(pair.Value.M_MinDfa, m_inputCharStream);
            }
        }

        public Dictionary<uint, RERecord> DeSerializeEDUFLEXOutput(string filename) {

            BinaryFormatter res = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                return (Dictionary<uint, RERecord>)(res.Deserialize(stream));
            }
        }

        public override void ResetState() {
            m_currentState.M_Match = true;      // only for the initialization
            m_currentState.M_StreamPointer = 0;
            m_currentState.MErrorPrefix = -1;
        }

        public override void Continue() {
            Continue((_, __) => {
                var edustream = (EDUFlexStream)__;
                LexerState state = (LexerState)_;
                return !edustream.M_EOF && state.M_Match;
            });
        }

        /// <summary>
        /// The lexer continue() method scans the whole input stream to
        /// match as many strings as long as the condition calculated
        /// by the input delegate evaluates to true. The input delegate
        /// takes the current state machine's state and the input provider
        /// as parameters
        /// </summary>
        public override void Continue(Func<IState, object, bool> cond=null) {
            ResetState();

            while (cond?.Invoke(m_currentState, m_inputCharStream) ?? false) {
                Step();
            }
            
            if (!m_currentState.M_Match) {
                Console.WriteLine("Lexical Error !!! Character {0} is not the prefix of any valid strings.",(char)m_currentState.MErrorPrefix);
            }
        }

        /// <summary>
        /// The lexer Step() method executes until it finds the next valid lexeme
        /// according to the whole set of regular expressions given in the input
        /// grammar file
        /// </summary>
        public override void Step() {
            uint? renum;

            foreach (KeyValuePair<uint, DFASimulator> pair in m_dfaSimulators) {
                // For each DFA Simulator applied to each of the regular expressions
                // retrieve characters until a Dead end state or EOF is reached 

                // 1. Initialize stream start point to be the point where
                // we left from the previous call of yylex()
                m_inputCharStream.SeekChar(m_currentState.M_StreamPointer);
                
                // 2. Initiate continuation of state machine for the current 
                // regular expression
                pair.Value.Step();
                
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
                Console.WriteLine("Match {{{0}}} Detected with RE {1}", m_dfaSimulators[(uint)renum].M_CurrentState.M_Lexeme, renum);
                
                // Check if the character after the discovered lexeme is the EOF. In the mean time 
                // the file pointer is place one position after the discovered lexeme.
                if (m_inputCharStream.SeekChar(m_currentState.M_StreamPointer + m_dfaSimulators[(uint)renum].M_CurrentState.M_Lexeme.Length) != -1) {
                    m_currentState.M_StreamPointer = m_currentState.M_StreamPointer + m_dfaSimulators[(uint)renum].M_CurrentState.M_Lexeme.Length;
                }
                m_currentState.M_Match = true;
            } else {
                Console.WriteLine("Lexical Error!!! Character {{{0}}} is not a prefix of a valid string", (char)m_currentState.MErrorPrefix);
                m_currentState.M_Match = false;
                m_currentState.M_StreamPointer++;
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

        // The DetectMatchRE() method enforces the policies 
        // 1. Among the matches select the longer match ( most characters ) 
        // 2. Among matches with the same length select the match from the 
        // regular expression that appears first in the input regular expression file
        public uint? DetectMatchRE() {
            int matchLength = -1;
            uint? renum = null;
            foreach (KeyValuePair<uint, DFASimulator> sim in m_dfaSimulators) {
                // maximum length policy
                if (sim.Value.M_CurrentState.M_Match && matchLength < sim.Value.M_CurrentState.M_Lexeme.Length) {
                    matchLength = sim.Value.M_CurrentState.M_Lexeme.Length;
                    renum = sim.Key;
                }
                // regular expression spatial priority policy
                else if (sim.Value.M_CurrentState.M_Match && matchLength == sim.Value.M_CurrentState.M_Lexeme.Length) {
                    if (sim.Key < renum) {
                        matchLength = sim.Value.M_CurrentState.M_Lexeme.Length;
                        renum = sim.Key;
                    }
                }
                else if (!sim.Value.M_CurrentState.M_Match &&
                          sim.Value.M_CurrentState.MErrorPrefix != -1 ) {
                    m_currentState.MErrorPrefix = sim.Value.M_CurrentState.MErrorPrefix;
                }
            }
            return renum;
        }

        /// <summary>
        /// Returns true if the given state of the specified DFA is accepted state
        /// </summary>
        /// <param name="dfa"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsFinalState(FA dfa, CGraphNode state) {
            return dfa.IsFinalState(state);
        }
    }
}
