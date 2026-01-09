using UnityEngine;
using System;


public class PlayerPresenter : MonoBehaviour
{
    public UnitModel Model { get; private set; }
    public UnitView View { get; private set; }
    public InputService InputService { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public EnemyDetector EnemyDetector { get; private set; }

    // Config Combat
    public float AttackRange = 5f;
    public float AttackSpeed = 2f; 
    public GameObject Bullet;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMovingState MoveState { get; private set; }
    public PlayerDieState DieState { get; private set; }

    private float _attackTimer;
    [Header("Aiming")]
    [SerializeField] private AimRingView _aimRingPrefab; 
    private AimRingView _aimRingInstance; 

    public void Init(UnitModel model, UnitView view, InputService input, Action<float, float> onHp, Action<float, float> onExp, Action<int> onLvl)
    {
        Model = model;
        View = view;
        InputService = input;

        EnemyDetector = GetComponentInChildren<EnemyDetector>();  
        if (!EnemyDetector)
        {
            Debug.LogError("EnemyDetector component not found in children!");
            return;
        }
        EnemyDetector.Init(AttackRange);

        _aimRingInstance = Instantiate(_aimRingPrefab, transform);
        if (_aimRingInstance) { _aimRingInstance.Init(transform); _aimRingInstance.SetRadius(AttackRange); }

        // Setup State Machine
        StateMachine = new StateMachine();
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMovingState(this);
        DieState = new PlayerDieState(this);

        Model.IsDead.Subscribe(isDead => { if (isDead) StateMachine.ChangeState(DieState); });

        StateMachine.ChangeState(IdleState);
    }

    public void Tick()
    {
        if (Model.IsDead.Value) return;

        StateMachine.Tick();
        
        EnemyDetector.UpdateClosestEnemy();
        HandleCombat();
    }

    private void HandleCombat()
    {
        var target = EnemyDetector.ClosestEnemy;

        if (_aimRingInstance) _aimRingInstance.Toggle(target != null);

        if (target == null) return;
        
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= (1f / AttackSpeed))
        {
            _attackTimer = 0;
            View.PlayAttackAnim();
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0; 

            Quaternion rotation = Quaternion.LookRotation(direction);

            var bulletObj = Instantiate(Bullet, transform.position + Vector3.up * 1.0f, rotation);
            bulletObj.GetComponent<SimpleBullet>().Init(10f);
            Debug.Log("Pew Pew!");
        }
    }
    public void TakeDamage(float amount)
    {
        if (Model.IsDead.Value) return;
        Model.TakeDamage(amount);
        //View.PlayGotHitAnim(); 
    }
}