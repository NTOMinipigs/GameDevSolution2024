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
    

    
    private Dictionary<string, int> emptyInventory = new()
    {
        {"maxMaterials", 0},
        {"maxFood", 0},
        {"maxBioFuel", 0},
        {"maxHoney", 0},
        {"maxBears", 0},
        {"maxMaterialPlus", 0},
        {"maxEnergy", 0},
        {"materials", 0},
        {"food", 0},
        {"bioFuel", 0},
        {"honey", 0},
        {"materialPlus", 0},
        {"energy", 0},
    };
    
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

            // Иначе
            APIClient.UserInventory userInventory = await apiClient.CreatePlayerRequest(inputFieldNickname.text, emptyInventory);
            if (userInventory == null) // Если произошла ошибка в запросе
            {
                return;
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
