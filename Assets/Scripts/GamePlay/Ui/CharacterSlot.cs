using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlot : UI_Base
{
    public TMP_Text _level;
    public TMP_Text _nickName;
    private int _characterId;

    public void SetInfo(CharacterInfo info)
    {
        _nickName.text = info.Nickname;
        _level.text = $"Lv. {info.Level}";
        _characterId = info.CharacterId;

        var go = Managers.CharacterData.GetCharacterPrefabData(info.ClassId);

        SPUM_Prefabs instance = Instantiate(go, gameObject.transform.position, Quaternion.identity);
        instance.transform.localScale = new Vector3(2f, 2f, 2f);
        instance.transform.SetParent(this.transform);
        instance.name = info.Nickname;
        instance.OverrideControllerInit();

        BindEvent(gameObject, OnClickSlot);

        gameObject.SetActive(true);
    }

    private void OnClickSlot(PointerEventData evt)
    {
        Managers.Object.SelectedCharacterId = _characterId;

        Debug.Log($"[Slot] {_characterId} 선택됨");
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}

/*Proxy EnsureProxy(int id, bool isMine, string nickname)
{
    if (_players.TryGetValue(id, out var proxy) && proxy.go != null) // 이미 있으면
        return proxy;

    proxy = new Proxy();
    string code = "SPUM_20240911215638389"; //SPUM_20240911215638474
    if (_prefabs.TryGetValue(code, out var go))
    {
        SPUM_Prefabs instance = Instantiate(go, Vector2.zero, Quaternion.identity);
        instance.name = isMine ? $"Player_{id}_Me" : $"Player_{id}";
        instance.transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
        instance.OverrideControllerInit();
        proxy.go = instance.gameObject;
    }

    var player = proxy.go.GetComponent<Player>();
    if (player == null) player = proxy.go.AddComponent<Player>();
    player.playerId = id;
    player.client = this;
    player.setNickname(nickname);

    proxy.playerObj = player;

    _players[id] = proxy; // 딕셔너리에 등록

    if (isMine)
    {
        player.isMine = true;
        Debug.Log($"[MiniClient] isMine={isMine}");
        player.hpImage = GameObject.Find("UI").transform.GetChild(2).GetComponent<Image>();

        //웨폰 오브젝트 탐색
        var weaponObj = proxy.go.transform.Find("UnitRoot/Root/BodySet/P_Body/ArmSet/ArmR/P_RArm/P_Weapon/R_Weapon");
        var weapon = weaponObj.gameObject.AddComponent<Weapon>();
        weapon.client = this;
        weapon.player = player;
        weapon.hitbox = weaponObj.GetComponent<Collider2D>();
    }
    return proxy;
}*/