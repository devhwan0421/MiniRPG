using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginTabNavigation : MonoBehaviour
{
    public Selectable idInput;
    public Selectable pwInput;
    public Button loginButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (EventSystem.current.currentSelectedGameObject == null) return;

            if (EventSystem.current.currentSelectedGameObject == idInput.gameObject)
            {
                pwInput.Select();
            }
            else if (EventSystem.current.currentSelectedGameObject == pwInput.gameObject)
            {
                loginButton.Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            loginButton.onClick.Invoke();
        }
    }
}