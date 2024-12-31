using UnityEngine;



/// <summary>
/// Все это - карта
/// </summary>
[System.Serializable]
public class PlaceOfTravel
{
    public string gameName, nameOfPlace;
    [TextAreaAttribute] public string description;
    [TextAreaAttribute] public string resultText;
    public float foodNeed;
    public float timeNow; // Сколько уже прошли
    public float timeToGoing; // Сколько надо
    public bool placeIsChecked; // Уже проверили
    [HideInInspector] public GameObject buttonInMap;
    public Reward[] rewards = new Reward[0];
}

[System.Serializable]
public class Reward
{
    public float count;
    public Enums.Resources typeOfReward;
}
