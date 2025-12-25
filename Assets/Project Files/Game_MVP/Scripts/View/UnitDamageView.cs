using UnityEngine;

namespace SquadShooterMVP
{
    public class UnitDamageView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        
        // Command: TakeDamage(damage) visual effect
        public void PlayHitEffect()
        {
            if (_renderer != null) _renderer.material.color = Color.red;
            Invoke(nameof(ResetColor), 0.1f);
        }

        private void ResetColor()
        {
            if (_renderer != null) _renderer.material.color = Color.white;
        }

        public void PlayDeathAnimation()
        {
            if (_renderer != null) _renderer.material.color = Color.black;
            Debug.Log("[View] Unit Died Animation");
        }
    }
}