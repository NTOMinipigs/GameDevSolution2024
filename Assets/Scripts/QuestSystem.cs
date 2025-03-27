using System;
using Cutscene;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// Система квестов (q for open it)
/// </summary>
public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Singleton { get; private set; }
    public GameObject questMenu;
    [SerializeField] private GameObject prehistoryObj;
    [SerializeField] private TextMeshProUGUI totalQuestText;
    private TextMeshProUGUI textName, textDescription, textStep;
    [SerializeField] private quest[] questsInGame = new quest[0];
    public int totalStep;
    public quest totalQuest;

    private void Awake()
    {
        Singleton = this;
    }

    public quest FindQuest(string questName)
    {
        foreach (quest newQuest in questsInGame)
        {
            if (newQuest.questName == questName)
                return newQuest;
        }
        return null;
    }

    public void StartFirst() // Заставка + первый квест
    {
        // Запуск катсцены в самом начале игры
        if (Config.ConfigManager.Instance.config.debug)
        {
            return;
        }

        CutsceneManager.Singleton.StartCutscene(CutsceneManager.Singleton.FirstCutscene);
        DialogManager.Singleton.ActivateDialog("afterPrehistory");
        Tutorial.Singleton.ActivateTutorialMenu();
    }

    public void ActivateQuest(string questName)
    {
        quest newQuest = FindQuest(questName);
        if (newQuest != null)
        {
            totalQuest = newQuest;
            totalStep = 0;
            DoStep(totalStep);
            UpdateQuestUI();
        }
    }

    public void MoveNextStep()
    {
        if (totalQuest != null)
        {
            totalStep++;
            if (totalQuest.steps.Length == totalStep)
            {
                totalQuest = null;
                return;
            }
            DoStep(totalStep);
        }
    }

    private void DoStep(int step)
    {
        if (totalQuest.steps[step].startStepWithDialog != "")
            DialogManager.Singleton.ActivateDialog(totalQuest.steps[step].startStepWithDialog);
    }

    public void UpdateQuestUI()
    {
        if (questMenu.activeSelf)
        {
            if (textName == null)
            {
                textName = questMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
                textDescription = questMenu.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>();
                textStep = questMenu.transform.Find("TextTotalStep").GetComponent<TextMeshProUGUI>();
            }
            textName.text = totalQuest != null ? totalQuest.gameName : "";
            textDescription.text = totalQuest != null ? totalQuest.description : "";
            textStep.text = totalQuest != null ? totalQuest.steps[totalStep].stepName : "";
        }
        totalQuestText.text = totalQuest.gameName + "\n----------------\n" + totalQuest.steps[totalStep].stepName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!GameMenuManager.Singleton.CheckOpenedWindows(!questMenu.activeSelf)) // Если какая-то менюха уже открыта
            {
                questMenu.gameObject.SetActive(!questMenu.activeSelf);
                CameraMove.Singleton.blockMove = questMenu.activeSelf;
                UpdateQuestUI();
            }
        }
        // if (totalQuest != null)
        //     totalQuestText.text = totalQuest.steps[totalStep].stepName;
        // else
        //     totalQuestText.text = "";
    }
}

[System.Serializable]
public class quest
{
    public string questName, gameName, description;
    public questStep[] steps = new questStep[0];
}

[System.Serializable]
public class questStep
{
    public string stepName;
    public string startStepWithDialog;
}
