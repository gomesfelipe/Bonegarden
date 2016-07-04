using UnityEngine;

public class PoolableObject : MonoBehaviour 
{
    private Transform _transform;

    public IObjectPool Pool { get; set; }

    public Transform Transform
    {
        get
        {
            if (_transform == null)
                _transform = GetComponent<Transform>();

            return _transform;
        }
    }
    
    public virtual void OnSpawned()
    {

    }

    public virtual void OnDispose()
    {

    }

    public void Dispose()
    {
        Pool.Dispose(this);
    }
}
