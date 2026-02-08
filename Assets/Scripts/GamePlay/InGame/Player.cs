public class MyPlayer
{
    public int CharacterId { get; set; }
    //public int MapId { get; set; }

    public PlayerController PlayerController { get; set; }
    public PlayerInfo PlayerInfo { get; set; }

    public Inventory Inventory { get; set; } = new Inventory();

    public MyPlayer(CharacterInfo character)
    {
        CharacterId = character.CharacterId;
        //MapId = character.Map;
        Inventory = new Inventory();
    }
}

/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Player 정보
//공통(맵에 보이는): 이름, 레벨, 외형(남/녀), 착용장비, 위치 좌표
//플레이어 본인 : 경험치, 인벤토리, 퀵슬롯(UI로 빼는 게 나을 듯), 퀘스트, 스킬, 능력치

public class Player
{
    //DB 조회용
    public int CharacterId { get; set; }

    //공통 플레이어 정보(생성시 1회 또는 필요시 업데이트)
    public string Nickname { get; set; }
    public int ClassId { get; set; }
    public int Level { get; set; }
    public bool Gender { get; set; } //외형(체형) 클래스 추가시 변경 필요
    public EquipmentInfo Equipment { get; set; }

    //위치 좌표 정보(실시간 업데이트)
    public int Hp { get; set; } //파티원 체력 표시
    public int MaxHp { get; set; } //나중에 설정 현재는 100이라고 가정
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

    public Player(PlayerInfo playerInfo)
    {
        CharacterId = playerInfo.CharacterId;
        Nickname = playerInfo.Nickname;
        ClassId = playerInfo.ClassId;
        PosX = playerInfo.PosX;
        PosY = playerInfo.PosY;
        PosZ = playerInfo.PosZ;
    }

    protected Player() { }

    public void OnHpHeal(int hp) {
        if (Hp + hp > 100)
            Hp = 100;
        else
            Hp += hp;

        //UI업데이트 이벤트 호출 -> 시스템 ui(싱글톤)에서 자동 관리중
    }
}

public class MyPlayer : Player
{
    //DB 조회용
    public int UserId { get; set; }

    //플레이어 본인 정보(필요시 업데이트)
    public int Mp { get; set; }
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Luk { get; set; }
    //public int ClassId { get; set; }
    public int Exp { get; set; }
    public int Map { get; set; }
    public Inventory Inventory { get; set; }
    public Skill Skill { get; set; }
    public Quest Quest { get; set; }

    public MyPlayer(CharacterInfo character)
    {
        Inventory = new Inventory();
        Skill = new Skill();
        Quest = new Quest();

        CharacterId = character.CharacterId;
        UserId = character.UserId;
        Nickname = character.Nickname;
        ClassId = character.ClassId;
        Level = character.Level;
        Exp = character.Exp;
        Map = character.Map;
        PosX = character.Pos_x;
        PosY = character.Pos_y;
        PosZ = character.Pos_z;

        Hp = character.Hp;
        MaxHp = character.MaxHp;
    }

    *//*public void SendMoveRequest()
    {
        var playerMoveRequestBuff = PacketMaker.Instance.Move(this.PosX, this.PosY, this.PosZ);
        NetworkManager.Instance.SendPacket(playerMoveRequestBuff);
    }*//*
}*/