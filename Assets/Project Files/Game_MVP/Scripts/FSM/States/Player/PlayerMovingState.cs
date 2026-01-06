using UnityEngine;

public class PlayerMovingState : IState
{
    private PlayerPresenter _presenter;

    public PlayerMovingState(PlayerPresenter presenter)
    {
        _presenter = presenter;
    }

    public void Enter()
    {
        _presenter.View.PlayRunAnim(true);
    }

    public void Tick()
    {
        Vector2 input = _presenter.InputService.JoystickInput;
        if (input == Vector2.zero)
        {
            _presenter.StateMachine.ChangeState(_presenter.IdleState);
            return;
        }

        Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;
        float speed = _presenter.Model.MoveSpeed;
        _presenter.View.SetVelocity(moveDir * speed);

        // Nếu có địch -> Nhìn vào địch
        // Nếu không có địch -> Nhìn theo hướng chạy
        var target = _presenter.EnemyDetector.ClosestEnemy;
        if (target != null)
        {
            Vector3 lookDir = (target.transform.position - _presenter.transform.position).normalized;
            lookDir.y = 0;
            _presenter.View.RotateTowards(lookDir);
        }
        else
        {
            _presenter.View.RotateTowards(moveDir);
        }
    }

    public void Exit() { }
}