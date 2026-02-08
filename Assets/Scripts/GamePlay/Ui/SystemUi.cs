using UnityEngine;
using UnityEngine.UI;

public class SystemUi : MonoBehaviour
{
    //public static SystemUi Instance { get; } = new SystemUi();

    [SerializeField] private Image _hpImage;
    [SerializeField] private Image _hpAreaImage;

    public float maxHp = 100;

    private bool _isActive = false;

    //리프레쉬 함수 호출형으로 변경할 것
    private void Update()
    {
        if (!_isActive) return;

        float result = (float)Managers.Object.MyPlayer.PlayerInfo.Hp / maxHp;
        _hpImage.fillAmount = result;
    }

    public void Active()
    {
        _isActive = true; //임시
        gameObject.SetActive(true); //임시
    }

    /*public void SetHp(float currentHp)
    {
        
    }*/
}