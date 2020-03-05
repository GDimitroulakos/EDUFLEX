using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using Parser.UOPCore;

namespace DFASimulator {
    /// <summary>
    /// Represents the current state of a state machine assigned the 
    /// objective to recognize a string satisfying a specific regular expression
    /// </summary>
    public class DFAStateSingleton : IState {
        private Stack<CGraphNode> m_stateStack = new Stack<CGraphNode>();
        // Current state machine state that is a DFA state
        private CGraphNode m_currentState;
        private bool m_deadend;
        private bool m_match;
        private bool m_EOFreached;
        // Current buffered string
        private StringBuilder m_lexeme = new StringBuilder();
        // The start character of an unrecognized lexeme. 
        private int m_errorPrefix;
        private int m_streamPointerLine;
        private int m_streamPointerColumn;
        
        private static DFAStateSingleton ms_instance;

        public static DFAStateSingleton GetInstance() {
            DFAStateSingleton temp;
            if (ms_instance != null) {
                temp = ms_instance;
                ms_instance = null;
                return temp;
            }
            return ms_instance = new DFAStateSingleton();
        }

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
        
        public StringBuilder M_Lexeme {
            get => m_lexeme;
        }

        public int MStreamPointerLine {
            get => m_streamPointerLine;
            set => m_streamPointerLine = value;
        }

        public int MStreamPointerColumn {
            get => m_streamPointerColumn;
            set => m_streamPointerColumn = value;
        }

        public int MErrorPrefix {
            set => m_errorPrefix = value;
            get => m_errorPrefix;
        }

        public bool M_EOF {
            get => m_EOFreached;
            set => m_EOFreached = value;
        }

        public override string ToString() {
            return base.ToString();
        }
    }
    public class DFASimulator : StateMachine<FA> {
        /// <summary>
        /// This field points to an object that describes the current state of state machine
        /// </summary>
        private readonly DFAStateSingleton m_currentState;
        /// <summary>
        /// Holds the starting point in the stream in before a yylex() method call
        /// </summary>
        private int m_streamPointer = 0;

        int m_nextChar = 0;
        /// <summary>
        /// The input stream
        /// </summary>
        private EDUFlexStream m_inputCharStream;

        public DFAStateSingleton M_CurrentState => m_currentState;

        public int M_StreamPointer {
            get => m_streamPointer;
            set => m_streamPointer = value;
        }

        public DFASimulator(FA resource, EDUFlexStream istream, IStateMachine parent = null) : base(resource, DFAStateSingleton.GetInstance(), parent) {
            m_currentState = DFAStateSingleton.GetInstance();
            m_stateModel = resource;
            m_inputCharStream = istream;
            ResetState();
        }

        private void AdvanceStreamPosition(int c) {

        }

        private void RetreatStreamPosition(int c) {

        }

        /// <summary>
        /// The step() method of DFASimulator has the objective to pass through a series
        /// of states until it finds a valid lexeme or a deadend. 
        /// </summary>
        public override void Step() {
            ResetState();
            while (!m_currentState.M_Deadend && m_nextChar != -1) {
                // Get next character from stream and advance the stream position
                m_nextChar = m_inputCharStream.NextChar();
                m_currentState.M_EOF = m_nextChar == -1;

                // Update file position coordinates
                AdvanceStreamPosition(m_nextChar);
                // Append the character to the current buffered string
                m_currentState.M_Lexeme.Append((char) m_nextChar);
                // Record the last character in case of a lexical error
                m_currentState.MErrorPrefix = m_nextChar;
                // Calculate the new simulator state
                m_currentState.M_CurrentState =
                    m_stateModel.GetTransitionTarget(m_currentState.M_CurrentState, m_nextChar);

                // If the current state is Final clear the stack to trace state
                // from the current final(accepted) state and beyong. That is, we ignore 
                // any accepted states before the current accept state
                if (IsFinalState(m_stateModel, m_currentState.M_CurrentState)) {
                    m_currentState.M_StateStack.Clear();
                }

                // If there is no transition for the given character indicate a dead end state
                // By setting this flag the while loop exits without updating the simulator 
                // state meaning that the current simulator state corresponds to the last valid 
                // state
                if (m_currentState.M_CurrentState == null) {
                    m_currentState.M_Deadend = true;
                }
                else {
                    m_currentState.M_StateStack.Push(m_currentState.M_CurrentState);
                }
            }

            // BACKTRACKING TO THE MOST RECENT ACCEPTED STATE
            // If previous loop ended in a non-accepted state the current loop 
            // goes backward until the most recent accepted state
            while (!IsFinalState(m_stateModel, m_currentState.M_CurrentState) &&
                   m_currentState.M_StateStack.Count != 0) {
                // Remove the last state from the stack
                m_currentState.M_CurrentState = m_currentState.M_StateStack.Pop();
                // Remove the last character from the buffered stream
                m_currentState.M_Lexeme.Remove(m_currentState.M_Lexeme.Length - 1, 1);
                // Go back one character on the input stream
                // Update the m_nextChar variable to point to the next character 
                // after backtracking in order to provide valid input to any subsequent
                // condition
                m_nextChar = m_inputCharStream.GoBackwards();
                // Update file position coordinates
                RetreatStreamPosition(m_nextChar);
            }

            // DETECT MATCH OR MISMATCH FOR THE CURRENT REGULAR EXPRESSION
            // If after backtracking the simulator is not in an accepted state 
            // set the Deadend flag that indicates a mismatch for the current 
            // regular expression. Otherwise set the Match flag indicating a
            // match of the current buffered string with the current regular
            // expression
            if (!IsFinalState(m_stateModel, m_currentState.M_CurrentState)) {
                m_currentState.M_Match = false;
                m_currentState.M_Deadend = true;
            } else {
                m_currentState.M_Match = true;
            }
        }

        public override void Continue(Func<IState, object, bool> endCondition = null) {
            throw new NotImplementedException();
        }

        public override void Continue() {
            m_nextChar = 0;
            // 1. Reset DFA Simulator to start new matching procedure
            while ( m_nextChar != -1) {
                Step();
            }
        }

        public bool IsFinalState(FA dfa, CGraphNode state) {
            return dfa.IsFinalState(state);
        }

        public override void ResetState() {
            // Init Output
            m_currentState.M_Lexeme.Clear();
            m_currentState.MErrorPrefix=0;
            // Init State
            m_currentState.M_CurrentState = m_stateModel.M_Initial;
            // Init Stack
            m_currentState.M_StateStack.Clear();
            m_currentState.M_StateStack.Push(m_currentState.M_CurrentState);
            // Init Flags
            m_currentState.M_Deadend = false;
            m_currentState.M_Match = false;
        }
    }
}
