using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSelectionUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _selectionOptionPrefab;

    private int _npcId;
    private int _type;

    private List<GameObject> _selectionOptions = new List<GameObject>();

    public void ShowDialogue(DialogueSelectionResponse res)
    {
        Clear();
        _npcId = res.NpcId;
        _type = res.Type;
        _dialogueText.text = res.Contents;

        foreach (var option in res.Selections)
        {
            GameObject go = Instantiate(_selectionOptionPrefab, _content);
            _selectionOptions.Add(go);

            var dialogueSelectionOptionUi = go.GetComponent<DialogueSelectionOptionUi>();
            if (dialogueSelectionOptionUi != null)
            {
                dialogueSelectionOptionUi.SetInfo(res.NpcId, option.OptionType, option.DialogueId, option.Contents, option.QuestId);
            }
        }

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        _dialogueText.text = string.Empty;
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
        _selectionOptions.Clear();
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }
}