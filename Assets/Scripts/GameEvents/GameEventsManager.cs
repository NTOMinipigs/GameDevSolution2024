using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private List<string> _onceGameEventsWasRead = new List<string>();

    private readonly Dictionary<string, GameEvent> _allGameEventsDict = new Dictionary<string, GameEvent>();
    private int _hoursToNextEvent;

    private int _worldHours, _worldMinuts;
    private TextMeshProUGUI _textTime;
    private Light _directLight;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        _textTime = GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>();
        _directLight = GameObject.Find("Directional Light").GetComponent<Light>();
        foreach (GameEvent ge in allGameEvents)
            _allGameEventsDict.Add(ge.gameName, ge);
        // Стартовые значения
        _directLight.intensity = 0.3f;
        _hoursToNextEvent = Random.Range(3, 10);

        StartCoroutine(WorldTimeChange());
    }

    /// <summary>
    /// Управление временем игры
    /// </summary>
    /// <param name="timeScale"></param>
    public void SetTimeScale(float timeScale) => Time.timeScale = timeScale;

    public void ActivateEvent(string eventName) => ActivatingEvent(_allGameEventsDict[eventName]);
    public void ActivateEvent(GameEvent gameEvent) => ActivatingEvent(gameEvent);

    /// <summary>
    /// Активация ивента
    /// </summary>
    /// <param name="selectedEvent"></param>
    private void ActivatingEvent(GameEvent selectedEvent)
    {
        string textReward = "";
        foreach (Reward reward in selectedEvent.eventRewards)
        {
            switch (reward.typeOfReward)
            {
                case Resources.Material:
                    ColonyManager.Singleton.Materials += reward.count;
                    textReward += "+" + Resources.Material + " x" + reward.count + "\n";
                    break;
                case Resources.MaterialPlus:
                    ColonyManager.Singleton.MaterialsPlus += reward.count;
                    textReward += "+" + Resources.MaterialPlus + " x" + reward.count + "\n";
                    break;
                case Resources.Food:
                    ColonyManager.Singleton.Food += reward.count;
                    textReward += "+" + Resources.Food + " x" + reward.count + "\n";
                    break;
                case Resources.Honey:
                    ColonyManager.Singleton.Honey += reward.count;
                    textReward += "+" + Resources.Honey + " x" + reward.count + "\n";
                    break;
                case Resources.BioFuel:
                    ColonyManager.Singleton.Biofuel += reward.count;
                    textReward += "+" + Resources.BioFuel + " x" + reward.count + "\n";
                    break;
                case Resources.Bears:
                    for (int i = 0; i < reward.count; i++)
                    {
                        Bear newBear = new Bear();
                        // Рандомный выбор по ВЫБРАННЫМ традициям
                        int traditionRandom = Random.Range(0, 6);
                        switch (traditionRandom)
                        {
                            case 0:
                                newBear = ColonyManager.Singleton.GenerateNewBear(Traditions.Beekeepers);
                                break;
                            case 1:
                                newBear = ColonyManager.Singleton.GenerateNewBear(Traditions.Constructors);
                                break;
                            case 2:
                                newBear = ColonyManager.Singleton.GenerateNewBear(Traditions.Drone);
                                break;
                            case 4:
                                newBear = ColonyManager.Singleton.GenerateNewBear(Traditions.BioEngineers);
                                break;
                            case 5:
                                newBear = ColonyManager.Singleton.GenerateNewBear(Traditions.Programmers);
                                break;
                        }

                        textReward += "+" + newBear.tradition.GetString() + " " + newBear.bearName + "\n";
                    }

                    break;
            }
        }

        ActivateEventMenu(selectedEvent, textReward);
    }

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
            Time.timeScale = 0.1f;
        }
        else
            Time.timeScale = 1.5f;
    }

    /// <summary>
    /// Функция для UI кнопки
    /// </summary>
    public void CloseEventMenu() => ActivateEventMenu();

    /// <summary>
    /// Обновление времени каждую N секунду + действия
    /// </summary>
    /// <returns></returns>
    private IEnumerator WorldTimeChange()
    {
        _worldMinuts += 10;
        if (_worldMinuts >= 60)
        {
            _worldMinuts = 0;
            _worldHours++;

            _hoursToNextEvent--;
            if (_hoursToNextEvent == 0) // Если пора активировать ивент
            {
                GameEvent newEvent = allGameEvents[Random.Range(0, allGameEvents.Length - 1)];
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
            if (_worldHours == 24)
                _worldHours = 0;

            // Смена глобального света в зависимости от времени. Цикл для плавности
            if (_worldHours < 16)
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

        _textTime.text = _worldHours.ToString("D2") + ":" + _worldMinuts.ToString("D2");
        yield return new WaitForSeconds(3f); // Частота обновления дня
        StartCoroutine(WorldTimeChange());
    }
}