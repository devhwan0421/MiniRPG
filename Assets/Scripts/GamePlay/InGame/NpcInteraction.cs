using UnityEngine;

public class NpcInteraction : MonoBehaviour
{
    public int _npcId = 1;

    private bool _isPlayerOverlapping = false;

    private void Update()
    {
        if (_isPlayerOverlapping && Input.GetKeyDown(KeyCode.Space))
        {
            NpcTalkRequest();
        }
    }

    private void OnMouseDown()
    {
        NpcTalkRequest();
    }

    private void NpcTalkRequest()
    {
        Debug.Log("Dialogue 요청");
        var npcTalkBuff = PacketMaker.Instance.NpcTalk(_npcId, 0, 0, 0);
        Managers.Network.SendPacket(npcTalkBuff);
    }
    /*private void NpcDialogueRequest()
    {
        Debug.Log("Dialogue 요청");
        var npcDialogueBuff = PacketMaker.Instance.Dialogue(_npcId, 1);
        Managers.Network.SendPacket(npcDialogueBuff);
    }*/

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!(col.CompareTag("MyPlayer"))) return;
        
        var player = col.GetComponent<PlayerController>();
        if(player == null && player.IsDeath()) return;

        _isPlayerOverlapping = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!(col.CompareTag("MyPlayer"))) return;

        _isPlayerOverlapping = false;
    }
}