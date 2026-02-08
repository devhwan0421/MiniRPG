using UnityEngine;

public class DialogueUi : MonoBehaviour
{
    [SerializeField] private DialogueSimpleUi _dialogueSimpleUi;
    [SerializeField] private DialogueOkUi _dialogueOkUi;
    [SerializeField] private DialogueNextUi _dialogueNextUi;
    [SerializeField] private DialogueAcceptDeclineUi _dialogueAcceptDeclineUi;
    [SerializeField] private DialogueSelectionUi _dialogueSelectionUi;

    public DialogueUi()
    {
        //_dialogueSimpleUi = GetComponentInChildren<DialogueSimpleUi>();
        //_dialogueOkUi = GetComponentInChildren<DialogueOkUi>();
        //_dialogueNextUi = GetComponentInChildren<DialogueNextUi>();
        //_dialogueAcceptDeclineUi = GetComponentInChildren<DialogueAcceptDeclineUi>();
    }

    /*public void OnDialogue(DialogueResponse res)
    {
        Debug.Log($"µ•¿Ã≈Õ: {res.Contents}");
        ClearAll();
        switch (res.Type)
        {
            case 0:
                _dialogueSimpleUi.ShowDialogue(res);
                break;
            case 1:
                _dialogueOkUi.ShowDialogue(res);
                break;
            case 2:
                _dialogueNextUi.ShowDialogue(res);
                break;
            case 3:
                _dialogueAcceptDeclineUi.ShowDialogue(res);
                break;
        }
    }*/

    public void OnDialogueSimple(DialogueSimpleResponse res)
    {
        ClearAll();
        _dialogueSimpleUi.ShowDialogue(res);
    }

    public void OnDialogueOk(DialogueOkResponse res)
    {
        ClearAll();
        _dialogueOkUi.ShowDialogue(res);
    }

    public void OnDialogueNext(DialogueNextResponse res)
    {
        ClearAll();
        _dialogueNextUi.ShowDialogue(res);
    }

    public void OnDialogueAcceptDecline(DialogueAcceptDeclineResponse res)
    {
        ClearAll();
        _dialogueAcceptDeclineUi.ShowDialogue(res);
    }

    public void OnDialogueSelection(DialogueSelectionResponse res)
    {
        ClearAll();
        _dialogueSelectionUi.ShowDialogue(res);
    }

    private void ClearAll()
    {
        _dialogueSimpleUi.Exit();
        _dialogueOkUi.Exit();
        _dialogueNextUi.Exit();
        _dialogueAcceptDeclineUi.Exit();
        _dialogueSelectionUi.Exit();
    }
}