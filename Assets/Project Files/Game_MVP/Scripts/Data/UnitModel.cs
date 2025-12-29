using System;

namespace SquadShooterMVP
{
    [Serializable]
    public class UnitModel
    {
        // Reactive Properties (Presenter modifies -> View listens)
        public ReactiveProperty<int> Health = new ReactiveProperty<int>(100);
        public ReactiveProperty<bool> IsDead = new ReactiveProperty<bool>(false);

        // Standard Config (Static data)
        public float MoveSpeed = 5f;
        public int Damage = 50;
        public float FireRate = 0.5f;
        public void SetDead()
        {
            if(Health.Value <= 0)
            {
                IsDead.Value = true;
            }
        }
        public UnitModel()
        {
            Health.Value = 100; 
            IsDead.Value = false;
        }
    }
}