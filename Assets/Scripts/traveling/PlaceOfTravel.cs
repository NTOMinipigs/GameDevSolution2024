using UnityEngine;

/// <summary>
/// Места, которые можно посетить.
/// </summary>
[System.Serializable]
public class PlaceOfTravel
{
    public string gameName, nameOfPlace;
    [TextArea] public string description;
    [TextArea] public string resultText;
    public float foodNeed;
    public float timeNow; // Сколько уже прошли
    public float timeToGoing; // Сколько надо
    public bool placeIsChecked; // Уже проверили
    [HideInInspector] public GameObject buttonInMap;
    public Reward[] rewards = new Reward[0];
}
