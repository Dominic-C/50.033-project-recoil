using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPooler : MonoBehaviour  // singleton class
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
        public int spawnCount = 0;
        public List<GameObject> objectPool = new List<GameObject>();
    }

    public Dictionary<string, Pool> poolDictionary;
    
    // all projectile prefabs
    public GameObject shotgunProjectilePrefab;
    public GameObject rocketProjectilePrefab;

    // implementing ObjectPooler as singleton
    private static ObjectPooler instance = null;
    private static readonly object objectPoolerLock = new object();  // locking objectPooler because it may be accessed by multiple threads beyond this class
    public static ObjectPooler Instance
    {
        get 
        {
            lock(objectPoolerLock)
            {
                return instance;
            }
        }
    }

    private void Awake()
    {
        // implementing ObjectPooler as singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);  // don't destroy this instance of ObjectPooler when changing scenes
    }

    void Start()
    {
        Pool shotgunProjectilePool = new Pool {prefab=shotgunProjectilePrefab, size=20};
        Pool rocketProjectilePool = new Pool {prefab=rocketProjectilePrefab, size=10};
        
        // all existing pools
        poolDictionary = new Dictionary<string, Pool>
        {
            {"shotgun", shotgunProjectilePool},
            {"rocket", rocketProjectilePool},
        };

        // populate existing pools
        foreach (string poolTag in poolDictionary.Keys)
        {
            Pool pool = poolDictionary[poolTag];
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                DontDestroyOnLoad(obj);
                pool.objectPool.Add(obj);
            }
        }
    }

    public GameObject SpawnFromPool(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Pool pool = poolDictionary[tag];
            List<GameObject> objectPool = pool.objectPool;
            for (int i=0; i<objectPool.Count; i++)
            {
                if (!objectPool[i].activeInHierarchy)
                {
                    return objectPool[i];
                }
            }
            return null;
        }
        else
        {
            Debug.LogWarning("Pool with tag" + tag + " does not exist");
            return null;
        }
    }
}
