using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UGC;


/// <summary>
/// Управляет главным меню
/// </summary>
public class MenuManager : MonoBehaviour
{
    #region Переменные 

    [Header("IDK")]
    [SerializeField] private Preference preference; // Нужно сделать загрузку и сохранение
    [SerializeField] private Player player;
    [SerializeField] private APIClient apiClient;
    private bool gameExists;

    [Header("UI")]
    [SerializeField] private GameObject enterToGameCanvas;
    [SerializeField] private GameObject authCanvas;
    [SerializeField] private GameObject menuMain, menuNickname, menuMods, menuNewGame, editorMenu, warningNewGame;
    [SerializeField] private TMP_InputField inputFieldNickname, inputFieldSeed;
    [SerializeField] private TextMeshProUGUI textInfo;
    [SerializeField] private Button buttonResume;

    #endregion


    private void Start()
    {
        authCanvas.SetActive(true);
        enterToGameCanvas.SetActive(false);
        if (Config.ConfigManager.Instance.config.debug)
        {
            player.playerName = "test";
            StartGame();
        }
    }

    public void StartGame() => SceneManager.LoadScene("Game");

    public void CreateNewGame()
    {
        if (inputFieldSeed.text == "")
            return;

        if (gameExists)
        {
            // Если предупреждение было закрыто - открываем
            if (!warningNewGame.activeSelf)
            {
                warningNewGame.SetActive(true);
                return;
            }
            else // Стираем все
            {
                apiClient.DeletePlayerRequest(player.playerName);
                SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
                systemSaver.DeleteGame();
            }

        }

        player.seed = int.Parse(inputFieldSeed.text);
        StartGame();
    }

    public void DisableWarningMenu() => warningNewGame.SetActive(false);

    public void ManageCreateGameMenu()
    {
        menuNewGame.SetActive(!menuNewGame.activeSelf);
        menuMain.SetActive(!menuNewGame.activeSelf);
        inputFieldSeed.text = Random.Range(0, 100000).ToString();
    }

    public void ManageModsMenu()
    {
        menuMods.SetActive(!menuMods.activeSelf);
        menuMain.SetActive(!menuMods.activeSelf);

        // Обновляем превьюшки модов в мод меню

        // Удаляю все превьюшки в контенте 
        foreach (Transform child in UGCManager.Singleton.UGCViewsContent.transform)
            Destroy(child.gameObject);

        UGCManager.Singleton.ShowAllUGC();
    }

    public void ExitGame() => Application.Quit();
    public void OpenPreferencs() => preference.ManagePrefenceMenu();

    private void ActivateEnterMenu()
    {
        authCanvas.SetActive(false);
        enterToGameCanvas.SetActive(true);
        textInfo.text += " " + player.playerName;
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();

        bool loadResult = systemSaver.LoadGame();
        gameExists = loadResult;
        buttonResume.interactable = gameExists;
    }

    public async void ActivateNickname() // Вызывать при первом старте?
    {
        if (inputFieldNickname.text != "")
        {
            List<APIClient.UserInventory> users = await apiClient.GetUsersListRequest();

            foreach (APIClient.UserInventory user in users)
            {
                // Если пользователь существует
                if (inputFieldNickname.text == user.Name)
                {
                    player.playerName = inputFieldNickname.text;
                    player.seed = user.Resources["seed"];
                    ActivateEnterMenu();
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

    /// <summary>
    /// включить/выключить Editor для модов
    /// </summary>
    public void EditorButtonManage()
    {
        editorMenu.SetActive(!editorMenu.activeSelf);
    }
}
