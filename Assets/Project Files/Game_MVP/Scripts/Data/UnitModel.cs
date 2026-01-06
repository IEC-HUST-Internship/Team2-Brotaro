using System;
using UnityEngine;

public class UnitModel
{
    // Stats Base
    public float MoveSpeed { get; private set; }
    public float MaxHealth { get; private set; }
    public int Damage { get; private set; }
    
    // Stats Level và Exp
    public ReactiveProperty<int> Level = new ReactiveProperty<int>(1);
    public ReactiveProperty<float> CurrentExp = new ReactiveProperty<float>(0);
    public float MaxExp => Level.Value * 100f; 

    // Stats Runtime
    public ReactiveProperty<float> CurrentHealth = new ReactiveProperty<float>(0);
    public ReactiveProperty<bool> IsDead = new ReactiveProperty<bool>(false);

    public UnitModel(float speed, float hp, int dmg)
    {
        MoveSpeed = speed;
        MaxHealth = hp;
        CurrentHealth.Value = hp;
        Damage = dmg;
    }

    public void AddExp(float amount)
    {
        CurrentExp.Value += amount;
        while (CurrentExp.Value >= MaxExp)
        {
            CurrentExp.Value -= MaxExp;
            Level.Value++;
            Debug.Log($"Level Up! New Level: {Level.Value}");
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead.Value) return;

        CurrentHealth.Value -= amount;
        if (CurrentHealth.Value <= 0)
        {
            CurrentHealth.Value = 0;
            IsDead.Value = true;
        }
    }
}