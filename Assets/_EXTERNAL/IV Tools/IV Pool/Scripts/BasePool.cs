using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class BasePool : GenericPool<PoolableObject>
{

}

public class GenericPool<T> : MonoBehaviour, IObjectPool where T : PoolableObject
{
    public T Prefab;

    public int InitialSize = 10;

    public bool DynamicallyIncreaseSize = true;

    private Transform _transform;

    private readonly List<T> _objects = new List<T>();

    [SerializeField]
    [ReadOnly]
    private int _availableObjects;

    private void Awake()
    {
        _transform = GetComponent<Transform>();

        for (int i = 0; i < InitialSize; i++)
        {
            InstantiateObj();
        }

        _availableObjects = InitialSize;
    }

    public T Spawn(Vector3 pos, Quaternion rot)
    {
        return Spawn((obj) =>
        {
            obj.Transform.position = pos;
            obj.Transform.rotation = rot;
        });
    }

    public T Spawn(Action<T> onSpawn = null)
    {
        T toReturn = null;

        for (int i = 0; i < _objects.Count; i++)
        {
            T obj = _objects[i];

            if (!obj.gameObject.activeSelf)
            {
                toReturn = obj;
                break;
            }
        }

        if (toReturn == null)
        {
            if (DynamicallyIncreaseSize)
            {
                toReturn = InstantiateObj();
            }
            else
            {
                throw new Exception("Pool limit reached!");
            }
        }

        if (onSpawn != null)
            onSpawn(toReturn);

        EnableObj(toReturn, true);

        toReturn.OnSpawned();

        _availableObjects--;

        return toReturn;
    }

    public void Dispose(PoolableObject obj)
    {
        obj.OnDispose();
        EnableObj(obj, false);
        _availableObjects++;
    }

    public void DisposeAll()
    {
        for (int counter = 0; counter < _objects.Count; counter++)
        {
            _objects[counter].Dispose();
        }
    }

    private T InstantiateObj()
    {
        T obj = Instantiate(Prefab.gameObject).GetComponent<T>();

        EnableObj(obj, false);

        obj.Pool = this;

        obj.Transform.position = _transform.position;
        obj.Transform.rotation = _transform.rotation;

        _objects.Add(obj);

        return obj;
    }

    void EnableObj(PoolableObject obj, bool enable)
    {
        obj.gameObject.SetActive(enable);

        if (!enable)
            obj.Transform.SetParent(_transform);
        else
            obj.Transform.SetParent(null);
    }
}

public interface IObjectPool
{
    void Dispose(PoolableObject obj);
}
