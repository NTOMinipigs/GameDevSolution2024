using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class dialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textName, _textDialog;
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject dialogMenu;
    public dialog[] dialogs = new dialog[0];
    [SerializeField] private int totalStep;
    private dialog _activatedDialog;
    private dialogStep _selectedStep;
    private bear _selectedBear;
    private bool _animatingText, _canStepNext;
    [SerializeField] private allScripts scripts;

    private dialog GetDialog(string name)
    {
        foreach (dialog totalDialog in dialogs)
        {
            if (totalDialog.nameDialog == name)
                return totalDialog;
        }
        return null;
    }

    public void ActivateDialog(string name, string gameNameBear = "") // Старт диалога
    {
        if (_activatedDialog == null)
        {
            _activatedDialog = GetDialog(name);
            dialogMenu.gameObject.SetActive(true);
            scripts.clicksHandler.blockMove = true;
            _selectedStep = _activatedDialog.steps[0];
            if (gameNameBear != "")
                _selectedBear = scripts.colonyManager.GetBear((gameNameBear));
            DialogUpdateAction();
        }
    }

    // Старт диалога при взаимодействии с медведем
    public void ActivateBearInteractionDialog(bear selectedBear)
    {
        if (selectedBear.tired > 10)
            ActivateDialog("bearTired", selectedBear.gameName);
        else if (selectedBear.hungry > 10)
            ActivateDialog("bearHungry", selectedBear.gameName);
        else
        {
            int mode = Random.Range(0, 2);
            if (mode == 0)
                ActivateDialog("bearTalk" + Random.Range(0, 3), selectedBear.gameName);
            else if (mode == 1)
                ActivateDialog("bearActivity", selectedBear.gameName);
        }
    }

    private void DialogUpdateAction()
    {
        if (_selectedBear != null)
        {
            _selectedStep.nameBear = _selectedBear.bearName;
            _selectedStep.traditionBear = _selectedBear.tradition;
            _selectedStep.icon = _selectedBear.sprite;
        }
        else
            _selectedStep.SetBear(scripts.colonyManager);
        _textName.text = _selectedStep.nameBear;
        StartCoroutine(SetText(_selectedStep.text));
        _iconImage.sprite = _selectedStep.icon;
        _iconImage.SetNativeSize();
    }

    private void DialogMoveNext()
    {
        if ((totalStep + 1) == _activatedDialog.steps.Length) // Окончание обычного диалога
        {
            DialogCLose();
            return;
        }
        totalStep++;
        _selectedStep = _activatedDialog.steps[totalStep];
        DialogUpdateAction();
    }

    public void DialogCLose()
    {
        totalStep = 0;
        dialogMenu.gameObject.SetActive(false);
        _activatedDialog = null;
        scripts.clicksHandler.blockMove = false;
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
                    _textDialog.text = _selectedStep.text;
                }
                else
                    DialogMoveNext();
            }
        }
    }

    private IEnumerator SetText(string text)
    {
        _textDialog.text = "";
        _animatingText = true;
        if (text.Contains("{activity}"))
            text = text.Replace("{activity}", scripts.colonyManager.GetBear(_selectedStep.nameBear).activityStr);
        char[] textChar = text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (_animatingText)
            {
                _textDialog.text += tChar;
                yield return new WaitForSeconds(0.05f);
            }
        }
        _animatingText = false;
    }
}

[System.Serializable]
public class dialog
{
    [Header("Main")]
    public string nameDialog;
    public dialogStep[] steps = new dialogStep[0];
}

[System.Serializable]
public class dialogStep
{
    public bear.traditions traditionBear;
    [HideInInspector] public string nameBear;
    public string text;
    [HideInInspector] public Sprite icon;
    public Transform cameraTarget;

    public void SetBear(colonyManager CM)
    {
        foreach (bear totalBear in CM.bearsInColony)
        {
            if (totalBear.tradition == traditionBear)
            {
                nameBear = totalBear.bearName;
                icon = totalBear.sprite;
            }
        }
    }
}