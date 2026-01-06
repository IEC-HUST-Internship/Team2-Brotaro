using UnityEngine;

public class EnemyDeadState : IState
{
    private EnemyPresenter _presenter;
    private float _despawnTimer;

    public EnemyDeadState(EnemyPresenter presenter)
    {
        _presenter = presenter;
    }

    public void Enter()
    {
        _presenter.View.SetVelocity(Vector3.zero);
        _presenter.View.PlayDeathAnim();
    }

    public void Tick()
    {
        _despawnTimer += Time.deltaTime;
        if (_despawnTimer > 2f)
        {
            Object.Destroy(_presenter.gameObject);
        }
    }

    public void Exit() { }
}