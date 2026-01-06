using UnityEngine;

public class PlayerIdleState : IState
{
    private PlayerPresenter _presenter;

    public PlayerIdleState(PlayerPresenter presenter)
    {
        _presenter = presenter;
    }

    public void Enter()
    {
        _presenter.View.PlayRunAnim(false);
        _presenter.View.SetVelocity(Vector3.zero); 
        Debug.Log("Entering Idle State");
    }

    public void Tick()
    {
        if (_presenter.InputService.JoystickInput != Vector2.zero)
        {
            _presenter.StateMachine.ChangeState(_presenter.MoveState);
            return;
        }
        
        var target = _presenter.EnemyDetector.ClosestEnemy;
        if (target != null)
        {
            Vector3 lookDir = (target.transform.position - _presenter.transform.position).normalized;
            lookDir.y = 0;
            _presenter.View.RotateTowards(lookDir);
        }
    }

    public void Exit() { }
}