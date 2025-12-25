using UnityEngine;

namespace SquadShooterMVP
{
    public class EnemyPresenter : BaseUnitPresenter
    {
        private Transform _target;

        public void Init(UnitModel model, Transform target)
        {
            base.Setup(model);
            _target = target;
        }

        protected override void HandleMovement()
        {
            if (_target == null) return;
            if(Vector3.Distance(transform.position, _target.position) < 0.5f)
            {
                _movementView.Stop();
                return;
            }
            Vector3 dir = (_target.position - transform.position).normalized;
            _movementView.Move(dir, _model.MoveSpeed);
            _movementView.Rotate(dir);
        }

        protected override void HandleShooting()
        {
            if (_target == null) return;
            
            if (Vector3.Distance(transform.position, _target.position) < 5f)
            {
                _shootingView.Shoot(_model.Damage, _model.FireRate);
            }
        }
    }
}