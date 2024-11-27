using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class questSystem : MonoBehaviour
{
    [SerializeField] private GameObject questMenu;
    private TextMeshProUGUI textName, textDescription, textStep;
    [SerializeField] private quest[] questsInGame = new quest[0];
    public int totalStep;
    private quest totalQuest;
    [SerializeField] private allScripts scripts;

    public quest FindQuest(string questName)
    {
        foreach (quest newQuest in questsInGame)
        {
            if (newQuest.questName == questName)
                return newQuest;
        }
        return null;
    }

    public void ActivateQuest(string questName)
    {
        quest newQuest = FindQuest(questName);
        if (newQuest != null)
        {
            // TODO: Сделать мигающую икноку нового квеста
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
            if (totalQuest.steps.Length == totalStep + 1)
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
            scripts.dialogManager.ActivateDialog(totalQuest.steps[step].startStepWithDialog);
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questMenu.gameObject.SetActive(!questMenu.activeSelf);
            UpdateQuestUI();
        }
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
