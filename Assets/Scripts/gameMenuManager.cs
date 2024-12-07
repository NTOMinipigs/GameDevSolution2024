using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameMenuManager : MonoBehaviour
{
    public GameObject gameMenu;
    [SerializeField] private GameObject mainMenu, preferenceMenu;
    [SerializeField] private Slider sensSlider, volumeSlider;
    [SerializeField] private Toggle postProcessingToggle;
    [SerializeField] private allScripts scripts;

    public void ManageGameMenu()
    {
        if (scripts.CheckOpenedWindows(!gameMenu.activeSelf)) // Если какая-то менюха уже открыта
            return;

        gameMenu.gameObject.SetActive(!gameMenu.activeSelf);
        if (gameMenu.activeSelf)
        {
            mainMenu.gameObject.SetActive(true);
            preferenceMenu.gameObject.SetActive(false);
            scripts.clicksHandler.SetTimeScale(0.05f);
        }
        else
            scripts.clicksHandler.SetTimeScale(1f);
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
