using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
[Header("Settings")]
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 2f;    

    private float _damage;

    public void Init(float damage)
    {
        _damage = damage;
        
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {

        var enemy = other.GetComponentInParent<EnemyPresenter>();

        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
            
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("Bullet")) 
        {
            Destroy(gameObject);
        }
    }
}