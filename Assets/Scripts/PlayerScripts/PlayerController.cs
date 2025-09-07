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
    //OldFSM _oldFsm;
    FSM<PlayerStates> _fsm;
    IMove _move;
    ISpin _spin;
    private PlayerStates currentState = PlayerStates.Idle;


    void Start()
    {
        _move = GetComponent<IMove>();
        _spin = GetComponent<ISpin>();

        /*_oldFsm = new();
        _oldFsm.AddState(PlayerStates.Idle, new OldIdlePlayerState(_move));
        _oldFsm.AddState(PlayerStates.Moving, new OldMovePlayerState(_move));
        _oldFsm.AddState(PlayerStates.Spining, new OldSpinPlayerState(_spin));

        _oldFsm.SetStartState(PlayerStates.Idle);*/

        SetFSM();
    }

    private void SetFSM()
    {
        _fsm = new();
        var idle = new IdlePlayerState(_fsm, _move);
        var move = new MovePlayerState(_fsm, _move);
        var spin = new SpinPlayerState(_fsm, _spin);

        idle.AddTransition(PlayerStates.Moving, move);
        idle.AddTransition(PlayerStates.Spining, spin);

        move.AddTransition(PlayerStates.Idle, idle);
        move.AddTransition(PlayerStates.Spining, spin);

        spin.AddTransition(PlayerStates.Idle, idle);

        _fsm.SetInitialState(idle);
    }

    void Update()
    {
        /*switch (currentState)
        {
            case PlayerStates.Idle:
                IdleState();
                break;
            case PlayerStates.Moving:
                MovingState();
                break;
            case PlayerStates.Spining:
                SpiningState();
                break;
            default:
                break;
        }*/

        /*if (_spin.IsDetectable)
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            Vector3 dir = new Vector3(h, 0, v);
            _move.Move(dir.normalized);
            if (h != 0 || v != 0) _move.Look(dir);
        }
        if (Input.GetKeyDown(KeyCode.Space)) _spin.Spin();*/

        /*_oldFsm.OnUpdate();

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
            _oldFsm.SetState(PlayerStates.Spining);
        else if (h != 0 || v != 0)
        {
            _oldFsm.SetState(PlayerStates.Moving);
        }
        else
            _oldFsm.SetState(PlayerStates.Idle);*/

        _fsm.OnUpdate();
    }

    private void ChageState(PlayerStates newState)
    {
        if(currentState == PlayerStates.Spining)
            _spin.Spin();

        currentState = newState;

        if (currentState == PlayerStates.Spining)
            _spin.Spin();

        if (currentState == PlayerStates.Idle)
            _move.Move(Vector3.zero);
    }

    private void IdleState()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            ChageState(PlayerStates.Moving);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
            ChageState(PlayerStates.Spining);
    }
    private void MovingState()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            Vector3 dir = new Vector3(h, 0, v);
            _move.Move(dir.normalized);
            _move.Look(dir);
        }
        else
            ChageState(PlayerStates.Idle);
            
        if (Input.GetKeyDown(KeyCode.Space))
            ChageState(PlayerStates.Spining);
    }
    private void SpiningState()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChageState(PlayerStates.Idle);

    }
}
