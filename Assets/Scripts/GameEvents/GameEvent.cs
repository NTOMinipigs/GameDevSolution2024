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
    /// Отображаемое описание ивента
    /// </summary>
    [TextArea(0, 5)]public string eventDescription;

    /// <summary>
    /// Одноразовый ивент?
    /// </summary>
    public bool onceEvent;
    
    /// <summary>
    /// Ресурсовые награды
    /// </summary>
    public Reward[] eventRewards = new Reward[0];
    
    public GameEvent()
    {
        
    }

    public GameEvent(string EventName, string EventDescription)
    {
        eventName = EventName;
        eventDescription = EventDescription;
    }
}