using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cutscene;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class DialogManager : MonoBehaviour
{
    #region BearSpec

    /// <summary>
    /// расстояние от камеры до объекта
    /// </summary>
    public Vector3 offset;

    /// <summary>
    /// Скорость сглаживания
    /// </summary>
    public float zoomSpeed;

    /// <summary>
    /// Камера
    /// </summary>
    public Camera camera;

    /// <summary>
    /// Сохрани здесь позицию камеры, до того как ты прикрепился к медведю
    /// </summary>
    private Vector3 beforeCamPosition;

    #endregion


    public static DialogManager Singleton { get; private set; }
    [SerializeField] private TextMeshProUGUI textName, textDialog;
    [SerializeField] private Image iconImage;
    public GameObject dialogMenu;

    public Dialog[] dialogs = new Dialog[0];
    private Dictionary<string, Dialog> _dialogsDict = new();

    [SerializeField] private int totalStep;
    private bool interactionDialog;
    private Dialog _activatedDialog;
    private DialogStep _selectedStep;
    private Bear _selectedBear;
    private bool _animatingText, _canStepNext;

    private void Awake()
    {
        Singleton = this;
    }

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
        beforeCamPosition = camera.transform.position;

        if (GameMenuManager.Singleton.CheckOpenedWindows(blockWithOtherMenu)) // Если какая-то менюха уже открыта
            return;

        if (_activatedDialog != null) return;

        _activatedDialog = GetDialog(dialogName);
        dialogMenu.gameObject.SetActive(true);
        CameraMove.Singleton.blockMove = true;
        _selectedStep = _activatedDialog.steps[0];
        if (gameNameBear != "")
            _selectedBear = ColonyManager.Singleton.GetBear(gameNameBear);
        DialogUpdateAction();
        MusicManager.Singleton.AudioLoops["snow_steps"].Play();
    }

    // Старт диалога при взаимодействии с медведем
    public void ActivateBearInteractionDialog(Bear selectedBear)
    {
        interactionDialog = true;
        if (selectedBear.tired > 10)
            ActivateDialog("bearTired", selectedBear.gameName, true);
        else if (selectedBear.hungry > 10)
            ActivateDialog("bearHungry", selectedBear.gameName, true);
        else
        {
            int mode = Random.Range(0, 2);
            if (mode == 0)
                ActivateDialog("bearTalk" + Random.Range(0, 8), selectedBear.gameName, true);
            else if (mode == 1)
                ActivateDialog("bearActivity", selectedBear.gameName, true);
        }
    }

    private void DialogUpdateAction()
    {
        if (_selectedBear != null)
        {
            _selectedBear.canMove = true;
            _selectedStep.nameBear = _selectedBear.bearName;
            _selectedStep.traditionBear = _selectedBear.tradition;
            _selectedStep.icon = _selectedBear.sprite;
        }
        else
            _selectedBear = _selectedStep.SetBear(ColonyManager.Singleton);

        _selectedBear.canMove = false;
        if (GameObject.Find(_selectedBear.gameName))
            CameraMove.Singleton.MoveAndZoom(GameObject.Find(_selectedBear.gameName).transform.position, 20f);

        textName.text = _selectedBear?.bearName + " | " + _selectedBear?.tradition.GetString();
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
            QuestSystem.Singleton.ActivateQuest(_selectedStep.questStart);
        DialogUpdateAction();
    }

    private void DialogCLose()
    {
        MusicManager.Singleton.AudioLoops["snow_steps"].Stop();
        totalStep = 0;
        dialogMenu.gameObject.SetActive(false);
        _activatedDialog = null;
        CameraMove.Singleton.blockMove = false;
        _selectedBear = null;
        camera.transform.position = beforeCamPosition;
        if (interactionDialog)
        {
            if (QuestSystem.Singleton.GetEndTrigger() == "dialog")
                QuestSystem.Singleton.MoveNextStep();
        }
        interactionDialog = false;
    }

    private void CameraBack()
    {
    }

    private string CodeTextReplace(string text)
    {
        if (text.Contains("{activity}"))
            return text.Replace("{activity}", _selectedBear.activity.GetString());
        if (text.Contains("{totalDay}"))
            return text.Replace("{totalDay}", GameEventsManager.Singleton.gameDay.ToString());
        if (text.Contains("{myName}"))
            return text.Replace("{myName}", _selectedBear.bearName);
        return text;
    }

    private void Update()
    {
        if (_activatedDialog != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CutsceneView.Singleton.cutsceneView.active) return;
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

            if (_selectedBear != null)
            {
                GameObject gameObject = GameObject.Find(_selectedBear.gameName);
                if (gameObject == null) return; // это костыль мне похер
                camera.transform.position = gameObject.transform.position + offset;
            }
        }
    }

    // void LateUpdate()
    // {
    //     if (_selectedBear != null)
    //     {
    //         GameObject bearGameObject = GameObject.Find(_selectedBear.gameName).gameObject;
    //         Vector3 updatePosition = bearGameObject.transform.position + offset;
    //         camera.transform.position = Vector3.Lerp(
    //             bearGameObject.transform.position,
    //             updatePosition,
    //             smoothSpeed * Time.deltaTime
    //             );
    //     }
    // }

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
                yield return new WaitForSeconds(0.01f);
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
    [TextArea] public string text;
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