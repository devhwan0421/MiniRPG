using UnityEngine;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _nickName;
    [SerializeField] private GameObject DeathObject;

    public void SetInfo(int level, string nickname)
    {
        _nickName.text = nickname;
        _level.text = $"Lv. {level}";
    }

    public void SetLevel(int level)
    {
        _level.text = $"Lv. {level}";
    }

    public void SetDeathOn() => DeathObject.SetActive(true);
    public void SetDeathOff() => DeathObject.SetActive(false);
}
