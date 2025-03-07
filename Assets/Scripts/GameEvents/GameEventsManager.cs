using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameEventsManager : MonoBehaviour
{
    public GameObject gameEventsMenu;
    [SerializeField] private TextMeshProUGUI textEventName, textEventDescription;
    [SerializeField] private GameEvent[] allGameEvents = new GameEvent[0];
    private readonly Dictionary<string, GameEvent> _allGameEventsDict = new Dictionary<string, GameEvent>();

    private int _worldHours, _worldMinuts;
    private TextMeshProUGUI _textTime;
    private Light _directLight;
    private AllScripts _scripts;

    private void Start()
    {
        _textTime = GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>();
        _directLight = GameObject.Find("Directional Light").GetComponent<Light>();
        _directLight.intensity = 0.3f; // Стартовое значение
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        foreach (GameEvent ge in allGameEvents)
            _allGameEventsDict.Add(ge.gameName, ge);

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
        ActivateEventMenu(selectedEvent);
        foreach (Reward reward in selectedEvent.eventRewards)
        {
            switch (reward.typeOfReward)
            {
                case Resources.Material:
                    _scripts.colonyManager.Materials += reward.count;
                    break;
                case Resources.MaterialPlus:
                    _scripts.colonyManager.MaterialsPlus += reward.count;
                    break;
                case Resources.Food:
                    _scripts.colonyManager.Food += reward.count;
                    break;
                case Resources.Honey:
                    _scripts.colonyManager.Honey += reward.count;
                    break;
                case Resources.BioFuel:
                    _scripts.colonyManager.Biofuel += reward.count;
                    break;
                case Resources.Bears:
                    for (int i = 0; i < reward.count; i++)
                    {
                        // Получаем все значения перечисления Traditions
                        Traditions[] traditions = (Traditions[])System.Enum.GetValues(typeof(Traditions));

                        // Генерируем случайный индекс
                        int randomIndex = Random.Range(0, traditions.Length);
                        _scripts.colonyManager.GenerateNewBear(traditions[randomIndex]);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Управление состоянием меню с ивентами
    /// </summary>
    /// <param name="gameEvent">Игровой ивент</param>
    public void ActivateEventMenu(GameEvent gameEvent = null)
    {
        gameEventsMenu.gameObject.SetActive(gameEventsMenu.activeSelf);
        if (gameEventsMenu.activeSelf && gameEvent != null)
        {
            textEventName.text = gameEvent.eventName;
            textEventDescription.text = gameEvent.eventDescription;
            Time.timeScale = 0.1f;
        }
        else
            Time.timeScale = 1f;
    }

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
        yield return new WaitForSeconds(2.5f); // Частота обновления дня
        StartCoroutine(WorldTimeChange());
    }
}