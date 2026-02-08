using TMPro;
using UnityEngine;

public class DialogueAcceptDeclineUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;

    private int _npcId;
    //private int _type;
    //private int _nextDialogueId;
    private int _questId;

    public void ShowDialogue(DialogueAcceptDeclineResponse res)
    {
        _npcId = res.NpcId;
        //_type = res.Type;
        //_nextDialogueId = res.NextDialogueId;
        _dialogueText.text = res.Contents;
        _questId = res.QuestId;
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        _dialogueText.text = string.Empty;
    }

    public void Accept()
    {
        /*var npcQuestRequestBuff = PacketMaker.Instance.Quest(_npcId, _questId);
        Managers.Network.SendPacket(npcQuestRequestBuff);*/

        var npcTalkBuff = PacketMaker.Instance.NpcTalk(_npcId, 11, 0, _questId);
        Managers.Network.SendPacket(npcTalkBuff);

        Exit();
    }
    /*public void Accept()
    {
        var npcQuestRequestBuff = PacketMaker.Instance.Quest(_npcId, _questId);
        Managers.Network.SendPacket(npcQuestRequestBuff);

        var npcDialogueBuff = PacketMaker.Instance.Dialogue(_npcId, _nextDialogueId);
        Managers.Network.SendPacket(npcDialogueBuff);
    }*/

    public void Decline()
    {
        //юс╫ц
        Exit();
        //var npcDialogueBuff = PacketMaker.Instance.Dialogue(_npcId, _nextDialogueId);
        //Managers.Network.SendPacket(npcDialogueBuff);
    }
}