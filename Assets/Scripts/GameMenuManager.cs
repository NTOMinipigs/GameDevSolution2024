using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public GameObject gameMenu;
    [SerializeField] private GameObject mainMenu, preferenceMenu;
    [SerializeField] private Slider sensSlider, volumeSlider;
    [SerializeField] private Toggle postProcessingToggle;
    [SerializeField] private AllScripts scripts;

    public void ManageGameMenu()
    {
        if (scripts.CheckOpenedWindows(!gameMenu.activeSelf)) // Если какая-то менюха уже открыта
            return;

        gameMenu.gameObject.SetActive(!gameMenu.activeSelf);
        if (gameMenu.activeSelf)
        {
            mainMenu.gameObject.SetActive(true);
            preferenceMenu.gameObject.SetActive(false);
            Time.timeScale = 0.05f;
        }
        else
            Time.timeScale = 1f;
    }

    public void ManagePreferensMenu()
    {
        preferenceMenu.gameObject.SetActive(!preferenceMenu.activeSelf);
        mainMenu.gameObject.SetActive(!preferenceMenu.activeSelf);
        if (preferenceMenu.activeSelf)
        {
            sensSlider.value = scripts.preference.sensitivityOfCamera;
            volumeSlider.value = scripts.preference.globalVolume;
            postProcessingToggle.isOn = scripts.preference.postProcessing;
        }
    }

    public void SavePreferens()
    {
        scripts.preference.sensitivityOfCamera = sensSlider.value;
        scripts.cameraMove.sensitivity = scripts.preference.sensitivityOfCamera;
        scripts.preference.globalVolume = volumeSlider.value;
        scripts.preference.postProcessing = postProcessingToggle.isOn;
        ManagePreferensMenu();
    }

    public void ExitToMenu() => SceneManager.LoadScene("Menu");
    public void ExitFromGame() => Application.Quit();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ManageGameMenu();
    }
}
