using System;
using System.Collections;
using System.Collections.Generic;
using Alerts;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

/// <summary>
/// Класс, контролирующий все события в игре
/// </summary>
public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Singleton { get; private set; }
    [Header("Event menu")] public GameObject gameEventsMenu;
    [SerializeField] private TextMeshProUGUI textEventName, textEventDescription;

    [Header("GameEvents")] [SerializeField]
    private GameEvent[] allGameEvents = new GameEvent[0];

    private readonly List<string> _onceGameEventsWasRead = new List<string>();

    private readonly Dictionary<string, GameEvent> _allGameEventsDict = new Dictionary<string, GameEvent>();

    [Header("WorldEvents")]
    public float WorldTemperature
    {
        get => _worldTemperature;
        set
        {
            _worldTemperature = value;
            _textTemperature.text = _worldTemperature + "\u00b0С";
        }
    }

    private float _worldTemperature;
    private TextMeshProUGUI _textTemperature;

    public int gameDay, worldHours, worldMinuts;

    private int _hoursToNextEvent;
    private TextMeshProUGUI _textTime;
    private Light _directLight;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        _textTime = GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>();
        _textTemperature = GameObject.Find("TextTemperature").GetComponent<TextMeshProUGUI>();
        _directLight = GameObject.Find("Directional Light").GetComponent<Light>();
        foreach (GameEvent ge in allGameEvents)
            _allGameEventsDict.Add(ge.gameName, ge);
        // Стартовые значения
        _directLight.intensity = 0.3f;
        WorldTemperature = -25f;

        _hoursToNextEvent = Random.Range(3, 10);

        StartCoroutine(WorldTimeChange());
    }

    /// <summary>
    /// Управление временем игры
    /// </summary>
    /// <param name="timeScale"></param>
    public void SetTimeScale(float timeScale) => Time.timeScale = timeScale;

    # region Activation of events

    public void ActivateEvent(string eventName) => ActivatingEvent(_allGameEventsDict[eventName]);
    public void ActivateEvent(GameEvent gameEvent) => ActivatingEvent(gameEvent);

    /// <summary>
    /// Активация ивента
    /// </summary>
    /// <param name="selectedEvent"></param>
    private void ActivatingEvent(GameEvent selectedEvent)
    {
        string textReward = selectedEvent.typeOfEvent switch
        {
            TypeOfEvent.Disaster => ActivateDisaster(selectedEvent.disaster),
            TypeOfEvent.ChangeBearCharacter => ChangeBearCharacter(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        if (selectedEvent.eventRewards.Length > 0)
            ColonyManager.Singleton.GiveRewards(selectedEvent.eventRewards);
        
        ActivateEventMenu(selectedEvent, textReward);
    }

    /// <summary>
    /// Активировать бедствие тун тун тун тун тун
    /// </summary>
    /// <param name="disaster">Само бедствие</param>
    /// <returns>Возращает строку с бедствием</returns>
    private string ActivateDisaster(TypeOfDisaster disaster)
    {
        string textReward = "";
        if (disaster == TypeOfDisaster.ChangeOfTemperature)
            textReward = ChangeTemperature();

        return textReward;
    }

    /// <summary>
    /// Активирует выдачу черты характеру медведю/медведям
    /// </summary>
    private string ChangeBearCharacter()
    {
        Bear bear = ColonyManager.Singleton.bearsInColony[Random.Range(0, ColonyManager.Singleton.bearsInColony.Count)];
        string textReward = bear.AddRandomCharacters();
        return bear.bearName + " получает черту характера " + textReward;
    }

    /// <summary>
    /// Рандомно(от -10 до 10) изменить температуру
    /// </summary>
    /// <returns></returns>
    private string ChangeTemperature()
    {
        float newTemperature = Random.Range(-10, 10);
        WorldTemperature += newTemperature;

        // Конструкция чисто ради знака
        if (newTemperature >= 0)
            return "+" + newTemperature + "\u00b0С";
        else
            return newTemperature + "\u00b0С";
    }

    # endregion

    # region EventMenu

    /// <summary>
    /// Управление состоянием меню с ивентами
    /// </summary>
    /// <param name="gameEvent">Игровой ивент, заданный в коде</param>
    /// <param name="textReward">Поулченные награды</param>
    public void ActivateEventMenu(GameEvent gameEvent = null, string textReward = "")
    {
        gameEventsMenu.gameObject.SetActive(!gameEventsMenu.activeSelf);
        if (gameEventsMenu.activeSelf && gameEvent != null)
        {
            textEventName.text = gameEvent.eventName;
            textEventDescription.text = gameEvent.eventDescription;
            textEventDescription.text = textEventDescription.text.Replace("{reward}", textReward);
            Time.timeScale = 0.5f;
        }
        else
            Time.timeScale = 1.5f;
    }

    /// <summary>
    /// Функция для UI кнопки
    /// </summary>
    public void CloseEventMenu() => ActivateEventMenu();

    #endregion

    /// <summary>
    /// Обновление времени каждую N секунду + действия
    /// </summary>
    /// <returns></returns>
    private IEnumerator WorldTimeChange()
    {
        worldMinuts += 10;
        if (worldMinuts >= 60)
        {
            worldMinuts = 0;
            worldHours++;

            _hoursToNextEvent--;
            if (_hoursToNextEvent == 0) // Если пора активировать ивент
            {
                GameEvent newEvent = allGameEvents[Random.Range(0, allGameEvents.Length)];
                bool canActivateEvent = true;
                if (newEvent.onceEvent) // Если ивент одноразовый
                {
                    foreach (string eventCheckName in _onceGameEventsWasRead)
                    {
                        if (eventCheckName == newEvent.eventName)
                            canActivateEvent = false;
                    }

                    // Если не нашелся - то добавляем
                    if (canActivateEvent)
                        _onceGameEventsWasRead.Add(newEvent.gameName);
                }

                _hoursToNextEvent = Random.Range(5, 20);

                if (canActivateEvent)
                    ActivateEvent(newEvent);
            }

            // Начало нового дня
            if (worldHours == 24)
            {
                WorldTemperature += Random.Range(-10f, 6.5f);
                AlertsManager.Singleton.ShowAlert("Температура изменилась до " + WorldTemperature);
                gameDay++;
                worldHours = 0;
            }

            // Смена глобального света в зависимости от времени. Цикл для плавности
            if (worldHours < 16)
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return new WaitForSeconds(0.00000001f);
                    _directLight.intensity += 0.0035f;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return new WaitForSeconds(0.00000001f);
                    _directLight.intensity -= 0.006f;
                }
            }
        }

        _textTime.text = worldHours.ToString("D2") + ":" + worldMinuts.ToString("D2");
        yield return new WaitForSeconds(3f); // Частота обновления дня
        StartCoroutine(WorldTimeChange());
    }
}