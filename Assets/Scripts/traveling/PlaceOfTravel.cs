using UnityEngine;

/// <summary>
/// Места, которые можно посетить.
/// </summary>
[System.Serializable]
public class PlaceOfTravel
{
    // Задается в редакторе/коде
    [Header("MainInfo")]
    public string gameName;
    public string nameOfPlace;
    [TextArea] public string description;
    public float timeToGoing; // Сколько надо

    // Переделать по сложности клетки
    public Reward[] rewards = new Reward[0];
    [TextArea] public string resultText;

    // Задается в коде
    [HideInInspector] public bool isHome;
    [HideInInspector] public bool placeIsChecked; // Уже проверили
    [HideInInspector] public float timeNow; // Сколько уже прошли

    public PlaceOfTravel()
    {

    }

    public PlaceOfTravel(string GameName, string NameOfPlace, bool IsHome)
    {
        gameName = GameName;
        nameOfPlace = NameOfPlace;
        isHome = IsHome;
    }
}
