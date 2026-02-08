using UnityEngine;
using UnityEngine.UI;

public class LoginUi : MonoBehaviour
{
    public InputField idInput; //TMP_InputField 이후에 변경할 것
    public InputField pwInput;
    //public GameObject LoginPanel;
    //public GameObject RespwanPanel;
    public GameObject mainCamera;

    public GameObject characterSelectUi;

    public void Awake()
    {
        PacketHandler.Instance.OnLoginResponse += OnLoginRes;
        PacketHandler.Instance.OnGetCharacterListResponse += OnCharListRes;
    }

    public void OnClickLogin()
    {
        var loginRequestBuff = PacketMaker.Instance.LoginRequest(idInput.text, pwInput.text);
        NetworkManager.Instance.SendPacket(loginRequestBuff);
    }

    private void OnLoginRes(bool success)
    {
        if (success)
        {
            Debug.Log("로그인 성공, 캐릭터 목록 대기 중");
        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }

    private void OnCharListRes(GetCharacterListResponse res)
    {
        Debug.Log("캐릭터 목록 수신 완료");

        characterSelectUi.SetActive(false); //임시로 사용.
        characterSelectUi.SetActive(true);

        Vector3 TargetPos;
        TargetPos = mainCamera.transform.localPosition;
        TargetPos.x += 3200;
        mainCamera.transform.localPosition = TargetPos;
    }

    private void OnDestroy()
    {
        PacketHandler.Instance.OnLoginResponse -= OnLoginRes;
        PacketHandler.Instance.OnGetCharacterListResponse -= OnCharListRes;
    }
}