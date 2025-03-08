using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Singleton { get; private set; }
    public GameObject gameMenu;
    [SerializeField] private GameObject mainMenu, preferenceMenu;
    [SerializeField] private Slider sensSlider, volumeSlider;
    [SerializeField] private Toggle postProcessingToggle;

    private void Awake()
    {
        Singleton = this;
    }

    /// <summary>
    /// Проверяет - открыто какое-нибудь окно или нет
    /// </summary>
    /// <param name="mode">Принимает true - если в планах открыть новое меню</param>
    /// <param name="extraOpen">Открытие не смотря на другие условия</param>
    /// <returns></returns>
    public bool CheckOpenedWindows(bool mode = true, bool extraOpen = false)
    {
        if (!mode) // Если окно закрывается, то скипать проверку
            return false;

        return (DialogManager.Singleton.dialogMenu.activeSelf || QuestSystem.Singleton.questMenu.activeSelf ||
                ColonyManager.Singleton.bearsListMenu.activeSelf ||
                BuildingSystem.Singleton.buildingCreateMenu.activeSelf ||
                BuildingSystem.Singleton.buildMenu.activeSelf || TravelingManager.Singleton.travelMenu.activeSelf ||
                gameMenu.activeSelf) && !extraOpen;
    }

    public void ManageGameMenu()
    {
        if (CheckOpenedWindows(!gameMenu.activeSelf)) // Если какая-то менюха уже открыта
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
            sensSlider.value = Preference.Singleton.sensitivityOfCamera;
            volumeSlider.value = Preference.Singleton.globalVolume;
            postProcessingToggle.isOn = Preference.Singleton.postProcessing;
        }
    }

    public void SavePreferens()
    {
        Preference.Singleton.sensitivityOfCamera = sensSlider.value;
        CameraMove.Singleton.sensitivity = Preference.Singleton.sensitivityOfCamera;
        Preference.Singleton.globalVolume = volumeSlider.value;
        Preference.Singleton.postProcessing = postProcessingToggle.isOn;
        MusicManager.Singleton.UpdateVolumeRatio(volumeSlider.value * 0.01f);
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