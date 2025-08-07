using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GenericPool<Bullet> _pool;
    private IDamageDealer _shooter;
    private Vector3 _direction;
    private float _speed;

    public void Initialize(GenericPool<Bullet> pool, IDamageDealer shooter, float speed)
    {
        _pool = pool;
        _shooter = shooter;
        _speed = speed;
        
        _direction = transform.forward;
        gameObject.SetActive(true);
    }

    public void Update()
    {
        transform.position += _direction * (_speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(_shooter.Damage);
        
        Recycle();
    }
    
    public void Recycle()
    {
        gameObject.SetActive(false);
        _pool.Recycle(this);
    }
}