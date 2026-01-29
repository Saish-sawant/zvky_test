using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager<T> where T : MonoBehaviour
{
    private readonly T prefab;                  // The prefab to pool
    private readonly Transform defaultParent;   // Default parent transform
    private readonly Queue<T> pool;             // Queue to store pooled objects

    public ObjectPoolManager(T prefab, Transform defaultParent, int initialPoolSize = 10)
    {
        this.prefab = prefab;
        this.defaultParent = defaultParent;
        pool = new Queue<T>();

        // Initialize pool
        InitializePool(initialPoolSize);
    }

    // Initialize the pool with inactive prefabs
    private void InitializePool(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            T instance = Object.Instantiate(prefab, defaultParent);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    // Get an object from the pool
    public T Get(Transform parent)
    {
        T instance;

        if (pool.Count > 0)
        {
            instance = pool.Dequeue();
        }
        else
        {
            // If pool is empty, instantiate a new one
            instance = Object.Instantiate(prefab, parent);
        }

        instance.transform.SetParent(parent);
        instance.gameObject.SetActive(true);
        return instance;
    }

    // Return an object to the pool
    public void Return(T instance)
    {
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(defaultParent); // Return to default pool parent
        pool.Enqueue(instance);
    }
}
