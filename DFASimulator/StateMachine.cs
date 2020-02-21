using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DFASimulator {


    public interface IState {
        string ToString();
    }

    /// <summary>
    /// Represents mandatory functionality of a state machine
    /// 1. It has a method (UpdateState) that calculates the new state based on an event
    /// 2. It has a method (Go) that initiates process of changing the current and
    /// nested state machines state
    /// </summary>
    public interface IStateMachine {
        IState M_State { get; }
        void Continue();
        void ResetState();
        void Step();
    }

    /// <summary>
    /// A state machine where its state depends on other nested state machines
    /// The class has it own state that is calculated using the resource and the
    /// state of the nested state machines 
    /// </summary>
    /// <typeparam name="TStateModel">State machine resource on which the states apply</typeparam>
    /// <typeparam name="TEvent">Type of event causing the state machine to change state</typeparam>
    public abstract class StateMachine<TStateModel> : IStateMachine {
        // Model on which the state machine decides its state
        protected TStateModel m_resource;
        // The state of current state machine
        protected readonly IState m_currentState;
        // List of nested state machines
        protected List<IStateMachine> m_currentSuperState=null;
        // Parent state machine
        protected IStateMachine m_parent;

        public IState M_State => m_currentState;

        /// <summary>
        /// State machine constructor
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="parent"></param>
        public StateMachine(TStateModel resource, IState stateDescriptionObject, IStateMachine parent = null) {
            m_resource = resource;
            m_parent = parent;
            m_currentState = stateDescriptionObject;
        }

        public virtual void AddStateMachine(IStateMachine st) {
            if (m_currentSuperState == null) {
                m_currentSuperState = new List<IStateMachine>();
            }
            m_currentSuperState.Add(st);
        }
        
        /// <summary>
        /// This method continues the state machine from the last stop point. This method
        /// assume that state machine executes passing through stop points and resumes
        /// its operation maybe from a new state 
        /// </summary>
        public abstract void Continue();

        /// <summary>
        /// This method realizes the mechanism to calculate the next state
        /// machine state based on the input event. It is realized in the
        /// subclasses where the algorithm to calculate the algorithm
        /// is known. This method may take into account the states of
        /// any nested state machines in order to calculate the new state
        /// of the current state machine
        /// </summary>
        /// <param name="event_"></param>
        public abstract void Step();

        /// <summary>
        /// This method initializes the state machine state
        /// </summary>
        public abstract void ResetState();
        
    }
}
