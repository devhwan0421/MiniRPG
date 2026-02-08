using System;
using System.Text.Json;

public class PacketMaker
{
    public static PacketMaker Instance { get; } = new PacketMaker();
    public NetworkManager Network { set; get; }

    public ArraySegment<byte> LoginRequest(string username, string password)
    {
        LoginRequest loginRequest = new LoginRequest
        {
            Username = username,
            Password = password
        };
        string json = JsonSerializer.Serialize(loginRequest);
        return PacketSerializer.Serialize((ushort)loginRequest.PacketId, json);
    }

    public ArraySegment<byte> EnterWorldRequest(int characterId)
    {
        EnterWorldRequest enterWorldRequest = new EnterWorldRequest
        {
            CharacterId = characterId
        };
        string json = JsonSerializer.Serialize(enterWorldRequest);
        return PacketSerializer.Serialize((ushort)enterWorldRequest.PacketId, json);
    }

    public ArraySegment<byte> MakeUseItemBuffer(int inventoryId)
    {
        var useItemPacket = new UseItemRequest
        {
            InventoryId = inventoryId,
        };
        string json = JsonSerializer.Serialize(useItemPacket);
        return PacketSerializer.Serialize((ushort)useItemPacket.PacketId, json);
    }

    public ArraySegment<byte> MakeDropItemBuffer(int inventoryId, int mapId, float posX, float posY, float posZ)
    {
        var dropItemPacket = new DropItemRequest
        {
            InventoryId = inventoryId,
            MapId = mapId,
            PosX = posX,
            PosY = posY,
            PosZ = posZ,
        };
        string json = JsonSerializer.Serialize(dropItemPacket);
        return PacketSerializer.Serialize((ushort)dropItemPacket.PacketId, json);
    }

    public ArraySegment<byte> Move(float posX, float posY, float posZ, int dir, int state)
    {
        var playerMoveRequest = new PlayerMoveRequest
        {
            CharacterId = Managers.Object.MyPlayer.CharacterId,
            PosX = posX,
            PosY = posY,
            PosZ = posZ,
            Dir = dir,
            State = state
        };
        string json = JsonSerializer.Serialize(playerMoveRequest);
        return PacketSerializer.Serialize((ushort)playerMoveRequest.PacketId, json);
    }

    public ArraySegment<byte> MoveMap(int targetMapId, float targetPosX, float targetPosY)
    {
        var moveMapRequest = new MoveMapRequest
        {
            CharacterId = Managers.Object.MyPlayer.CharacterId,
            TargetMapId = targetMapId,
            TargetPosX = targetPosX,
            TargetPosY = targetPosY
        };
        string json = JsonSerializer.Serialize(moveMapRequest);
        return PacketSerializer.Serialize((ushort)moveMapRequest.PacketId, json);
    }

    public ArraySegment<byte> PickUpItem(int mapId, int inventoryId)
    {
        var pickUpItemRequest = new PickUpItemRequest
        {
            MapId = mapId,
            InventoryId = inventoryId
        };
        string json = JsonSerializer.Serialize(pickUpItemRequest);
        return PacketSerializer.Serialize((ushort)pickUpItemRequest.PacketId, json);
    }

    public ArraySegment<byte> PlayerTakeDamage(int characterId, int damage)//, float knockbackX, float knockbackY)
    {
        var playerTakeDamageRequest = new PlayerTakeDamageRequest
        {
            CharacterId = characterId,
            Damage = damage,
            //KnockbackX = knockbackX,
            //KnockbackY = knockbackY
        };
        string json = JsonSerializer.Serialize(playerTakeDamageRequest);
        return PacketSerializer.Serialize((ushort)playerTakeDamageRequest.PacketId, json);
    }

    public ArraySegment<byte> PlayerHit(int characterId, int spawnId, int AttackType, int damage, int knockbackDir)
    {
        var playerHitRequest = new PlayerHitRequest
        {
            CharacterId = characterId,
            SpawnId = spawnId,
            AttackType = AttackType,
            Damage = damage,
            KnockbackDir = knockbackDir
        };
        string json = JsonSerializer.Serialize(playerHitRequest);
        return PacketSerializer.Serialize((ushort)playerHitRequest.PacketId, json);
    }

    /*public ArraySegment<byte> Dialogue(int npcId, int dialogueId)
    {
        var DialogueRequest = new DialogueRequest
        {
            NpcId = npcId,
            DialogueId = dialogueId
        };
        string json = JsonSerializer.Serialize(DialogueRequest);
        return PacketSerializer.Serialize((ushort)DialogueRequest.PacketId, json);
    }*/
    public ArraySegment<byte> NpcTalk(int npcId, int type, int dialogueId, int questId)
    {
        var npcTalkRequest = new NpcTalkRequest
        {
            NpcId = npcId,
            Type = type,
            DialogueId = dialogueId,
            QuestId = questId
        };
        string json = JsonSerializer.Serialize(npcTalkRequest);
        return PacketSerializer.Serialize((ushort)npcTalkRequest.PacketId, json);
    }

    /*public ArraySegment<byte> Quest(int npcId, int questId)
    {
        var questRequest = new QuestAcceptRequest
        {
            NpcId = npcId,
            QuestId = questId
        };
        string json = JsonSerializer.Serialize(questRequest);
        return PacketSerializer.Serialize((ushort)questRequest.PacketId, json);
    }*/
}