/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EnterWorldUi : MonoBehaviour
{
    //public static UiManager Instance { get; } = new UiManager();

    public InputField idInput;
    public InputField pwInput;
    public GameObject LoginPanel;
    public GameObject RespwanPanel;

    public async Task<bool> EnterWorld(int characterId)
    {
        var enterWorldTcs = new TaskCompletionSource<EnterWorldResponse>();
        var inventoryTcs = new TaskCompletionSource<InventoryResponse>();

        Action<EnterWorldResponse> enterWorldHandler = res => enterWorldTcs.TrySetResult(res);
        Action<InventoryResponse> inventoryHandler = res => inventoryTcs.TrySetResult(res);

        PacketHandler.Instance.OnEnterWorldResponse += enterWorldHandler;
        PacketHandler.Instance.OnInventoryResponse += inventoryHandler;

        try
        {
            //NetworkManager.Instance.Send(new EnterWorldRequest { CharacterId = characterId });
            var enterWorldRequestBuff = PacketMaker.Instance.EnterWorldRequest(characterId);
            NetworkManager.Instance.SendPacket(enterWorldRequestBuff);

            var enterWorldResponse = await enterWorldTcs.Task;
            if (!enterWorldResponse.Success)
            {
                Console.WriteLine("게임 입장 실패");
                return false;
            }

            StateManager.Instance.EnterWorld = true;
            StateManager.Instance.CurrentState = StateType.EnterWorld;
            
            //플레이어 객체 생성 및 오브젝트 매니저에 등록
            MyPlayer myPlayer = new MyPlayer(enterWorldResponse.Character);
            ObjectManager.Instance.MyPlayer = myPlayer;

            //이거 때문에 코드가 꼬임. Map 객체는 오브젝트 매니저에서 new로 받아놓는게 아니라 실제 게임 오브젝트인데 잘못 됨.
            //ObjectManager.Instance.Map.MapId = enterWorldResponse.Character.Map;

            //인벤토리 불러오기
            var inventoryResponse = await inventoryTcs.Task;
            if (inventoryResponse.Items != null)
            {
                ObjectManager.Instance.MyPlayer.Inventory.InitInventory(inventoryResponse.Items);
                Console.WriteLine("인벤토리 초기화 완료");
            }

            //플레이어 맵 및 위치 설정
            //맵 매니저 객체 필요
        }
        finally
        {
            PacketHandler.Instance.OnEnterWorldResponse -= enterWorldHandler;
        }

        return true;
    }
}*/