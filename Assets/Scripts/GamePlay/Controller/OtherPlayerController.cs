using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    private PlayerInfo _playerInfo;
    private SPUM_Prefabs _spum;
    private PlayerUi _playerUi;

    public bool _isDeath = false;

    public Rigidbody2D _rb;

    public SPUM_Prefabs spum;
    Dictionary<PlayerState, int> _indexPair = new();

    private int _lastState;

    public float _lerpSpeed = 20f;

    private Vector3 _targetPos;
    public int dir = 0;
    public int state = 0;

    //float currentHp;

    /*public float moveSpeed = 5f;
    public float jumpForce = 13f;
    public GroundCheck groundCheck;*/

    public void Init(PlayerInfo playerinfo, SPUM_Prefabs spum, PlayerUi playerUi)
    {
        _playerInfo = playerinfo;
        _spum = spum;
        _playerUi = playerUi;

        if (_playerInfo.State == 9) OnDeath();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        //_rb.gravityScale = 3f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.interpolation = RigidbodyInterpolation2D.None;

        if (spum == null) spum = GetComponentInChildren<SPUM_Prefabs>();
        _indexPair = Managers.CharacterData.GetIndexPair();

        _targetPos = transform.position;
    }

    private void Update()
    {
        if (state == 9)
        {
            _spum.PlayAnimation(PlayerState.DEATH, _indexPair[PlayerState.DEATH]);
            return;
        }

        //hp ui 업데이트 -> 시스템ui에서 자동관리함
        //SystemUi.Instance.SetHp(Managers.Object.MyPlayer.Hp);

        transform.position = Vector3.Lerp(transform.position, _targetPos, _lerpSpeed * Time.deltaTime);

        var s = spum.transform.localScale;
        s.x = Mathf.Abs(s.x) * (dir < 0 ? 1 : -1);
        spum.transform.localScale = s;

        if (_lastState != state)
        {
            UpdateAnimationState();
            _lastState = state;
        }
    }

    /*public enum PlayerState
    {
        IDLE,
        MOVE,
        ATTACK,
        DAMAGED,
        DEBUFF,
        DEATH,
        OTHER,
    }*/

    public void UpdateAnimationState()
    {
        switch (state)
        {
            case 0:
                spum.PlayAnimation(PlayerState.IDLE, _indexPair[PlayerState.IDLE]);
                break;
            case 1:
                spum.PlayAnimation(PlayerState.MOVE, _indexPair[PlayerState.MOVE]);
                break;
            case 2:
                spum.PlayAnimation(PlayerState.ATTACK, _indexPair[PlayerState.ATTACK]);
                break;
            case 3:
                spum.PlayAnimation(PlayerState.DAMAGED, _indexPair[PlayerState.DAMAGED]);
                break;
            default:
                spum.PlayAnimation(PlayerState.IDLE, _indexPair[PlayerState.IDLE]);
                break;
        }
    }

    public void OnUpdateMove(PlayerMoveResponse res)
    {
        if (res.State == 3)
            Debug.Log("333333333333333");
        _targetPos = new Vector3(res.PosX, res.PosY, res.PosZ);
        dir = res.Dir;
        state = res.State;
    }

    public void OnTakeDamage(PlayerTakeDamageResponse res)
    {
        //if (_isDeath || _isInvincible) return;

        //hp 감소
        //_myPlayer.Hp = hp;
        _playerInfo.Hp = res.Hp;

        GameObject prefab = Managers.Resource.GetUi("DamageText");
        GameObject go = Managers.Pool.Pop(prefab, this.transform);

        go.transform.position = transform.position + Vector3.up * 2.0f;
        Color color = Color.red;
        go.GetComponent<DamageUi>().Init(res.Damage, color);
    }

    public void OnDeath()
    {
        Debug.Log("OnDeath");
        state = 9;
        _playerInfo.Hp = 0;

        _spum.gameObject.SetActive(false);
        _playerUi.SetDeathOn();
    }
}