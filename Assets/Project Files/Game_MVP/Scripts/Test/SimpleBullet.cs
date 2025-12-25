using UnityEngine;
using SquadShooterMVP; // Use your namespace!

public class SimpleBullet : MonoBehaviour
{
    public int Damage = 50;
    void Update() { transform.Translate(Vector3.forward * 20f * Time.deltaTime); }

    void OnTriggerEnter(Collider other)
    {
        // Look for the SHARED BaseUnitPresenter
        var unit = other.GetComponentInParent<BaseUnitPresenter>();
        if (unit != null)
        {
            unit.TakeDamage(Damage);
            Debug.Log("Bullet hit " + unit.name + " for " + Damage + " damage.");
            Destroy(gameObject); 
        }
    }
}