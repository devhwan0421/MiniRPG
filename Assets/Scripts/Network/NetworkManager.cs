using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public string host = "127.0.0.1";
    public int port = 7777;

    private static NetworkManager _instance;
    Socket _socket;
    NetworkStream _stream;
    private CancellationTokenSource _cts;   //취소토큰을 생성하고 관리하는데 사용. Task 비동기 작업 내에서 작업을 취소하거나 취소 상태 확인
    public bool IsConnected => _socket != null && _socket.Connected;

    Task _recvTask, _sendTask;
    private ConcurrentQueue<ArraySegment<byte>> _sendQ = new ConcurrentQueue<ArraySegment<byte>>();
    private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

    private readonly byte[] _recvBuffer = new byte[65536];

    private readonly ConcurrentQueue<Action> _mainQ = new();       // 메인 스레드에서 실행할 작업 큐

    //이벤트 (Unity 메인 스레드에서 호출)
    //public event Action OnConnected;
    //public event Action OnDisconnected;

    public PacketHandler Handler { get; set; }

    public static NetworkManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindAnyObjectByType<NetworkManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);  // 씬이 바뀌어도 파괴되지 않게
        Application.runInBackground = true;  // 포커스 없어도 Update 돌려
        Application.targetFrameRate = 60;    // 프레임레이트 고정
        Screen.SetResolution(960, 540, false);
    }

    void Update()
    {
        PacketQueue.Instance.PopAll();
    }

    void EnqueueMain(Action action) => _mainQ.Enqueue(action);

    public async Task<bool> Connect()
    {
        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _cts = new CancellationTokenSource();

            //비동기 연결
            await _socket.ConnectAsync(IPAddress.Parse(host), port);
            _stream = new NetworkStream(_socket, true);

            //Console.WriteLine($"[Client] {host}:{port} 서버 연결 성공");
            Debug.Log($"[Client] {host}:{port} 서버 연결 성공");

            _recvTask = Task.Run(() => RecvLoop(_cts.Token));
            _sendTask = Task.Run(() => SendLoop(_cts.Token));

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Client] 서버 연결 실패: {e.Message}");
            return false;
        }
    }

    private async Task RecvLoop(CancellationToken token)
    {
        int currentLength = 0;

        try
        {
            while (!token.IsCancellationRequested && IsConnected)
            {
                //_recvBuffer.Length - currentLength
                int n = await _stream.ReadAsync(_recvBuffer, currentLength, _recvBuffer.Length - currentLength, token);
                Debug.Log($"받은 패킷양: {n}");
                if (n <= 0) break;

                currentLength += n;
                int processed = 0;

                while(currentLength - processed >= 4)
                {
                    ushort size = BitConverter.ToUInt16(_recvBuffer, processed);
                    ushort id = BitConverter.ToUInt16(_recvBuffer, processed + 2);

                    if (currentLength - processed < size) break;

                    string json = Encoding.UTF8.GetString(_recvBuffer, processed + 4, size - 4);

                    Debug.Log($"[Client] <<< Packet Received: ID={id}, Size={size}, JSON={json}");

                    //유니티 메인 스레드 큐로 넘김. 현재 update문에서 하나씩 처리중
                    PacketQueue.Instance.Push(()=> Handler.OnRecvPacket((PacketID)id, json));
                    processed += size;
                }

                int remain = currentLength - processed;
                if(remain > 0)
                {
                    Array.Copy(_recvBuffer, processed, _recvBuffer, 0, remain);
                    currentLength = remain;
                }
                else
                {
                    currentLength = 0;
                }
            }
        }
        catch (OperationCanceledException)
        {
            //정상 종료, 토큰
            //Console.WriteLine("[Client] 수신 루프 종료 요청으로 종료됨");
            Debug.Log("[Client] 수신 루프 종료 요청으로 종료됨");
        }
        catch (Exception e)
        {
            //Console.WriteLine($"[Client] 수신 루프 오류: {e.Message}");
            Debug.Log($"[Client] 수신 루프 오류: {e.Message}");
        }
        finally
        {
            Disconnect();
        }
    }

    public void SendPacket(ArraySegment<byte> packet)
    {
        _sendQ.Enqueue(packet);
        _signal.Release();
    }

    public async Task SendLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await _signal.WaitAsync(token);

                List<ArraySegment<byte>> sendList = new List<ArraySegment<byte>>();
                while (_sendQ.TryDequeue(out var packet))
                {
                    sendList.Add(packet);
                }

                if(sendList.Count > 0)
                {
                    foreach (var buffer in sendList)
                    {
                        await _stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, token);
                    }
                    await _stream.FlushAsync(token);
                }
            }
        }
        catch (OperationCanceledException)
        {
            //정상 종료, 토큰
            Console.WriteLine("[Client] 정상 종료");
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Client] 송신 루프 오류: {e.Message}");
        }
    }

    public void Disconnect()
    {
        if(_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }
        _signal.Release();
        try
        {
            if(_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
            if (_socket != null)
            {
                if (_socket.Connected) _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        _cts = null;
        Debug.Log("[Client] 서버 연결 종료");
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private void OnDestroy()
    {
        Disconnect();
    }





























    /*public void SendJson(ushort op, object obj)
    {
        var json = JsonUtility.ToJson(obj); // C# 객체를 JSON 문자열로 직렬화
        var body = Encoding.UTF8.GetBytes(json); // 문자열 바이트 배열로

        var len = 2 + body.Length;                // opcode(2) + payload
        var pkt = new byte[4 + len];

        WriteU32(pkt, 0, (uint)len);              // [0..3]에 len 기록 (리틀엔디안)
        WriteU16(pkt, 4, op);                     // [4..5]에 opcode 기록 (리틀엔디안)
        Buffer.BlockCopy(body, 0, pkt, 6, body.Length); // 다음 칸에 payload 복사

        _sendQ.Enqueue(pkt);    // 송신 큐에 넣고
        _sendSignal.Set();      // 송신 스레드 깨우기

        //Debug.Log($"[Net] >>> op={op}, body={body.Length}, total={pkt.Length}, json={json}");
    }*/


    //[Header("Endpoint")]
    //public string host = "127.0.0.1";
    //public int port = 7777;

    /*public const ushort OP_LOGIN = 1;
    public const ushort OP_LOGIN_OK = 2;
    public const ushort OP_INPUT = 3;
    public const ushort OP_STATE = 4;
    public const ushort OP_PING = 100;
    public const ushort OP_PONG = 101;

    public const ushort OP_MSTATE = 10; // 몬스터 상태 배열 방송
    public const ushort OP_HIT = 11; // 클라→서버, 내가 몬스터에 맞았다고 신고(또는 내가 맞췄다)
    //public const ushort OP_HURT = 12; // 서버→클라, 데미지/넉백 적용 지시

    public const ushort OP_CPOSE = 201;  // Client -> Server: 내 좌표

    
    public event Action<ushort, ArraySegment<byte>> Onpacket; // opcode, payload*/

    //Socket _sock;
    //NetworkStream _ns;



    //readonly MemoryStream _in = new(64 * 1024);     
    //readonly byte[] _buf = new byte[32 * 1024];










    /*void OnEnable() {
        //Connect();
    }
    void OnDisable() => Close();

    public void SendLogin(string id, string nickname)
    {
        var req = new LoginReq { userid = id,  nickname = nickname };
        var json = JsonUtility.ToJson(req);
        //Debug.Log($"[Net] >>> LOGIN json={json}");
        SendJson(OP_LOGIN, req);   // OP_LOGIN = 1
    }

    [Serializable]
    public class ClientPose
    {
        public int id;    // myId
        public float x, y;
        public float vx, vy;
        public int dir;
        public int state;
    }

    public void SendPose(int id, Vector2 pos, Vector2 vel, int dir, int state)
    {
        if (!IsConnected) return;
        var p = new ClientPose { id = id, x = pos.x, y = pos.y, vx = vel.x, vy = vel.y, dir = dir, state = state };
        SendJson(OP_CPOSE, p);
    }

    public void SendInput(int seq, int ax, int ay, bool jump)
    {
        var req = new InputReq { seq = seq, ax = ax, ay = ay, jump = jump };
        var json = JsonUtility.ToJson(req);
        //Debug.Log($"[Net] >>> INPUT seq={seq} ax={ax} ay={ay} jump={jump} json={json}");
        SendJson(NetworkManager.OP_INPUT, req);
    }*/





    /*// 리틀엔디안으로 변환
    static void WriteU32(byte[] b, int o, uint v) {
        b[o] = (byte)v; //v는 32비트이나 byte 형변환하였기 때문에 마지막 8비트만 읽음
        b[o + 1] = (byte)(v >> 8);
        b[o + 2] = (byte)(v >> 16);
        b[o + 3] = (byte)(v >> 24);
    }

    static void WriteU16(byte[] b, int o, ushort v)
    {
        b[o] = (byte)v;
        b[o + 1] = (byte)(v >> 8);
    }

    static uint ReadU32(Stream s)
    {
        int b0 = s.ReadByte(), b1 = s.ReadByte(), b2 = s.ReadByte(), b3 = s.ReadByte();
        return (uint)(b0 | (b1 << 8) | (b2 << 16) | (b3 << 24));
    }

    static ushort ReadU16(Stream s)
    {
        int b0 = s.ReadByte(), b1 = s.ReadByte();
        return (ushort)(b0 | (b1 << 8));
    }

    static byte[] ReadBytes(Stream s, int len) //스트림에서 지정한 길이만큼 바이트를 읽음
    {
        var r = new byte[len];
        int off = 0;
        while(off < len) // 다 읽을 때까지 반복. 읽다가 끊길 수 있어서 이런 구조를 쓴다함.
        {
            int n = s.Read(r, off, len - off);
            if (n <= 0) throw new EndOfStreamException();
            off += n;
        }
        return r;
    }*/

    /*[Serializable]
    public class LoginReq {
        public string userid;
        public string nickname; 
    }

    [Serializable]
    public class InputReq
    {
        public int seq;
        public int ax;
        public int ay;
        public bool jump;
    }

    [Serializable]
    public class MonsterInfo
    {
        public int id;
        public float x;
        public float y;
        public int dir;
        public int hp;
    }

    [Serializable]
    public class Hurt
    {
        public int victimId;
        public int damage;
        public float knockX;    // 넉백 벡터
        public float knockY;
        public float invSec;    // 무적시간
    }*/
}