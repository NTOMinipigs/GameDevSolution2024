using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Места, которые можно посетить.
/// </summary>
[System.Serializable]
public class PlaceOfTravel
{
    // Задается в редакторе/коде
    [Header("MainInfo")]
    public string nameOfPlace;
    [TextArea] public string description;
    public float timeToGoing; // Сколько надо
    [TextArea] public string resultText;

    // Задается в коде
    [HideInInspector] public int difficulty;
    [HideInInspector] public bool isHome;
    [HideInInspector] public bool placeIsChecked; // Изучено?
    [HideInInspector] public Sprite placeCellIcon;
    [HideInInspector] public float timeNow; // Сколько уже прошли
    [HideInInspector] public List<Reward> rewards = new List<Reward>();

    public PlaceOfTravel()
    {

    }

    public PlaceOfTravel(string NameOfPlace, string DescriptionOfPlace, float TimeToGoing)
    {
        nameOfPlace = NameOfPlace;
        description = DescriptionOfPlace;
        timeToGoing = TimeToGoing;
    }

    public PlaceOfTravel(string NameOfPlace, bool IsHome)
    {
        nameOfPlace = NameOfPlace;
        isHome = IsHome;
    }
}
