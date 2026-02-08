using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectManager : MonoBehaviour
{
    //public static ObjectManager Instance { get; } = new ObjectManager();
    private static ObjectManager _instance;
    public static ObjectManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<ObjectManager>();
            }
            return _instance;
        }
    }

    //로그인, 보유 캐릭터 리스트 정보
    public List<CharacterInfo> characterListInfo;

    public int SelectedCharacterId { get; set; }

    //플레이어 본인 정보
    public MyPlayer MyPlayer { get; set; }
}