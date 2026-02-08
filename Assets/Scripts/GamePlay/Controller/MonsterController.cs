using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public SPUM_Prefabs _spum;
    Dictionary<PlayerState, int> _indexPair = new();

    private MonsterInfo _monsterInfo;
    private MonsterUi _monsterUi;

    private float _spawnPosX, _spawnPosY;

    private Vector3 _targetPos;
    public float lerpSpeed = 10f;

    private MonsterAttack _monsterAttack;

    private bool _isDeath = true;

    public void Init(MonsterInfo monsterInfo, MonsterUi monsterUi)
    {
        _monsterInfo = monsterInfo;
        _monsterUi = monsterUi;

        _spawnPosX = monsterInfo.PosX;
        _spawnPosY = monsterInfo.PosY;

        _indexPair = Managers.CharacterData.GetIndexPair();

        transform.position = new Vector3(_monsterInfo.PosX, _monsterInfo.PosY, _monsterInfo.PosZ);
        _targetPos = transform.position;

        _monsterAttack = GetComponentInChildren<MonsterAttack>();
        if (_monsterAttack != null) _monsterAttack.Init(_monsterInfo.Damage);

        _isDeath = false;
    }

    //public int GetMonsterId() => _monsterInfo.MonsterId;
    //서버에서는 스폰 아이디로 관리하기로 변경
    public int GetMonsterId() => _monsterInfo.SpawnId;

    public void UpdateFlip()
    {
        if (_spum == null || _monsterInfo.State == 2) return;

        Vector3 scale = _spum.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (_monsterInfo.Dir < 0 ? 1 : -1);
        _spum.transform.localScale = scale;
    }

    public void UpdateAnimation()
    {
        if (_spum == null) return;

        switch (_monsterInfo.State)
        {
            case 0:
                _spum.PlayAnimation(PlayerState.IDLE, _indexPair[PlayerState.IDLE]);
                break;
            case 1:
                _spum.PlayAnimation(PlayerState.MOVE, _indexPair[PlayerState.MOVE]);
                break;
            case 2:
                //넉백
                break;
            case 3: //추격
                _spum.PlayAnimation(PlayerState.MOVE, _indexPair[PlayerState.MOVE]);
                break;
        }
    }

    public void OnUpdateMove(MonsterMoveResponse res)
    {
        if (_isDeath) return;
        // 데이터 갱신 (참조를 통해 _monsterInfo 내부 값도 바뀜)
        _monsterInfo.PosX = res.PosX;
        //_monsterInfo.PosY = res.PosY;
        //_monsterInfo.PosZ = res.PosZ;
        _monsterInfo.Dir = res.Dir;
        _monsterInfo.State = res.State;

        // 목적지 설정
        //_targetPos = new Vector3(res.PosX, transform.position.y, transform.position.z);

        // 방향과 애니메이션 즉시 업데이트
        UpdateFlip();
        UpdateAnimation();
    }

    private void Update()
    {
        if (_isDeath) return;

        _targetPos = new Vector3(_monsterInfo.PosX, transform.position.y, transform.position.z);

        // 매 프레임 목적지를 향해 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * lerpSpeed);
    }

    public void OnKnockback(int knockbackDir)
    {
        if (_isDeath) return;

        //맞으면 맞은 방향을 쳐다보고 쫓아가게 하고 싶음. 시간 남으면 할 것
        Vector3 scale = _spum.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * knockbackDir;
        _spum.transform.localScale = scale;

        _spum.PlayAnimation(PlayerState.DAMAGED, _indexPair[PlayerState.DAMAGED]);
    }

    public void OnTakeDamage(int hp, int damage)
    {
        if (_isDeath) return;

        _monsterInfo.Hp = hp;
        _monsterUi.UpdateHp(hp);

        GameObject prefab = Managers.Resource.GetUi("DamageText");
        GameObject go = Managers.Pool.Pop(prefab, this.transform);
        go.transform.position = transform.position + Vector3.up * 2.0f;
        Color color = Color.red;
        go.GetComponent<DamageUi>().Init(damage, color);
    }

    public void Despawn()
    {
        _isDeath = true;
        gameObject.SetActive(false);
    }

    public void Respawn(float spawnPosX, float spawnPosY)
    {
        Debug.Log($"x: {spawnPosX}, y: {spawnPosY}");
        _monsterInfo.PosX = spawnPosX;
        _monsterInfo.PosY = spawnPosY;

        _monsterUi.UpdateHp(_monsterInfo.MaxHp);

        transform.position = new Vector3(spawnPosX, spawnPosY, transform.position.z);
        gameObject.SetActive(true);
        _isDeath = false;
    }
}