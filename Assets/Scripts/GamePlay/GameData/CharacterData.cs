using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    //public static CharacterData Instance { get; } = new CharacterData();

    Dictionary<PlayerState, int> IndexPair = new();
    List<SPUM_Prefabs> _savedUnitList = new List<SPUM_Prefabs>();
    Dictionary<string, SPUM_Prefabs> _prefabs = new Dictionary<string, SPUM_Prefabs>();

    public void Init()
    {
        var saveArray = Resources.LoadAll<SPUM_Prefabs>("");
        foreach (var unit in saveArray)
        {
            if (unit.ImageElement.Count > 0)
            {
                _savedUnitList.Add(unit);
                _prefabs.Add(unit._code, unit);
                unit.PopulateAnimationLists();
            }
        }

        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            IndexPair[state] = 0;
        }
    }

    public Dictionary<PlayerState, int> GetIndexPair()
    {
        return IndexPair;
    }

    public SPUM_Prefabs GetCharacterPrefabData(int classId)
    {
        string code;
        if (classId == 0)
            code = "SPUM_20240911215638474";
        else if (classId == 1)
            code = "SPUM_20240911215638558";
        else if (classId == 2)
            code = "SPUM_20240911215638643";
        else
            code = "SPUM_20240911215638731";

        return _prefabs.GetValueOrDefault(code);
    }
}