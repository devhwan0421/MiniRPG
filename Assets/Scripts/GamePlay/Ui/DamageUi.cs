using UnityEngine;
using TMPro;
using System.Collections;

public class DamageUi : MonoBehaviour
{
    private TextMeshProUGUI _damageText;
    private float moveSpeed = 0.5f;
    private float duration = 0.7f;

    private void Awake()
    {
        _damageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(int damage, Color color)
    {
        _damageText.text = damage.ToString();
        _damageText.color = color;

        CancelInvoke("ReturnToPool");
        Invoke("ReturnToPool", duration);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    private void ReturnToPool()
    {
        Managers.Pool.Push(this.gameObject);
    }
}