using UnityEngine;

public class SimpleTwoHandIK : MonoBehaviour
{
    private Animator animator;

    [Header("IK Targets")]
    public Transform rightHandObj; // Drag "Right Hand Holder" here
    public Transform leftHandObj;  // Drag "Left Hand Holder" here

    [Header("Settings")]
    [Range(0, 1)] public float globalWeight = 1.0f; // Master slider for testing
    
    // Optional: Add "Hint" objects for elbows if they bend weirdly
    // public Transform leftElbowHint; 
    // public Transform rightElbowHint;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // This special function runs AFTER animation but BEFORE rendering
    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // --- RIGHT HAND ---
        if (rightHandObj != null)
        {
            // 1. Tell Unity how strongly to force the hand (1 = 100% force)
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, globalWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, globalWeight);

            // 2. Set the goal position/rotation to match your blue sphere
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
        }

        // --- LEFT HAND ---
        if (leftHandObj != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, globalWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, globalWeight);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
        }
    }
}