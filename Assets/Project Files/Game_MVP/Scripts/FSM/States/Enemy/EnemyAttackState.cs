using UnityEngine;

public class EnemyAttackState : IState
{
    private EnemyPresenter _presenter;
    private float _attackTimer;

    public EnemyAttackState(EnemyPresenter presenter)
    {
        _presenter = presenter;
    }

    public void Enter()
    {
        _presenter.View.PlayRunAnim(false);
        _presenter.View.SetVelocity(Vector3.zero); 
        _attackTimer = 0; 
    }

    public void Tick()
    {
        if (_presenter.Target == null)
        {
            _presenter.StateMachine.ChangeState(_presenter.IdleState);
            return;
        }

        Vector3 dirToTarget = (_presenter.Target.position - _presenter.transform.position).normalized;
        _presenter.View.RotateTowards(dirToTarget);

        float distance = Vector3.Distance(_presenter.transform.position, _presenter.Target.position);
        if (distance > _presenter.AttackRange + 0.5f) 
        {
            _presenter.StateMachine.ChangeState(_presenter.ChaseState);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _presenter.AttackCooldown)
        {
            _attackTimer = 0;
            Attack();
        }
    }

    private void Attack()
    {
        _presenter.View.PlayAttackAnim();
        var player = _presenter.Target.GetComponent<PlayerPresenter>();
        if (player != null)
        {
            
            player.TakeDamage(_presenter.Model.Damage);
            Debug.Log($"Enemy đấm trúng player! -{_presenter.Model.Damage} HP");
        }
    }

    public void Exit() { }
}