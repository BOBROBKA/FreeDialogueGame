using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    public FsmState CurrentState{get; private set;}

    private Dictionary<string, FsmState> _states = new Dictionary<string, FsmState>();

    public void AddState(FsmState state)
    {
        if (_states.ContainsKey(state.GetStateID()))
            throw new Exception($"Cannot add state with id {state.GetStateID()}, it is already added.");
        _states.Add(state.GetStateID(), state);

        if(_states.Count == 1) CurrentState = state;
    }

    public void SetState(string id)
    {
        if (CurrentState.GetStateID() == id) return;

        _states.TryGetValue(id, out FsmState newState);
        if (newState == null) throw new Exception($"State with id {id} is not in fsm");
        
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
    public void SetState<T>(string id, T data) where T:IFSMEnterStateData
    {
        if (CurrentState.GetStateID() == id) return;

        _states.TryGetValue(id, out FsmState newState);
        if (newState == null) throw new Exception($"State with id {id} is not in fsm");
        
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter(data);
    }

    public void Update()
    {
        CurrentState?.Update();
    }
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}