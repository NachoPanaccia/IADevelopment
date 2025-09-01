using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    Idle,
    Moving,
    Spining
}

public class PlayerController : MonoBehaviour
{
    FSM _fsm;
    IMove _move;
    ISpin _spin;

    void Start()
    {
        _move = GetComponent<IMove>();
        _spin = GetComponent<ISpin>();
        _fsm = new();

        _fsm.AddState(PlayerStates.Idle, new IdlePlayerState(_move));
        _fsm.AddState(PlayerStates.Moving, new MovePlayerState(_move));
        _fsm.AddState(PlayerStates.Spining, new SpinPlayerState(_spin));

        _fsm.SetStartState(PlayerStates.Idle);
    }
    void Update()
    {
        _fsm.OnUpdate();

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            _fsm.SetState(PlayerStates.Spining);
        else if (h != 0 || v != 0)
        {
            _fsm.SetState(PlayerStates.Moving);
        }
        else
            _fsm.SetState(PlayerStates.Idle);

    }
}
