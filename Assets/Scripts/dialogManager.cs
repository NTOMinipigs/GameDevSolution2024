using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textName, _textDialog;
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject dialogMenu;
    public Dialog[] dialogs = new Dialog[0];
    [SerializeField] private int totalStep;
    private Dialog _activatedDialog;
    private DialogStep _selectedStep;
    private bool _animatingText, _canStepNext;
    [SerializeField] private allScripts scripts;

    public void ActivateDialog(string name) // Старт диалога
    {
        if (_activatedDialog == null)
        {
            foreach (Dialog totalDialog in dialogs)
            {
                if (totalDialog.nameDialog == name)
                {
                    _activatedDialog = totalDialog;
                    dialogMenu.gameObject.SetActive(true);
                    scripts.clicksHandler.blockMove = true;
                    _selectedStep = _activatedDialog.steps[0];
                    DialogUpdateAction();
                    break;
                }
            }
        }
    }

    private void DialogUpdateAction()
    {
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