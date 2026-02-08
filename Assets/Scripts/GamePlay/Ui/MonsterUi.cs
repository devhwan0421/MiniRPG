using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUi : MonoBehaviour
{
    [SerializeField] private Image _hpImage;
    [SerializeField] private Image _hpAreaImage;
    [SerializeField] private TMP_Text _nickName;

    private int _maxHp;

    public void SetInfo(string nickname, int hp, int maxHp)
    {
        _nickName.text = nickname;
        _maxHp = maxHp;

        UpdateHp(hp);
    }

    public void UpdateHp(int hp)
    {
        float result = (float)hp / _maxHp;
        _hpImage.fillAmount = result;
    }
}
