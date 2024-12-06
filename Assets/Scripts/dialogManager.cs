using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textName, _textDialog;
    [SerializeField] private Image _iconImage;
    public GameObject dialogMenu;
    public Dialog[] dialogs = new Dialog[0];
    [SerializeField] private int totalStep;
    private Dialog _activatedDialog;
    private DialogStep _selectedStep;
    private Bear _selectedBear;
    private bool _animatingText, _canStepNext;
    [SerializeField] private allScripts scripts;

    private Dialog GetDialog(string name)
    {
        foreach (Dialog totalDialog in dialogs)
        {
            if (totalDialog.nameDialog == name)
                return totalDialog;
        }
        return null;
    }

    public void ActivateDialog(string name, string gameNameBear = "", bool blockWithOtherMenu = false) // Старт диалога
    {
        if (scripts.CheckOpenedWindows(blockWithOtherMenu)) // Если какая-то менюха уже открыта
            return;

        if (_activatedDialog == null)
        {
            Debug.Log(name);
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
                ActivateDialog("bearTalk" + Random.Range(0, 3).ToString(), selectedBear.gameName, true);
            else if (mode == 1)
                ActivateDialog("bearActivity", selectedBear.gameName, true);
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
        _textName.text = _selectedStep.nameBear + " | " + _selectedBear.TraditionStr;
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
                    _textDialog.text = newText;
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
        text = CodeTextReplace(text);
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
public class Dialog
{
    [Header("Main")]
    public string nameDialog;
    public DialogStep[] steps = new DialogStep[0];
}

[System.Serializable]
public class DialogStep
{
    public Bear.Traditions traditionBear;
    [HideInInspector] public string nameBear;
    public string text;
    [HideInInspector] public Sprite icon;
    public Transform cameraTarget;

    public void SetBear(ColonyManager CM)
    {
        foreach (Bear totalBear in CM.bearsInColony)
        {
            if (totalBear.tradition == traditionBear)
            {
                nameBear = totalBear.bearName;
                icon = totalBear.sprite;
            }
        }
    }
}