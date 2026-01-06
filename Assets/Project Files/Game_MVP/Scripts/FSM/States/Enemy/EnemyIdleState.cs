using UnityEngine;

public class EnemyIdleState : IState
{
    private EnemyPresenter _presenter;

    public EnemyIdleState(EnemyPresenter presenter)
    {
        _presenter = presenter;
    }

    public void Enter()
    {
        _presenter.View.PlayRunAnim(false);
        _presenter.View.SetVelocity(Vector3.zero);
    }

    public void Tick()
    {
        if (_presenter.Target == null) return;

        float distance = Vector3.Distance(_presenter.transform.position, _presenter.Target.position);
        
        if (distance <= _presenter.DetectRange)
        {
            _presenter.StateMachine.ChangeState(_presenter.ChaseState);
        }
    }

    public void Exit() { }
}