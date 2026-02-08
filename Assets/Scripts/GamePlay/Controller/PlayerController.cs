using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool _isDeath = false;

    //MyPlayer _myPlayer = Managers.Object.MyPlayer; //매니저에 넣지말고 _playerinfo를 그냥 컨트롤러로만 받아 통합할까?
                                                    //그러기엔 스탯정보나 이런 정보를 가지고 있을 곳이 필요

    public Rigidbody2D _rb;

    private SPUM_Prefabs _spum;
    private PlayerInfo _playerInfo;
    private PlayerUi _playerUi;

    Dictionary<PlayerState, int> _indexPair = new();

    public int dir = 0;
    public int state = 0;
    private bool _isInvincible = false;
    private bool _isKnockback = false;
    private float _knockbackSec = 0.25f;

    private float _invincibleTimer;
    private float _invincibleSec = 1.0f;

    public float moveSpeed = 5f;
    public float jumpForce = 13f;
    public GroundCheck groundCheck;

    private Vector3 _lastSentPos; //마지막으로 서버에 보낸 위치
    private int _lastState;

    private float _attackTimer;
    private bool _isAttacking = false;
    public float _attackSec = 0.5f;

    public void Init(PlayerInfo playerinfo, SPUM_Prefabs spum, PlayerUi playerUi)
    {
        _playerInfo = playerinfo;
        _spum = spum;
        _playerUi = playerUi;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 3f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //if(spum == null) spum = GetComponent<SPUM_Prefabs>();
        //if (spum == null) spum = GetComponentInChildren<SPUM_Prefabs>();
        //spum.OverrideControllerInit(); //플레이어 맵에 스폰할 때 Mapmanager에서 이미 호출함

        _indexPair = Managers.CharacterData.GetIndexPair();
        /*foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            _indexPair[state] = 0;
        }*/
        //_spum.PlayAnimation(PlayerState.ATTACK, _indexPair[PlayerState.ATTACK]);

        if (groundCheck == null) groundCheck = GetComponentInChildren<GroundCheck>();

        //_indexPair = Managers.CharacterData.GetIndexPair();
    }

    private void Update()
    {
        //플레이어 Death
        if (state == 9) {
            return; 
        }

        Hit();

        _isInvincible = (Time.time < _invincibleTimer);
        _isAttacking = (Time.time < _attackTimer);

        if (!_isKnockback && !_isAttacking)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            //float vertical = Input.GetAxisRaw("Vertical");
            bool jump = Input.GetKeyDown(KeyCode.LeftAlt);

            // 플레이어 이동 처리 로직
            if (horizontal != 0)
            {
                Vector3 movement = new Vector3(horizontal, 0, 0).normalized;
                transform.position += movement * moveSpeed * Time.deltaTime;

                _playerInfo.PosX = transform.position.x;
                _playerInfo.PosY = transform.position.y;
                _playerInfo.PosZ = transform.position.z;

                dir = horizontal < 0 ? -1 : 1;
                state = 1;
                _spum.PlayAnimation(PlayerState.MOVE, _indexPair[PlayerState.MOVE]);

                var s = _spum.transform.localScale;
                s.x = Mathf.Abs(s.x) * (horizontal < 0 ? 1 : -1);
                _spum.transform.localScale = s;
                //Debug.Log($"s.x={s.x}, ax={ax}");
            }
            else
            {
                state = 0;
                _spum.PlayAnimation(PlayerState.IDLE, _indexPair[PlayerState.IDLE]);
            }

            if (jump && groundCheck.isGrounded)
            {
                //점프 애니메이션이 없다..
                _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        float distance = Vector3.Distance(_lastSentPos, transform.position);
        if (distance > 0.1f || state != _lastState)
        {
            var moveBuff = PacketMaker.Instance.Move(transform.position.x, transform.position.y, transform.position.z, dir, state);
            Managers.Network.SendPacket(moveBuff);
            _lastSentPos = transform.position;
            _lastState = state;
        }
    }

    public bool IsInvincible() => _isInvincible;

    public void OnKnockback(Vector2 knockbackForce)
    {
        //플레이어 오브젝트에 넉백 효과 적용
        if (_isDeath) return;

        StartCoroutine(CoKnockback(knockbackForce));
    }

    IEnumerator CoKnockback(Vector2 knockbackForce)
    {
        _isKnockback = true;

        //데미지 애니메이션
        _spum.PlayAnimation(PlayerState.DAMAGED, _indexPair[PlayerState.DAMAGED]);

        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        //무적 타이머
        _invincibleTimer = Time.time + _invincibleSec;

        float end = Time.time + _knockbackSec;
        while (Time.time < end)
            yield return new WaitForFixedUpdate();

        _isKnockback = false;
    }

    //최종 hp를 보내주게 변경할 것
    public void OnHpHeal(int hp)
    {
        if (_playerInfo.Hp + hp > 100)
            _playerInfo.Hp = 100;
        else
            _playerInfo.Hp += hp;

        //UI업데이트 이벤트 호출 -> 시스템 ui(싱글톤)에서 자동 관리중
    }

    public void OnTakeDamage(PlayerTakeDamageResponse res)
    {
        Debug.Log($"OnTakeDamage. {res.Damage}, {res.Hp}");
        //if (_isDeath || _isInvincible) return;
        //if (_isDeath) return;

        state = 3;
        _playerInfo.Hp = res.Hp;

        GameObject prefab = Managers.Resource.GetUi("DamageText");
        GameObject go = Managers.Pool.Pop(prefab, this.transform);
        go.transform.position = transform.position + Vector3.up * 2.0f;
        Color color = Color.red;
        go.GetComponent<DamageUi>().Init(res.Damage, color);
    }

    public bool IsDeath() => state == 9;

    public void OnDeath()
    {
        Debug.Log("OnDeath");
        state = 9;
        _playerInfo.Hp = 0;
        //_spum.PlayAnimation(PlayerState.DEATH, _indexPair[PlayerState.DEATH]);
        var moveBuff = PacketMaker.Instance.Move(transform.position.x, transform.position.y, transform.position.z, dir, state);
        Managers.Network.SendPacket(moveBuff);

        _spum.gameObject.SetActive(false);
        _playerUi.SetDeathOn();
    }

    private float _lastAttackTime;
    private float _attackCooldown = 0.5f;
    private void Hit()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (Time.time < _lastAttackTime + _attackCooldown) return;
            _spum.PlayAnimation(PlayerState.ATTACK, _indexPair[PlayerState.ATTACK]);

            state = 2;
            _isAttacking = true;
            //공격 패킷 전송

            //CheckMonsterHit();
            StartCoroutine(CoCheckHit());

            _lastAttackTime = Time.time;
        }
    }

    IEnumerator CoCheckHit()
    {
        HashSet<MonsterController> hitMonsters = new HashSet<MonsterController>();

        _attackTimer = Time.time + _attackSec;

        float end = Time.time + _attackSec;
        while (Time.time < end)
        {
            CheckMonsterHit(hitMonsters);
            yield return null; //null로 하면 프레임마다 확인
        }

        _isAttacking = false;
    }

    private void CheckMonsterHit(HashSet<MonsterController> hitMonsters)
    {
        Vector2 attackPos = (Vector2)transform.position + new Vector2(dir * 1.0f, 0.5f);
        float attackRadius = 1.0f;

        LayerMask monsterLayer = LayerMask.GetMask("Monster");
        Collider2D[] targets = Physics2D.OverlapCircleAll(attackPos, attackRadius, monsterLayer);

        //HashSet<MonsterController> hitMonsters = new HashSet<MonsterController>();

        foreach (Collider2D target in targets)
        {
            MonsterController monster = target.GetComponentInParent<MonsterController>();

            if (monster != null && !hitMonsters.Contains(monster))
            {
                hitMonsters.Add(monster);

                int knockbackDir = target.transform.position.x > transform.position.x ? 1 : -1;

                var playerHitRequestBuff = PacketMaker.Instance.PlayerHit(_playerInfo.CharacterId, monster.GetMonsterId(), 0, 100, knockbackDir);
                Managers.Network.SendPacket(playerHitRequestBuff);

                monster.OnKnockback(knockbackDir);

                Debug.Log($"{target.gameObject.name} 타격 성공");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 attackCenter = transform.position + new Vector3(dir * 1.0f, 0.5f, 0);
        float attackRadius = 1.0f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCenter, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackCenter, 0.05f);
    }

    public void OnPlayerLevelUp(int level, int damage)
    {
        _playerInfo.Level = level;
        _playerInfo.Damagae = damage;
        _playerUi.SetLevel(level);
    }

    /*IEnumerator CoInvincible()
    {
        _isInvincible = true;

        _invincibleTimer = Time.time + _invincibleSec;

        while(Time.time < _invincibleTimer)
            yield return new WaitForFixedUpdate();
    }*/

    /*public void ApplyHurt(int damage, Vector2 knock)
    {
        if (isDeath) return;

        StartCoroutine(CoKnockback(knock));
        ApplyDamage(damage);
    }

    IEnumerator CoKnockback(Vector2 knock)
    {
        _knockback = true;
        spum.PlayAnimation(PlayerState.DAMAGED, IndexPair[PlayerState.DAMAGED]);

        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(knock, ForceMode2D.Impulse);

        // 무적 타이머
        _invUntil = Time.time + invincibleSec;

        // 대기
        float end = Time.time + knockDuration;
        while (Time.time < end)
            yield return new WaitForFixedUpdate();

        _knockback = false;
    }

    public void ApplyDamage(int dmg)
    {
        if (_invincible) return; // 무적 중이면 데미지 무시

        currentHp -= dmg;

        if (currentHp <= 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            isDeath = true;
            if (isMine)
                client.RespwanPanel.SetActive(true);
        }

        if (isMine)
        {
            float result = (float)dmg / (float)maxHp;
            Debug.Log($"ApplyDamage! dmg={dmg}, currentHp={currentHp}, maxHp={maxHp}, damage={result}");

            hpImage.fillAmount -= result;
        }
    }*/
}