using TMPro;
using UnityEngine;

public class DialogueOkUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;

    public void ShowDialogue(DialogueOkResponse res)
    {
        _dialogueText.text = res.Contents;
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        _dialogueText.text = string.Empty;
    }
}