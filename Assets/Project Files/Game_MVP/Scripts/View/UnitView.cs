using System;
using UnityEngine;
public class UnitView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody _mainRb;     
    [SerializeField] private Collider _mainCollider; 
    [SerializeField] private Animator _animator;
    [SerializeField] private Renderer _renderer;

    // Danh sách các xương vật lý (Hips, Leg, Arm...)
    private Rigidbody[] _ragdollRigidbodies;
    private Collider[] _ragdollColliders;
    [SerializeField] private float _rotationSpeed = 20f;
    public Action OnAttackExecute;
    // Cache String ra Hash để tối ưu hiệu năng (Nguyễn Giê Min bảo thế!)
    private static readonly int IsRunHash = Animator.StringToHash("IsRun");
    private static readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private static readonly int GotHitHash = Animator.StringToHash("GotHit");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private void Awake()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _ragdollColliders = GetComponentsInChildren<Collider>();

        ToggleRagdoll(false);
    }
    public void ToggleRagdoll(bool isRagdoll)
    {
        foreach (var rb in _ragdollRigidbodies)
        {
            if (rb == _mainRb) continue; 

            rb.isKinematic = !isRagdoll; 
            rb.useGravity = isRagdoll;
        }

        foreach (var col in _ragdollColliders)
        {
            if (col == _mainCollider) continue; 
            if (col.gameObject == this.gameObject) continue;

            col.enabled = isRagdoll; 
        }

        if (isRagdoll)
        {
            _animator.enabled = false;     
            _mainRb.isKinematic = true;     
            _mainCollider.enabled = false;  
        }
        else
        {
            _animator.enabled = true;
            _mainRb.isKinematic = false;
            _mainCollider.enabled = true;
        }
    }
    public void SetVelocity(Vector3 velocity)
    {
        velocity.y = _mainRb.velocity.y; 
        _mainRb.velocity = velocity;
    }
    public void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
    public void PlayRunAnim(bool isRunning) => _animator.SetBool(IsRunHash, isRunning);
    public void PlayAttackAnim() => _animator.SetTrigger(IsAttackHash);
    public void PlayGotHitAnim() => _animator.SetTrigger(GotHitHash);
    
    public void PlayDeathAnim()
    {
        ToggleRagdoll(true);
    }

    public void SetPosition(Vector3 pos) => transform.position = pos;
    
    public void DestroySelf() => Destroy(gameObject);
    public void OnHitCallback()
    {
        OnAttackExecute?.Invoke();
    }

    // Animation gọi tên này khi kết thúc cú đánh
    public void OnHitFinishCallback()
    {
        // Để trống
    }
}