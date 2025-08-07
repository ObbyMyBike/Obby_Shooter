using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenericPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Stack<T> _stack = new Stack<T>();

    public GenericPool(T prefab, Transform parent = null, int initialSize = 10)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T instantiate = Object.Instantiate(_prefab, parent);
            
            instantiate.gameObject.SetActive(false);
            _stack.Push(instantiate);
        }
    }

    public T Create(Vector3 position, Quaternion rotation)
    {
        T item = _stack.Count > 0 ? _stack.Pop() : Object.Instantiate(_prefab, _parent);
        item.transform.SetPositionAndRotation(position, rotation);
        item.gameObject.SetActive(true);
        
        return item;
    }

    public void Recycle(T item)
    {
        item.gameObject.SetActive(false);
        _stack.Push(item);
    }
}