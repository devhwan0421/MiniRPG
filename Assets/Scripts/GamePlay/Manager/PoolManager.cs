using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //public static PoolManager Instance { get; private set; }
    private static PoolManager _instance;

    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PoolManager>();
            }
            return _instance;
        }
    }

    private Dictionary<string, Stack<GameObject>> _pool = new Dictionary<string, Stack<GameObject>>();

    /*private void Awake()
    {
        if (Instance == null) Instance = this;
    }*/

    public void PreLoad(GameObject prefab, int count = 50)
    {
        Debug.Log("PreLoad");
        string key = prefab.name;
        if (!_pool.ContainsKey(key))
            _pool.Add(key, new Stack<GameObject>());
        
        for(int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = key;
            Push(obj);
        }
    }

    public void Push(GameObject obj)
    {
        Debug.Log("Push");
        string key = obj.name;
        if (!_pool.ContainsKey(key)) return;

        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        _pool[key].Push(obj);
    }

    public GameObject Pop(GameObject prefab, Transform parent = null)
    {
        Debug.Log("Pop");
        string key = prefab.name;
        if(!_pool.ContainsKey(key)) return null;

        GameObject obj;
        if (_pool[key].Count > 0)
        {
            obj = _pool[key].Pop();
        }
        else
        {
            obj = Instantiate(prefab);
            obj.name = key;
        }

        obj.transform.SetParent(parent);
        obj.SetActive(true);
        return obj;
    }
}