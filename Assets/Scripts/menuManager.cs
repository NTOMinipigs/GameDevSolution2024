using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Preference preference; // Нужно сделать загрузку и сохранение
    [SerializeField] private GameObject menuNickname;
    [SerializeField] private TMP_InputField inputFieldNickname;

    private Dictionary<string, int> emptyInventory = new Dictionary<string, int>()
    {
        {"materials", 0},
        {"food", 0},
        {"bioFuel", 0},
        {"honey", 0},
        {"materialPlus", 0},
        {"energy", 0},
        {"maxMaterials", 0},
        {"maxFood", 0},
        {"maxBioFuel", 0},
        {"maxHoney", 0},
        {"maxBears", 0},
        {"maxMaterialPlus", 0},
    };

    private void Start()
    {
        if (Config.ConfigManager.Instance.config.debug)
        {
            Player.Instance.playerName = "test";
            StartGame();
        }
    }

    public void StartGame() => SceneManager.LoadScene("Game");
    public void ExitGame() => Application.Quit();
    public void OpenPreferencs() => preference.ManagePrefenceMenu();

    public async void ActivateNickname() // Вызывать при первом старте?
    {

        if (inputFieldNickname.text != "")
        {
            menuNickname.gameObject.SetActive(false);
            List<APIClient.UserInventory> users = await APIClient.Instance.GetUsersListRequest();

            foreach (APIClient.UserInventory user in users)
            {
                // Если пользователь уже существует
                if (inputFieldNickname.text == user.Name)
                {
                    Player.Instance.playerName = inputFieldNickname.text;
                    return;
                }
            }

            // Иначе
            APIClient.UserInventory userInventory = await APIClient.Instance.CreatePlayerRequest(inputFieldNickname.text, emptyInventory);
            if (userInventory == null) // Если произошла ошибка в запросе
            {
                return;
            }

            Player.Instance.playerName = inputFieldNickname.text;
            menuNickname.gameObject.SetActive(false);
        }
    }

    public async void SetQuest()
    {
        Player.Instance.playerName = "nickname";
        menuNickname.gameObject.SetActive(false);
    }
}
