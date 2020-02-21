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

        public int M_StreamPointer {
            get => m_streamPointer;
            set => m_streamPointer = value;
        }

        public bool M_Match {
            get => m_match;
            set => m_match = value;
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
            m_resource = DeSerializeEDUFLEXOutput("EDUFLEX.out");
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
        }

        /// <summary>
        /// The lexer continue() method scans the whole input stream to
        /// match as many strings it can
        /// </summary>
        public override void Continue() {
            ResetState();
            while (!m_inputCharStream.M_EOF && m_currentState.M_Match) {
                Step();
            }

            if (!m_currentState.M_Match) {
                Console.WriteLine("Lexical Error !!!");
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
                
                // 2. Initiate continuation of state machine
                pair.Value.Continue();
                
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
                m_currentState.M_Match = false;
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
