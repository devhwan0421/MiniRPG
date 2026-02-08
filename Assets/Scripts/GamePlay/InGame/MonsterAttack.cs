using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    //public MiniClient client;

    //public Monster monster;

    //public MonsterController _monsterController;

    private int _damage;

    public float knockbackX = 3.0f;
    public float knockbackY = 4.0f;

    void Awake()
    {
        //player = GetComponent<Player>();

        //_monsterController = GetComponent<MonsterController>();
        //_monsterController = GetComponentInParent<MonsterController>();
    }

    public void Init(int damage)
    {
        _damage = damage;
    }

    //내 클라이언트에서는 내 플레이어의 액션만 처리하면 되므로 다른 플레이어와의 부딪힘은 처리하지 않는다.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.CompareTag("MyPlayer"))) return;
        //if (!(other.gameObject.layer == LayerMask.NameToLayer("Player"))) return;

        var playerController = other.GetComponent<PlayerController>();
        if (playerController == null) return;

        //플레이어가 무적 상태일 경우 리턴
        if (playerController.IsInvincible() || playerController.IsDeath()) return;

        //플레이어가 몬스터를 기준으로 어느 쪽 방향에 있는지
        int dir = other.transform.position.x > transform.position.x ? 1 : -1;

        //넉백 수치 계산
        var knockbackForce = new Vector2(dir * knockbackX, knockbackY);

        //플레이어컨트롤러에 넉백 이벤트 적용
        playerController.OnKnockback(knockbackForce);

        //서버에 플레이어 넉백 요청 -> 맵에 해당 캐릭터 넉백이벤트 브로드캐스트
        //charid, damage, knockForce
        var playerTakeDamageBuff = PacketMaker.Instance.PlayerTakeDamage(Managers.Object.MyPlayer.CharacterId, _damage);//, dir * knockbackX, knockbackY);
        Managers.Network.SendPacket(playerTakeDamageBuff);
    }

    /*void OnTriggerEnter2D(Collider2D other) //
    {
        var player = other.GetComponent<Player>();
        if (player == null) return;
        if (player.IsInvincible() || client == null) return;

        int dir = other.transform.position.x > transform.position.x ? 1 : -1;

        var knock = new Vector2(dir * knockbackX, knockbackY);


        int objType = 0; // 0 : 플레이어, 1: 몬스터
        int objId = player.playerId;
        int objDir = dir;
        //int damage = 10;
        int damage = monster.damage;

        player.ApplyHurt(damage, knock);
        client.SendHitReq(objType, objId, objDir, damage); // 플레이어인지 몬스터인지, 넉백될 객체, 넉백될 방향, 입을 데미지
    }*/
}