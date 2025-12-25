using UnityEngine;

namespace SquadShooterMVP
{
    public abstract class BaseUnitPresenter : MonoBehaviour
    {
        // [Diagram Connections]
        [Header("Views")]
        [SerializeField] protected UnitMovementView _movementView;
        [SerializeField] protected UnitShootingView _shootingView;
        [SerializeField] protected UnitDamageView _damageView;

        protected UnitModel _model;

        // MainManager calls this to inject dependencies
        public virtual void Setup(UnitModel model)
        {
            _model = model;

            // Model changes -> View updates automatically
            _model.Health.Subscribe(OnHealthChanged);
        }

        public void TakeDamage(int damage)
        {
            if (_model.IsDead.Value) return;

            // Modify Model
            _model.Health.Value -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage, health now {_model.Health.Value}");
            
            // Command View
            _damageView.PlayHitEffect();

            if (_model.Health.Value <= 0)
            {
                Die();
            }
        }

        protected virtual void OnHealthChanged(int currentHealth)
        {
            
        }
        public bool IsDead()
        {
            return _model.IsDead.Value || _model.Health.Value <= 0;
        }
        protected virtual void Die()
        {
            _model.IsDead.Value = true;
            _damageView.PlayDeathAnimation();
            Destroy(gameObject, 1f);
        }

        protected abstract void HandleMovement();
        protected abstract void HandleShooting();

        protected virtual void Update()
        {
            if (_model == null || _model.IsDead.Value) return;
            HandleMovement();
            HandleShooting();
        }
    }
}