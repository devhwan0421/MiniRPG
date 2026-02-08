using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public static ResourceManager Instance { get; } = new ResourceManager();

    private Dictionary<string, Object> _resources = new Dictionary<string, Object>();

    public T Load<T>(string path) where T : Object
    {
        Debug.Log($"Load Resource: {path}");
        if (_resources.TryGetValue(path, out Object resource))
        {
            return resource as T;
        }

        T res = Resources.Load<T>(path);

        if (res == null) return null;

        _resources.Add(path, res);
        return res;
    }

    public Sprite GetItemSprite(int itemId)
    {
        if (itemId < 1) return null;
        return Load<Sprite>($"Sprites/Items/{itemId}");
    }

    public GameObject GetUi(string prefabName)
    {
        if (prefabName  == null) return null;
        return Load<GameObject>($"Prefabs/Ui/{prefabName}");
    }
}