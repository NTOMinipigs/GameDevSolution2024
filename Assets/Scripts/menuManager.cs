using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    [SerializeField] private Preference preference; // Нужно сделать загрузку и сохранение

    public void StartGame() => SceneManager.LoadScene("Game");
    public void ExitGame() => Application.Quit();
    public void OpenPreferencs() => preference.ManagePrefenceMenu();
}
