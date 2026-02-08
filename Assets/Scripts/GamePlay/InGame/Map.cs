using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class Map : MonoBehaviour
{
    public int _mapId;
    public string _mapName;

    private void Awake()
    {
        Managers.Map.MapObject = this;

        Managers.Map.SpwanPlayers();
        Managers.Map.SpawnItem();
        Managers.Map.SpawnMonsters();
        Debug.Log("맵 Awake완료");
        Managers.Map._isLoad = true;
    }
}