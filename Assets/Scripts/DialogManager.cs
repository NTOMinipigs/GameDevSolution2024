using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName, textDialog;
    [SerializeField] private Image iconImage;
    public GameObject dialogMenu;

    public Dialog[] dialogs = Array.Empty<Dialog>();
    private Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();

    [SerializeField] private int totalStep;
    private Dialog _activatedDialog;
    private DialogStep _selectedStep;
    private Bear _selectedBear;
    private bool _animatingText, _canStepNext;

    [SerializeField] private AllScripts scripts;

    private void Start()
    {
        foreach (Dialog totalDialog in dialogs)
            _dialogsDict.Add(totalDialog.nameDialog, totalDialog);
    }

    /// <summary>
    /// Метод возвращает диалог по имени
    /// </summary>
    /// <param name="dialogName">Название диалога</param>
    /// <returns></returns>
    private Dialog GetDialog(string dialogName)
    {
        return _dialogsDict[dialogName];
    }

    /// <summary>
    /// Открытие менюшки диалога
    /// </summary>
    /// <param name="dialogName">Название диалога</param>
    /// <param name="gameNameBear">Dev имя медведя, для произвольного диалого</param>
    /// <param name="blockWithOtherMenu">Блокировать иные окна?</param>
    public void ActivateDialog(string dialogName, string gameNameBear = "",
        bool blockWithOtherMenu = false) // Старт диалога
    {
        if (scripts.CheckOpenedWindows(blockWithOtherMenu)) // Если какая-то менюха уже открыта
            return;

        scripts.musicManager.AudioLoops["snow_steps"].Play();

        if (_activatedDialog != null) return;

        _activatedDialog = GetDialog(dialogName);
        dialogMenu.gameObject.SetActive(true);
        scripts.cameraMove.blockMove = true;
        _selectedStep = _activatedDialog.steps[0];
        if (gameNameBear != "")
            _selectedBear = scripts.colonyManager.GetBear(gameNameBear);
        DialogUpdateAction();
    }

    // Старт диалога при взаимодействии с медведем
    public void ActivateBearInteractionDialog(Bear selectedBear)
    {
        if (selectedBear.tired > 10)
            ActivateDialog("bearTired", selectedBear.gameName, true);
        else if (selectedBear.hungry > 10)
            ActivateDialog("bearHungry", selectedBear.gameName, true);
        else
        {
            int mode = Random.Range(0, 2);
            if (mode == 0)
                ActivateDialog("bearTalk" + Random.Range(0, 4), selectedBear.gameName, true);
            else if (mode == 1)
                ActivateDialog("bearActivity", selectedBear.gameName, true);
        }
    }

    private void DialogUpdateAction()
    {
        _selectedBear.canMove = true;
        if (_selectedBear != null)
        {
            _selectedStep.nameBear = _selectedBear.bearName;
            _selectedStep.traditionBear = _selectedBear.tradition;
            _selectedStep.icon = _selectedBear.sprite;
        }
        else
            _selectedBear = _selectedStep.SetBear(scripts.colonyManager);

        _selectedBear.canMove = false;
        scripts.cameraMove.MoveAndZoom(GameObject.Find(_selectedBear.gameName).transform.position, 20f);

        textName.text = _selectedBear?.bearName + " | " + _selectedBear?.TraditionStr;
        StartCoroutine(SetText(_selectedStep.text));
        iconImage.sprite = _selectedStep.icon;
        iconImage.SetNativeSize();
    }

    private void DialogMoveNext()
    {
        if (totalStep + 1 == _activatedDialog.steps.Length) // Окончание обычного диалога
        {
            _selectedBear.canMove = true;
            DialogCLose();
            return;
        }

        totalStep++;
        _selectedStep = _activatedDialog.steps[totalStep];
        if (_selectedStep.questStart != "")
            scripts.questSystem.ActivateQuest(_selectedStep.questStart);
        DialogUpdateAction();
    }

    private void DialogCLose()
    {
        scripts.musicManager.AudioLoops["snow_steps"].Stop();
        totalStep = 0;
        dialogMenu.gameObject.SetActive(false);
        _activatedDialog = null;
        scripts.cameraMove.blockMove = false;
    }

    private string CodeTextReplace(string text)
    {
        if (text.Contains("{activity}"))
            return text.Replace("{activity}", _selectedBear.ActivityStr);
        return text;
    }

    private void Update()
    {
        if (_activatedDialog != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_animatingText)
                {
                    _animatingText = false;
                    StopAllCoroutines();
                    string newText = CodeTextReplace(_selectedStep.text);
                    textDialog.text = newText;
                }
                else
                    DialogMoveNext();
            }
        }
    }

    private IEnumerator SetText(string text)
    {
        textDialog.text = "";
        _animatingText = true;
        text = CodeTextReplace(text);
        char[] textChar = text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (_animatingText)
            {
                textDialog.text += tChar;
                yield return new WaitForSeconds(0.05f);
            }
        }

        _animatingText = false;
    }
}

[System.Serializable]
public class Dialog
{
    [Header("Main")] public string nameDialog;
    public DialogStep[] steps = new DialogStep[0];
}

[System.Serializable]
public class DialogStep
{
    public Traditions traditionBear;
    [HideInInspector] public string nameBear;
    public string text;
    [HideInInspector] public Sprite icon;
    public string questStart;

    public Bear SetBear(ColonyManager CM)
    {
        foreach (Bear totalBear in CM.bearsInColony)
        {
            if (totalBear.tradition == traditionBear)
            {
                nameBear = totalBear.bearName;
                icon = totalBear.sprite;
                return totalBear;
            }
        }

        return null;
    }
}