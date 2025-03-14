using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


/// <summary>
/// Управляет главным меню
/// </summary>
public class MenuManager : MonoBehaviour
{
    #region хуйня полная  

    [SerializeField] private Preference preference; // Нужно сделать загрузку и сохранение
    [SerializeField] private GameObject menuNickname;
    [SerializeField] private TMP_InputField inputFieldNickname;
    [SerializeField] private Player player;
    [SerializeField] private APIClient apiClient;
    [SerializeField] private GameObject enterToGameCanvas;
    [SerializeField] private GameObject authCanvas;
    
    #endregion


    private void Start()
    {
        if (Config.ConfigManager.Instance.config.debug)
        {
            player.playerName = "test";
            StartGame();
        }
    }

    public void StartGame() => SceneManager.LoadScene("Game");
    public void ExitGame() => Application.Quit();
    public void OpenPreferencs() => preference.ManagePrefenceMenu();

    private void ActivateEnterMenu()
    {
        authCanvas.SetActive(false);
        enterToGameCanvas.SetActive(true);
    }

    public async void ActivateNickname() // Вызывать при первом старте?
    {
        if (inputFieldNickname.text != "")
        {
            ActivateEnterMenu();
            List<APIClient.UserInventory> users = await apiClient.GetUsersListRequest();

            foreach (APIClient.UserInventory user in users)
            {
                // Если пользователь уже существует
                if (inputFieldNickname.text == user.Name)
                {
                    player.playerName = inputFieldNickname.text;
                    return;
                }
            }

            player.playerName = inputFieldNickname.text;
            ActivateEnterMenu();
        }
    }

    public void SetQuest()
    {
        player.playerName = "nickname";
        ActivateEnterMenu();
    }
    
    
    
}
