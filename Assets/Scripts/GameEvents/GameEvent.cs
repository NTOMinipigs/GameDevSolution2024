using UnityEngine;

[System.Serializable]
public class GameEvent
{
    /// <summary>
    /// Системное имя
    /// </summary>
    public string gameName;

    /// <summary>
    /// Отображаемое имя
    /// </summary>
    public string eventName;

    /// <summary>
    /// Тип ивента йоу
    /// </summary>
    public TypeOfEvent typeOfEvent;

    /// <summary>
    /// Отображаемое описание ивента
    /// </summary>
    [TextArea(0, 5)] public string eventDescription;

    /// <summary>
    /// Одноразовый ивент?
    /// </summary>
    public bool onceEvent;

    /// <summary>
    /// Ресурсовые награды
    /// </summary>
    public Reward[] eventRewards = new Reward[0];

    /// <summary>
    /// Бедствие
    /// </summary>
    public TypeOfDisaster disaster;

    public GameEvent()
    {
    }

    public GameEvent(string EventName, string EventDescription)
    {
        eventName = EventName;
        eventDescription = EventDescription;
    }
}