using System.Collections.Generic;
using UnityEngine;

public class BearStateMachine
{
    private Bear _bear;
    public Dictionary<EBearState, IBearState> StateDictionary { get; private set; }

    private IBearState _currentState;
    public IBearState CurrentState => _currentState;

    public BearStateMachine(Bear bear, Dictionary<EBearState, IBearState> stateDictionary)
    {
        _bear = bear;
        StateDictionary = stateDictionary;
        ChangeState(EBearState.Patrol);
    }

    public void ChangeState(EBearState newState)
    {
        if (StateDictionary.TryGetValue(newState, out IBearState state))
        {
            if (_currentState != null && _currentState != state)
            {
                _currentState.Exit(_bear);
            }
            _currentState = state;
            _currentState.Enter(_bear);
        }
    }

    public IBearState GetState(EBearState state)
    {
        return StateDictionary[state];
    }

    public void Update()
    {
        _currentState?.Execute(_bear);
    }

    public void ModifyState(EBearState which, IBearState to)
    {
        StateDictionary[which] = to;
    }
}