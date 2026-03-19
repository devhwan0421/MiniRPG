using Protocol;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    private PlayerInfo _playerInfo;
    private SPUM_Prefabs _spum;
    private PlayerUi _playerUi;
    public Rigidbody2D _rb;
    Dictionary<PlayerState, int> _indexPair = new();

    public bool _isDeath = false;

    private int _lastState;
    public float _lerpSpeed = 7f;

    private Vector3 _targetPos;
    private Vector3 _currentVelocity;
    public int _dir = 0;
    public int _state = 0;

    private struct PendingPacket
    {
        public PlayerMoveResponseProto Packet;
        public float Latency;
    }

    private Queue<PendingPacket> _packetQueue = new Queue<PendingPacket>();

    private float _bufferTimer = 0f;
    private const float BUFFER_DELAY = 0.133f;
    private bool _isBuffering = true; // 처음에 패킷을 모으는 상태인지 체크

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
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.interpolation = RigidbodyInterpolation2D.None;

        if (_spum == null) _spum = GetComponentInChildren<SPUM_Prefabs>();
        _indexPair = Managers.CharacterData.GetIndexPair();

        _targetPos = transform.position;
    }

    private void Update()
    {
        if (_state == 9) return;

        //버퍼체크
        if (_isBuffering)
        {
            if (_packetQueue.Count >= 2) //버퍼에 데이터가 충분히 들어오면 다음 로직 실행
            {
                _isBuffering = false;
            }
            else
            {
                Move(); //3. 중간에 지연으로 패킷이 없어도 진행 방향으로 예측 이동
                return;
            }
        }

        //delay: 133ms 마다 큐에 쌓인 데이터를 꺼내어 이동 처리
        _bufferTimer += Time.deltaTime;
        float dynamicDelay = (_packetQueue.Count >= 3) ? BUFFER_DELAY * 0.5f : BUFFER_DELAY;

        if (_packetQueue.Count > 0 && _bufferTimer >= dynamicDelay)
        {
            ProcessNextPacket(_packetQueue.Dequeue());
            _bufferTimer = 0f;
        }

        //버퍼에 데이터가 없으면 예측 지점으로 이동하며 패킷 대기
        if (_packetQueue.Count == 0) _isBuffering = true;

        Move(); //목적지로 이동
    }

    private void Move()
    {
        if (_state != 0 && _state != 9) //이동중일 경우 패킷이 없어도 미리 예측 이동
            _targetPos += _currentVelocity * Time.deltaTime; //예측 이동

        //선형 보간
        transform.position = Vector3.Lerp(transform.position, _targetPos, _lerpSpeed * Time.deltaTime);

        UpdateDir(); //방향 업데이트
        UpdateAnimationState(); //상태값에 따른 애니메이션 재생
    }

    //큐에 담긴 패킷을 꺼내어 처리하는 함수
    private void ProcessNextPacket(PendingPacket pending)
    {
        var res = pending.Packet;
        _currentVelocity = new Vector3(res.Vx, res.Vy, 0);
        _dir = res.Dir;
        _state = res.State;

        float latency = pending.Latency; //도착 시점에 측정했던 지연시간

        Vector3 serverPosAtTime = new Vector3(res.PosX, res.PosY, 0);

        //속도와 지연시간으로 예측 위치 계산
        Vector3 predictedPos = serverPosAtTime + (_currentVelocity * latency);

        if (_state == 0) //멈췄을 경우
        {
            _targetPos = serverPosAtTime; //즉시 멈춘위치로 좌표 설정
            _currentVelocity = Vector3.zero; //속도값 0으로 초기화
        }
        else
        {
            _targetPos = predictedPos;
        }
    }

    //패킷을 받아 지연시간을 계산 후 큐에 저장
    public void OnUpdateMove(PlayerMoveResponseProto res)
    {
        long currentServerTime = Managers.Network.GetServerTime();
        
        //유니티 이동 로직이 초단위이므로 변환
        float networkLatency = (currentServerTime - res.TimeStamp) / 10000000f;
        networkLatency = Mathf.Max(0, networkLatency);

        _packetQueue.Enqueue(new PendingPacket
        {
            Packet = res,
            Latency = networkLatency
        });
    }

    private void UpdateDir()
    {
        var s = _spum.transform.localScale;
        s.x = Mathf.Abs(s.x) * (_dir < 0 ? 1 : -1);
        _spum.transform.localScale = s;
    }

    public void UpdateAnimationState()
    {
        if (_lastState == _state) return;

        _lastState = _state;

        switch (_state)
        {
            case 0:
                _spum.PlayAnimation(PlayerState.IDLE, _indexPair[PlayerState.IDLE]);
                break;
            case 1:
                _spum.PlayAnimation(PlayerState.MOVE, _indexPair[PlayerState.MOVE]);
                break;
            case 2:
                _spum.PlayAnimation(PlayerState.ATTACK, _indexPair[PlayerState.ATTACK]);
                break;
            case 3:
                _spum.PlayAnimation(PlayerState.DAMAGED, _indexPair[PlayerState.DAMAGED]);
                break;
            default:
                _spum.PlayAnimation(PlayerState.IDLE, _indexPair[PlayerState.IDLE]);
                break;
        }
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
        _state = 9;
        _playerInfo.Hp = 0;

        _spum.gameObject.SetActive(false);
        _playerUi.SetDeathOn();
    }
}