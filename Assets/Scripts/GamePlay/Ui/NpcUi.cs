using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NpcUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _nickName;

    public void SetInfo(string nickname)
    {
        _nickName.text = nickname;
    }
}
