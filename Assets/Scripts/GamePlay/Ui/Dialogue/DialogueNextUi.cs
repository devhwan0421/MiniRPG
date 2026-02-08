using TMPro;
using UnityEngine;

public class DialogueNextUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;

    private int _npcId;
    //private int _type;
    private int _nextDialogueId;

    public void ShowDialogue(DialogueNextResponse res)
    {
        _npcId = res.NpcId;
        //_type = res.Type;
        _nextDialogueId = res.NextDialogueId;
        _dialogueText.text = res.Contents;
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        _dialogueText.text = string.Empty;
    }

    public void Next()
    {
        Debug.Log($"Dialogue ø‰√ª {_npcId}, {0}, {_nextDialogueId}, {0}");
        var npcTalkBuff = PacketMaker.Instance.NpcTalk(_npcId, 0, _nextDialogueId, 0);
        Managers.Network.SendPacket(npcTalkBuff);
    }
}