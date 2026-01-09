using UnityEngine;

[ExecuteAlways] // Runs in editor so you can see adjustments instantly
public class UniversalWeaponRig : MonoBehaviour
{
    public enum WeaponType { OneHanded, TwoHanded }

    [Header("1. Settings")]
    public WeaponType rigType = WeaponType.TwoHanded;
    public bool stickToRightHand = true; // If false, sticks to left hand (useful for lefties)
    public Vector3 rotationOffset;       // Fixes upside-down guns

    [Header("2. Character Bones (Assign Manually or via Code)")]
    public Transform rightHandBone;
    public Transform leftHandBone;

    [Header("3. Weapon Handles")]
    public Transform mainHandleAnchor; // The main grip (Right hand usually)
    public Transform offHandAnchor;    // The barrel grip (Left hand) - Only for TwoHanded

    void LateUpdate()
    {
        if (rightHandBone == null || mainHandleAnchor == null) return;

        if (rigType == WeaponType.OneHanded)
        {
            UpdateOneHanded();
        }
        else if (rigType == WeaponType.TwoHanded && offHandAnchor != null && leftHandBone != null)
        {
            UpdateTwoHanded();
        }
    }

    // --- LOGIC FOR PISTOLS / SWORDS ---
    void UpdateOneHanded()
    {
        Transform targetHand = stickToRightHand ? rightHandBone : leftHandBone;

        // 1. Rotation: Match the hand's rotation exactly
        Quaternion desiredRotation = targetHand.rotation * Quaternion.Euler(rotationOffset);
        
        // Adjust for the handle's local rotation inside the gun model
        transform.rotation = desiredRotation * Quaternion.Inverse(mainHandleAnchor.localRotation);

        // 2. Position: Snap main handle to hand
        // We calculate the offset from the handle to the gun center, then apply it
        Vector3 positionCorrection = targetHand.position - mainHandleAnchor.position;
        transform.position = transform.position + positionCorrection;
    }

    // --- LOGIC FOR RIFLES / MINIGUNS ---
    void UpdateTwoHanded()
    {
        // 1. Calculate Direction
        // Gun looks from Off-Hand (Left) -> Main-Hand (Right)
        Vector3 direction = (rightHandBone.position - leftHandBone.position).normalized;
        
        // Apply rotation
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(rotationOffset);

        // 2. Calculate Center Position
        // Find center between character hands
        Vector3 centerHands = (rightHandBone.position + leftHandBone.position) / 2f;
        
        // Find center between gun handles (Local)
        Vector3 centerAnchors = (mainHandleAnchor.localPosition + offHandAnchor.localPosition) / 2f;

        // Move gun to align centers
        transform.position = centerHands - (transform.rotation * centerAnchors);
    }
    
    // Call this if spawning enemies dynamically
    public void SetupBones(Transform rightHand, Transform leftHand)
    {
        rightHandBone = rightHand;
        leftHandBone = leftHand;
    }
}