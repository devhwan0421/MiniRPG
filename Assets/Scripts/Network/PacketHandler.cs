using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

public class PacketHandler
{
    public static PacketHandler Instance { get; } = new PacketHandler();
    public NetworkManager Network { set; get; }

    private Dictionary<PacketID, Action<string>> _handlers = new Dictionary<PacketID, Action<string>>();

    //로그인 처리용 이벤트
    public event Action<bool> OnLoginResponse;
    public event Action<GetCharacterListResponse> OnGetCharacterListResponse;
    public event Action<EnterWorldResponse> OnEnterWorldResponse;
    public event Action<InventoryResponse> OnInventoryResponse;
    public event Action<SpawnPlayerResponse> OnSpawnOnePlayerResponse;

    public PacketHandler()
    {
        //Desrialize를 유니티메인스레드가 아닌 리시브 스레드에서 한 후 받도록 추후 변경
        _handlers.Add(PacketID.LoginResponse, (json) => HandleLoginResponse(JsonSerializer.Deserialize<LoginResponse>(json)));
        _handlers.Add(PacketID.GetCharacterListResponse, (json) => HandleGetCharacterListResponse(JsonSerializer.Deserialize<GetCharacterListResponse>(json)));
        _handlers.Add(PacketID.EnterWorldResponse, (json) => HandleEnterWorldResponse(JsonSerializer.Deserialize<EnterWorldResponse>(json)));
        _handlers.Add(PacketID.InventoryResponse, (json) => OnInventoryResponse?.Invoke(JsonSerializer.Deserialize<InventoryResponse>(json)));

        _handlers.Add(PacketID.SpawnPlayerResponse, (json) => HandleSpawnOnePlayer(JsonSerializer.Deserialize<SpawnPlayerResponse>(json)));
        _handlers.Add(PacketID.PlayerMoveResponse, (json) => HandlePlayerMove(JsonSerializer.Deserialize<PlayerMoveResponse>(json)));
        _handlers.Add(PacketID.DespawnPlayerResponse, (json) => HandleDespawnPlayer(JsonSerializer.Deserialize<DespawnPlayerResponse>(json)));
        _handlers.Add(PacketID.MoveMapResponse, (json) => HandleMoveMap(JsonSerializer.Deserialize<MoveMapResponse>(json)));
        _handlers.Add(PacketID.UseItemResponse, (json) => HandleUseItem(JsonSerializer.Deserialize<UseItemResponse>(json)));
        _handlers.Add(PacketID.DropItemResponse, (json) => HandleDropItem(JsonSerializer.Deserialize<DropItemResponse>(json)));
        _handlers.Add(PacketID.SpawnItemResponse, (json) => HandleSpawnItem(JsonSerializer.Deserialize<SpawnItemResponse>(json)));
        _handlers.Add(PacketID.PickUpItemResponse, (json) => HandlePickUpItem(JsonSerializer.Deserialize<PickUpItemResponse>(json)));
        _handlers.Add(PacketID.DropItemDestroyResponse, (json) => HandleDropItemDestroy(JsonSerializer.Deserialize<DropItemDestroyResponse>(json)));
        _handlers.Add(PacketID.MonsterMoveResponse, (json) => HandleMonsterMove(JsonSerializer.Deserialize<MonsterMoveResponse>(json)));
        _handlers.Add(PacketID.PlayerTakeDamageResponse, (json) => HandlePlayerTakeDamage(JsonSerializer.Deserialize<PlayerTakeDamageResponse>(json)));
        _handlers.Add(PacketID.PlayerDeathResponse, (json) => HandlePlayerDeath(JsonSerializer.Deserialize<PlayerDeathResponse>(json)));
        _handlers.Add(PacketID.PlayerHitMonsterResponse, (json) => HandlePlayerHitMonster(JsonSerializer.Deserialize<PlayerHitMonsterResponse>(json)));
        _handlers.Add(PacketID.MonsterDeathResponse, (json) => HandleMonsterDeath(JsonSerializer.Deserialize<MonsterDeathResponse>(json)));
        _handlers.Add(PacketID.MonsterSpawnResponse, (json) => HandleMonsterRespawn(JsonSerializer.Deserialize<MonsterSpawnResponse>(json)));
        //_handlers.Add(PacketID.DialogueResponse, (json) => HandleDialogue(JsonSerializer.Deserialize<DialogueResponse>(json)));
        _handlers.Add(PacketID.DialogueSimpleResponse, (json) => HandleDialogueSimple(JsonSerializer.Deserialize<DialogueSimpleResponse>(json)));
        _handlers.Add(PacketID.DialogueOkResponse, (json) => HandleDialogueOk(JsonSerializer.Deserialize<DialogueOkResponse>(json)));
        _handlers.Add(PacketID.DialogueNextResponse, (json) => HandleDialogueNext(JsonSerializer.Deserialize<DialogueNextResponse>(json)));
        _handlers.Add(PacketID.DialogueAcceptDeclineResponse, (json) => HandleDialogueAcceptDecline(JsonSerializer.Deserialize<DialogueAcceptDeclineResponse>(json)));
        _handlers.Add(PacketID.DialogueSelectionResponse, (json) => HandleDialogueSelection(JsonSerializer.Deserialize<DialogueSelectionResponse>(json)));

        _handlers.Add(PacketID.PlayerLevelUpResponse, (json) => HandlePlayerLevelUp(JsonSerializer.Deserialize<PlayerLevelUpResponse>(json)));
        _handlers.Add(PacketID.QuestCompleteResponse, (json) => HandleQuestComplete(JsonSerializer.Deserialize<QuestCompleteResponse>(json)));

        _handlers.Add(PacketID.ServerMessageResponse, (json) => HandleServerMessage(JsonSerializer.Deserialize<ServerMessageResponse>(json).Message));
    }

    public void OnRecvPacket(PacketID id, string json)
    {
        if (_handlers.TryGetValue(id, out var handler))
        {
            handler.Invoke(json);
        }
        else
        {
            Console.WriteLine("정의되지 않은 패킷이 수신되었습니다.");
        }
    }

    private void HandleLoginResponse(LoginResponse res)
    {
        OnLoginResponse?.Invoke(res.Success); //간단한 성공 여부만
    }

    private void HandleGetCharacterListResponse(GetCharacterListResponse res)
    {
        ObjectManager.Instance.characterListInfo = res.Characters;
        OnGetCharacterListResponse?.Invoke(res);
    }

    private void HandleEnterWorldResponse(EnterWorldResponse res)
    {
        OnEnterWorldResponse?.Invoke(res);
    }

    public void HandleSpawnOnePlayer(SpawnPlayerResponse res)
    {
        OnSpawnOnePlayerResponse?.Invoke(res);
    }

    public void HandlePlayerMove(PlayerMoveResponse res)
    {
        Managers.Map.UpdatePlayerMove(res);
    }

    public void HandleDespawnPlayer(DespawnPlayerResponse res)
    {
        Managers.Map.DespawnPlayer(res.CharacterId);
    }

    public void HandleMoveMap(MoveMapResponse res)
    {
        Managers.Map.MoveMap(res);
    }

    public void HandleUseItem(UseItemResponse res)
    {
        ObjectManager.Instance.MyPlayer.Inventory.OnUseItemResponse(res);
    }

    public void HandleDropItem(DropItemResponse res)
    {
        ObjectManager.Instance.MyPlayer.Inventory.OnDropItemResponse(res);
    }

    public void HandleSpawnItem(SpawnItemResponse res)
    {
        /*if(res.MapId == Managers.Map.MapId)
            Managers.Map.MapObject.OnSpawnItem(res);*/
        if (res.MapId == Managers.Map.MapId)
            Managers.Map.OnSpawnItem(res);
    }

    public void HandlePickUpItem(PickUpItemResponse res)
    {
        Managers.Object.MyPlayer.Inventory.OnPickUpItemResponse(res);
    }

    public void HandleDropItemDestroy(DropItemDestroyResponse res)
    {
        Managers.Map.OnDropItemDestroy(res.MapId, res.InventoryId);
    }

    public void HandleMonsterMove(MonsterMoveResponse res)
    {
        Managers.Map.UpdateMonsterMove(res);
    }

    public void HandlePlayerTakeDamage(PlayerTakeDamageResponse res)
    {
        Managers.Map.OnPlayerTakeDamage(res);
    }

    public void HandlePlayerDeath(PlayerDeathResponse res)
    {
        Managers.Map.OnPlayerDeath(res.CharacterId);
    }

    public void HandlePlayerHitMonster(PlayerHitMonsterResponse res)
    {
        Managers.Map.OnMonsterTakeDamage(res);
    }

    public void HandleMonsterDeath(MonsterDeathResponse res)
    {
        Managers.Map.OnMonsterDeath(res);
    }

    public void HandleMonsterRespawn(MonsterSpawnResponse res)
    {
        Managers.Map.OnMonsterRespawn(res);
    }

    /*public void HandleDialogue(DialogueResponse res)
    {
        Managers.Ui._dialogueUi.OnDialogue(res);
    }*/
    public void HandleDialogueSimple(DialogueSimpleResponse res)
    {
        Managers.Ui._dialogueUi.OnDialogueSimple(res);
    }

    public void HandleDialogueOk(DialogueOkResponse res)
    {
        Managers.Ui._dialogueUi.OnDialogueOk(res);
    }

    public void HandleDialogueNext(DialogueNextResponse res)
    {
        Managers.Ui._dialogueUi.OnDialogueNext(res);
    }

    public void HandleDialogueAcceptDecline(DialogueAcceptDeclineResponse res)
    {
        Managers.Ui._dialogueUi.OnDialogueAcceptDecline(res);
    }

    public void HandleDialogueSelection(DialogueSelectionResponse res)
    {
        Managers.Ui._dialogueUi.OnDialogueSelection(res);
    }

    public void HandlePlayerLevelUp(PlayerLevelUpResponse res)
    {
        //Managers.Ui._dialogueUi.OnDialogueSelection(res);
        Managers.Object.MyPlayer.PlayerController.OnPlayerLevelUp(res.Level, res.Damage);
    }

    public void HandleQuestComplete(QuestCompleteResponse res)
    {
        //Managers.Ui._dialogueUi.OnDialogueSelection(res);
    }

    public void HandleServerMessage(string msg)
    {
        Console.WriteLine($"[Server Message] {msg}");
    }
}