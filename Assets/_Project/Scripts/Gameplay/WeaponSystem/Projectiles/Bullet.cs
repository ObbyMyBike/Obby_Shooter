using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GenericPool<Bullet> _pool;
    private BulletCollision _collision;
    private IDamageDealer _shooter;
    private LayerMask _collisionMask;
    private Vector3 _direction;
    private Vector3 _spawnPosition;
    private float _maxDistance;
    private float _speed;

    public void Initialize(GenericPool<Bullet> pool, IDamageDealer shooter, float speed, Vector3 direction, LayerMask collisionMask, 
        BulletCollision collision, float maxDistance)
    {
        _pool = pool;
        _shooter = shooter;
        _speed = speed;
        _direction = direction.normalized;
        _collisionMask = collisionMask;
        _collision = collision;
        _spawnPosition = transform.position;
        _maxDistance = maxDistance;
        
        gameObject.SetActive(true);
    }

    public void Update()
    {
        transform.position += _direction * (_speed * Time.deltaTime);
        
        float travelled = Vector3.Distance(_spawnPosition, transform.position);
        
        if (travelled > _maxDistance)
            Recycle();
    }

    private void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;

        if ((_collisionMask.value & otherLayerMask) == 0)
            return;

        _collision.Collision(this, other, _shooter);

        Recycle();
    }

    private void Recycle()
    {
        gameObject.SetActive(false);
        _pool.Recycle(this);
    }
}