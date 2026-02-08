using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterSelectUi : MonoBehaviour
{
    public CharacterSlot[] characterSlots;

    private void Awake()
    {
        PacketHandler.Instance.OnEnterWorldResponse += OnEnterWorldRes;
    }

    private void OnDestroy()
    {
        PacketHandler.Instance.OnEnterWorldResponse -= OnEnterWorldRes;
    }

    //상태 체크해서 로그인 이후 불러오도록. 그 전까지는 활성화 해도 아무것도 안 함
    private void OnEnable()
    {
        List<CharacterInfo> characterListInfo = ObjectManager.Instance.characterListInfo;
        if(characterListInfo != null)
        {
            int count = characterListInfo.Count;

            //캐릭터 수는 5개로 제한되어 있다고 가정
            foreach (var slot in characterSlots)
            {
                slot.Clear();
            }

            for (int i = 0; i < count; i++)
            {
                characterSlots[i].SetInfo(characterListInfo[i]);
            }
        }
    }

    public void OnClickEnterWorld()
    {
        int characterId = Managers.Object.SelectedCharacterId;

        var enterWorldRequestBuff = PacketMaker.Instance.EnterWorldRequest(characterId);
        Managers.Network.SendPacket(enterWorldRequestBuff);
    }

    public void OnEnterWorldRes(EnterWorldResponse res)
    {
        if (!res.Success) return;

        StateManager.Instance.EnterWorld = true;
        StateManager.Instance.CurrentState = StateType.EnterWorld;

        MyPlayer myPlayer = new MyPlayer(res.Character);
        Managers.Object.MyPlayer = myPlayer; //맵, 인벤토리까지 자동 세팅하도록 변경할 것

        //Managers.Object.Map.MapId = res.Character.Map;
        Managers.Map.MapId = res.Character.Map;

        Managers.Object.MyPlayer.Inventory.InitInventory(res.Inventory);

        //인벤토리 갱신 이벤트 등록
        Managers.Ui._inventoryUi.Init();
        Managers.Ui._inventoryUi.Refresh();

        //시스템 Ui 활성화. 임시
        Managers.Ui._systemUi.Active();

        //임시로 hp 10 설정
        //Managers.Object.MyPlayer.Hp = 10;

        //MapInfo 세팅
        Managers.Map.Init(res.MapInfo);

        //데미지텍스트 풀 생성
        GameObject damagePrefab = Managers.Resource.GetUi("DamageText");//Resources.Load<GameObject>("Prefabs/UI/DamageText");
        Managers.Pool.PreLoad(damagePrefab, 50);

        //씬 로딩
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map0"); //로딩 후 Map0에 만들어진 맵객체가 생성되면 스폰 하도록
    }
}