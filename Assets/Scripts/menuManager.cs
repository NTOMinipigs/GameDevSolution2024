using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Preference preference; // Нужно сделать загрузку и сохранение
    [SerializeField] private GameObject menuNickname;
    [SerializeField] private TMP_InputField inputFieldNickname;

    public void StartGame() => SceneManager.LoadScene("Game");
    public void ExitGame() => Application.Quit();
    public void OpenPreferencs() => preference.ManagePrefenceMenu();

    public void ActivateNickname() // Вызывать при первом старте?
    {
        if (inputFieldNickname.text != "")
        {
            menuNickname.gameObject.SetActive(false);
            // ...
        }
    }
}
