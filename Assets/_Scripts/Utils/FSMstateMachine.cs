using System.Collections.Generic;

public enum FSMEVENT { START, UPDATE, EXIT };

public class FSMstateMachine<T>
{
    public delegate void function(FSMEVENT _event);

    Dictionary<T, function> states = new Dictionary<T, function>();
    function currentState;

    public void AddState(function _callback, T _stateID)
    {
        if(states.ContainsKey(_stateID)) //Security is not already add
        {
            UnityEngine.Debug.LogError("Ya existe estado: " + _stateID);
            return;
        }

        //Add the new state
        states.Add(_stateID, _callback);
    }

    public void ChangeState(T _stateID)
    {
        /*if (currentState != null)
            currentState.Invoke(FSMEVENT.EXIT);*/

        currentState = states[_stateID]; //Take current
        currentState.Invoke(FSMEVENT.START);
    }

    public void Update()
    {
        /*if (currentState != null)*/
        currentState.Invoke(FSMEVENT.UPDATE);
    }
}
