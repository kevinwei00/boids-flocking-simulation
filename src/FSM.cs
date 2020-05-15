using UnityEngine;

public interface FSM<T> {
    State<T> CurrentState { get; }
    State<T> PreviousState { get; }
    void GoToState(State<T> state);
    void StartingState(State<T> state);
    void CreateStates();
}

public abstract class State<T> {

    protected readonly T myFSM;

    public State(T fsm) {
        if (fsm is FSM<T>) {
            myFSM = fsm;
        }
        else {
            throw new System.Exception(fsm.GetType() + " is not a FSM!");
        }
    }

    public State() { }

    public virtual void OnStateEntry() { }
    public virtual void OnStateExit() { }

    // Unity specific
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnTriggerEnter(Collider2D c) { }
    public virtual void OnTriggerExit(Collider2D c) { }
    public virtual void OnTriggerStay(Collider2D c) { }
    public virtual void OnCollisionEnter(Collision2D c) { }
}

public class UnityFSM : MonoBehaviour, FSM<UnityFSM> {

    // Debug
    public string _CurrentState;
    public string _PreviousState;

    public State<UnityFSM> CurrentState { get; private set; }
    public State<UnityFSM> PreviousState { get; private set; }

    public virtual void GoToState(State<UnityFSM> state) {
        PreviousState = CurrentState;
        PreviousState.OnStateExit();

        CurrentState = state;
        CurrentState.OnStateEntry();

        _CurrentState = CurrentState.ToString();
        _PreviousState = PreviousState.ToString();
    }

    public virtual void StartingState(State<UnityFSM> state) {
        CurrentState = PreviousState = state;
        state.OnStateEntry();

        _CurrentState = CurrentState.ToString();
        _PreviousState = PreviousState.ToString();
    }

    public virtual void CreateStates() { }
}

public class UnityFSMState<T> : State<UnityFSM> {

    protected new readonly T myFSM;

    public UnityFSMState(T unityFSM) {
        if (unityFSM is UnityFSM) {
            myFSM = unityFSM;
        }
        else {
            throw new System.Exception(unityFSM.GetType() + " is not a UnityFSM!");
        }
    }
}
