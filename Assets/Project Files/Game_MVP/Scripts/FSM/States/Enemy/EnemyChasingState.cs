using UnityEngine;

public class EnemyChasingState : IState
{
    private EnemyPresenter _presenter;

    public EnemyChasingState(EnemyPresenter presenter)
    {
        _presenter = presenter;
    }

    public void Enter()
    {
        _presenter.View.PlayRunAnim(true);
    }

    public void Tick()
    {
        if (_presenter.Target == null)
        {
            _presenter.StateMachine.ChangeState(_presenter.IdleState);
            return;
        }

        Vector3 direction = (_presenter.Target.position - _presenter.transform.position).normalized;
        float distance = Vector3.Distance(_presenter.transform.position, _presenter.Target.position);

        // 1. Nếu đã sát bên -> Chuyển sang Attack
        if (distance <= _presenter.AttackRange)
        {
            _presenter.StateMachine.ChangeState(_presenter.AttackState);
            return;
        }

        // 2. Nếu Player chạy xa quá -> Bỏ cuộc (Về Idle)
        if (distance > _presenter.DetectRange * 1.5f)
        {
            _presenter.StateMachine.ChangeState(_presenter.IdleState);
            return;
        }

        _presenter.View.SetVelocity(direction * _presenter.Model.MoveSpeed);
        _presenter.View.RotateTowards(direction);
    }

    public void Exit() { }
}