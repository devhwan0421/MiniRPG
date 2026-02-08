using UnityEngine;

public class GameManager : MonoBehaviour
{
    private async void Start()
    {
        NetworkManager.Instance.Handler = PacketHandler.Instance;

        Debug.Log("서버 연결 시도");
        bool connected = await NetworkManager.Instance.Connect();

        if (connected)
        {
            Debug.Log("서버 연결 성공");
        }
        else
        {
            Debug.Log("서버 연결 실패");
        }
    }
}
