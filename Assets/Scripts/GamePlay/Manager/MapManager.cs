using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public int MapId { get; set; }
    public Map MapObject { get; set; }

    private MapInfo _pendingData;
    public bool _isLoad = false;

    private Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    //맵 로드 후 플레이어 생성 전까지 검은 화면이면 안 써도 될 듯. 이후 개선
    //private Dictionary<int, Player> _playersInfo = new Dictionary<int, Player>();

    //private Dictionary<int, Item> _dropItems = new Dictionary<int, Item>();
    private Dictionary<int, GameObject> _dropItems = new Dictionary<int, GameObject>();
    //private Dictionary<int , GameObject> _dropItemGameObjects = new Dictionary<int , GameObject>();

    //private Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
    private Dictionary<int, MonsterController> _monsters = new Dictionary<int, MonsterController>();

    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MapManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        PacketHandler.Instance.OnSpawnOnePlayerResponse += OnSpawnOnePlayerRes;
    }

    private void OnDestroy()
    {
        PacketHandler.Instance.OnSpawnOnePlayerResponse -= OnSpawnOnePlayerRes;
    }

    public void Init(MapInfo mapInfo)
    {
        _isLoad = false;
        _pendingData = mapInfo;
        MapId = mapInfo.MapId;

        //_playersInfo.Clear();
        _players.Clear();
        _dropItems.Clear();
        _monsters.Clear();

        //몬스터처럼 중복 제거 필요
        /*foreach (var playerInfo in mapInfo.Players)
        {
            Player player = new Player(playerInfo);
            _playersInfo[player.CharacterId] = player; //Dictionary에 추가
        }*/

        //몬스터처럼 중복 제거 필요
        /*foreach (var itemInfo in mapInfo.DropItems)
        {
            Item item = new Item(itemInfo);
            _dropItems[item.InventoryId] = item;
        }*/
    }

    public void SpwanPlayers()
    {
        //나중에 다른 곳으로 옮길 것. 임시 테스트용
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");

        int myCharacterId = Managers.Object.MyPlayer.CharacterId;

        if (prefab != null)
        {
            //foreach (var player in _playersInfo.Values)
            foreach (var playerInfo in _pendingData.Players)
            {
                Vector3 position = new Vector3(playerInfo.PosX, playerInfo.PosY, playerInfo.PosZ);

                GameObject playerObj = Instantiate(prefab, position, Quaternion.identity);
                playerObj.name = playerInfo.Nickname;
                PlayerUi playerUi = playerObj.GetComponentInChildren<PlayerUi>();
                playerUi.SetInfo(playerInfo.Level, playerInfo.Nickname);

                //Debug.Log($"플레이어 오브젝트 생성 완료: {player.Nickname} at {position}");

                var go = Managers.CharacterData.GetCharacterPrefabData(playerInfo.ClassId);

                //SPUM_Prefabs instance = Instantiate(go, gameObject.transform.position, Quaternion.identity);
                SPUM_Prefabs unit = Instantiate(go, playerObj.transform.position, Quaternion.identity);
                unit.transform.localScale = new Vector3(2f, 2f, 2f);
                unit.transform.SetParent(playerObj.transform);
                unit.name = playerInfo.Nickname;
                unit.OverrideControllerInit();

                if (playerInfo.CharacterId == myCharacterId)
                {
                    playerObj.tag = "MyPlayer";
                    PlayerController playerController = playerObj.AddComponent<PlayerController>();
                    playerController.Init(playerInfo, unit, playerUi);

                    MainCamera mainCamera = Camera.main.GetComponent<MainCamera>();
                    if (mainCamera != null) 
                    {
                        mainCamera._target = playerObj.transform;
                    }

                    Managers.Object.MyPlayer.PlayerController = playerController;
                    Managers.Object.MyPlayer.PlayerInfo = playerInfo;
                }
                else
                {
                    OtherPlayerController otherPlayerController = playerObj.AddComponent<OtherPlayerController>();
                    otherPlayerController.Init(playerInfo, unit, playerUi);
                }

                //락 필요x 이미 유니티 메인스레드에서 호출된 함수임
                _players[playerInfo.CharacterId] = playerObj;
            }
        }
        else
        {
            Debug.LogError("플레이어 프리팹을 찾을 수 없습니다!");
        }
    }

    public void OnSpawnOnePlayerRes(SpawnPlayerResponse res)
    {
        PlayerInfo playerInfo = res.player;
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");

        Vector3 position = new Vector3(playerInfo.PosX, playerInfo.PosY, playerInfo.PosZ);

        GameObject playerObj = Instantiate(prefab, position, Quaternion.identity);
        playerObj.name = playerInfo.Nickname;
        PlayerUi playerUi = playerObj.GetComponentInChildren<PlayerUi>();
        playerUi.SetInfo(playerInfo.Level, playerInfo.Nickname);

        //Debug.Log($"플레이어 오브젝트 생성 완료: {player.Nickname} at {position}");

        var go = Managers.CharacterData.GetCharacterPrefabData(playerInfo.ClassId);

        //SPUM_Prefabs instance = Instantiate(go, gameObject.transform.position, Quaternion.identity);
        SPUM_Prefabs unit = Instantiate(go, playerObj.transform.position, Quaternion.identity);
        unit.transform.localScale = new Vector3(2f, 2f, 2f);
        unit.transform.SetParent(playerObj.transform);
        unit.name = playerInfo.Nickname;
        unit.OverrideControllerInit();

        //playerObj.AddComponent<OtherPlayerController>();

        OtherPlayerController otherPlayerController = playerObj.AddComponent<OtherPlayerController>();
        otherPlayerController.Init(playerInfo, unit, playerUi);
        

        /*MonsterController monsterController = monsterObj.AddComponent<MonsterController>();
        monsterController.Init(monster);
        monsterController.Spum = unit;*/

        //락이 필요 없다 이미 유니티 메인스레드에서 호출된 함수임
        _players[playerInfo.CharacterId] = playerObj;
    }

    public void UpdatePlayerMove(PlayerMoveResponse res)
    {
        Debug.Log($"posX {res.PosX}, state {res.State}");
        if (_players.TryGetValue(res.CharacterId, out var playerObj))
        {
            OtherPlayerController otherPlayerController = playerObj.GetComponent<OtherPlayerController>();
            if (otherPlayerController != null)
            {
                otherPlayerController.OnUpdateMove(res);
            }
        }
    }

    public void DespawnPlayer(int characterId)
    {
        if (_players.TryGetValue(characterId, out var playerObj))
        {
            _players.Remove(characterId);
            //_playersInfo.Remove(characterId);

            if (playerObj != null)
                Destroy(playerObj);
        }
    }

    public void MoveMap(MoveMapResponse res)
    {
        Debug.Log($"맵이동 => MapId: {res.MapInfo.MapId}");

        Init(res.MapInfo);

        //맵 리소스 관리자로 분리할 것
        if(res.MapInfo.MapId == 1)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map0");
        else if (res.MapInfo.MapId == 2)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map1");
    }

    public void SpawnItem()
    {
        //foreach (var item in _dropItems)
        foreach (var item in _pendingData.DropItems)
        {
            DropItemField(item.InventoryId, item.ItemId, item.PosX, item.PosY, item.PosZ);
        }
    }

    public void OnSpawnItem(SpawnItemResponse res)
    {
        if (this.MapId != res.MapId) return;

        DropItemField(res.Item.InventoryId, res.Item.ItemId, res.PosX, res.PosY, res.PosZ);
    }


    //_dropitem 딕셔너리 제거 할 것. 몬스터 처럼
    public void DropItemField(int inventoryId, int itemId, float posX, float posY, float posZ)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldItem");

        Vector3 pos = new Vector3(posX, posY + 1.0f, posZ);
        GameObject dropItem = Instantiate(prefab, pos, Quaternion.identity);
        dropItem.name = $"DropItem_{inventoryId}";

        FieldItem fieldItem = dropItem.GetComponent<FieldItem>();
        fieldItem.Init(inventoryId, itemId);

        //_dropItemGameObjects.Add(inventoryId, dropItem);
        _dropItems.Add(inventoryId, dropItem);
    }

    public void OnDropItemDestroy(int mapId, int inventoryId)
    {
        Debug.Log($"mapid: {mapId}, inventoryid: {inventoryId}");
        if (MapId != mapId) return;
        Debug.Log($"mapid: {mapId}, inventoryid1111: {inventoryId}");
        //_dropItemGameObjects.Remove(inventoryId, out var gameObject);
        _dropItems.Remove(inventoryId, out var gameObject);
        Destroy(gameObject);
    }

    public void SpawnMonsters()
    {
        //몬스터 프리팹 로드
        GameObject monsterPrefab = Resources.Load<GameObject>("Prefabs/Monster");

        if (monsterPrefab != null)
        {
            foreach (MonsterInfo monster in _pendingData.Monsters)
            {
                Vector3 position = new Vector3(monster.PosX, monster.PosY, monster.PosZ);
                GameObject monsterObj = Instantiate(monsterPrefab, position, Quaternion.identity);
                monsterObj.name = monster.Nickname;

                if(monster.State == 9)
                {
                    monsterObj.SetActive(false);
                }

                MonsterUi monsterUi = monsterObj.GetComponentInChildren<MonsterUi>();
                monsterUi.SetInfo(monster.Nickname, monster.Hp, monster.MaxHp);

                //몬스터 아이디로 해당 프리팹 로드
                SPUM_Prefabs unitPrefab = Managers.MonsterData.GetMonsterPrefabData(monster.MonsterId);

                if (unitPrefab != null)
                {
                    SPUM_Prefabs unit = Instantiate(unitPrefab, monsterObj.transform.position, Quaternion.identity, monsterObj.transform);
                    unit.transform.localScale = new Vector3(2f, 2f, 2f);
                    unit.OverrideControllerInit();

                    MonsterController monsterController = monsterObj.AddComponent<MonsterController>();
                    monsterController.Init(monster, monsterUi);
                    monsterController._spum = unit;

                    //스폰 아이디를 키값으로 딕셔너리에 추가
                    _monsters.Add(monster.SpawnId, monsterController);
                }
            }
        }
        _pendingData.Monsters.Clear(); //참조 삭제. 이렇게 말고 mapinfo를 관리하는 함수 필요. 맵은 계속 바뀔 수 있음
    }

    public void UpdateMonsterMove(MonsterMoveResponse res)
    {
        if (!_isLoad) return;

        //앞으로는 SpawnId로 관리함
        //if(_monsters.TryGetValue(res.MonsterId, out MonsterController monsterController))
        if (_monsters.TryGetValue(res.SpawnId, out MonsterController monsterController))
        {
            monsterController.OnUpdateMove(res);
        }
        else
        {
            Debug.Log("딕셔너리에 몬스터가 존재하지 않음");
        }
    }

    public void OnPlayerTakeDamage(PlayerTakeDamageResponse res)
    {
        //if(_playerGameObjects)
        if (_players.TryGetValue(res.CharacterId, out GameObject player))
        {
            if(res.CharacterId == Managers.Object.MyPlayer.CharacterId)
            {
                //Managers.Object.MyPlayer.Hp = res.Hp;
                //본인 플레이어도 플레이어인포를 가질지 말지 결정해야 함
                //플레이어 인포는 서버에서도 데이터를 가지고 있음
                //클라이언트가 정확히 알아야 되는 정보는 characterId
                //어차피 서버에 있는 정보를 받을 뿐인데 굳이 매니저로 가지고 있을 필요는 없는 듯

                PlayerController playerController = player.GetComponent<PlayerController>();
                playerController.OnTakeDamage(res);
            }
            else
            {

                OtherPlayerController otherPlayerController = player.GetComponent<OtherPlayerController>();
                otherPlayerController.OnTakeDamage(res);
            }
            
        }
    }

    public void OnPlayerDeath(int characterId)
    {
        Debug.Log($"OnPlayerDeath: {characterId}");
        if(_players.TryGetValue(characterId, out GameObject player))
        {
            if (characterId == Managers.Object.MyPlayer.CharacterId)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                playerController.OnDeath();
            }
            else
            {

                OtherPlayerController otherPlayerController = player.GetComponent<OtherPlayerController>();
                otherPlayerController.OnDeath();
            }
        }
    }

    /*public void OnPlayerHit(PlayerHitMonsterResponse res)
    {
        //if(_playerGameObjects)
        if (_monsters.TryGetValue(res.MonsterId, out MonsterController monster))
        {
            monster.OnTakeDamage(res.Hp, res.FinalDamage);
        }
    }*/

    public void OnMonsterTakeDamage(PlayerHitMonsterResponse res)
    {
        if (_monsters.TryGetValue(res.SpawnId, out MonsterController monster))
        {
            monster.OnTakeDamage(res.Hp, res.FinalDamage);
        }
    }

    public void OnMonsterDeath(MonsterDeathResponse res)
    {
        if (_monsters.TryGetValue(res.SpawnId, out MonsterController monster))
        {
            //리스폰될 것이기 때문에 삭제가 아닌 비활성화
            //_monsters.Remove(res.MonsterId);
            //Destroy(monster.gameObject);
            monster.Despawn();
        }
    }

    public void OnMonsterRespawn(MonsterSpawnResponse res)
    {
        if (_monsters.TryGetValue(res.SpawnId, out MonsterController monster))
        {
            //스폰 좌표 서버에서 받아올 것
            monster.Respawn(res.SpawnPosX, res.SpawnPosY);
        }
    }
}