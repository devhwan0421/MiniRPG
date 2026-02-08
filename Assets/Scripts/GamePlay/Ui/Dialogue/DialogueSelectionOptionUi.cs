using TMPro;
using UnityEngine;

public class DialogueSelectionOptionUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _btnText;

    private int _npcId;
    private int _type;
    private int _dialogueId;
    private int _questId;

    public void SetInfo(int npcId, int type, int dialogueId, string text, int questId)
    {
        _npcId = npcId;
        _type = type;
        _btnText.text = text;
        _dialogueId = dialogueId;
        _questId = questId;
    }

    public void OnClick()
    {
        var npcTalkBuff = PacketMaker.Instance.NpcTalk(_npcId, _type, _dialogueId, _questId);
        Managers.Network.SendPacket(npcTalkBuff);
    }
}