using UnityEngine;
using System;

public class EnemyPresenter : MonoBehaviour
{
    // --- SETUP ---
    public UnitModel Model { get; private set; }
    public UnitView View { get; private set; }
    public StateMachine StateMachine { get; private set; }
    
    // Mục tiêu (Player)
    public Transform Target { get; private set; }

    // --- CONFIG ---
    [Header("AI Settings")]
    public float DetectRange = 10f; 
    public float AttackRange = 1.5f; 
    public float AttackCooldown = 1.5f; 

    // --- STATES ---
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChasingState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeadState DeadState { get; private set; }

    public void Init(UnitModel model, UnitView view, Transform target)
    {
        Model = model;
        View = view;
        Target = target;

        // Setup State Machine
        StateMachine = new StateMachine();
        IdleState = new EnemyIdleState(this);
        ChaseState = new EnemyChasingState(this);
        AttackState = new EnemyAttackState(this);
        DeadState = new EnemyDeadState(this);

        Model.IsDead.Subscribe(isDead => { if (isDead) StateMachine.ChangeState(DeadState);});

        StateMachine.ChangeState(IdleState);
    }

    private void Update()
    {
        if (Model == null) return; 
        
        StateMachine.Tick();
    }
    
    public void TakeDamage(float amount)
    {
        if (Model.IsDead.Value) return;
        Model.TakeDamage(amount);
        //View.PlayGotHitAnim(); 
    }
}