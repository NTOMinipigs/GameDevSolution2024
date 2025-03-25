using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Building")]
public class Building : ScriptableObject, IBuildable
{
    [Header("MainInformation")] public string buildingName;
    [TextArea(0, 10)] public string description;
    public string Description => description;
    public string BuildingName => buildingName;
    public Resources typeResource;
    public Resources TypeResource => typeResource;


    /// <summary>
    /// В здании можно работать?
    /// </summary>
    [Header("Workers")] public bool canWork;

    /// <summary>
    /// Касты, которые могут тут работать
    /// </summary>
    public Traditions typeOfWorkers;

    public int maxWorkers;
    public int MaxWorkers => maxWorkers;

    /// <summary>
    /// Здание - является исследовательским?
    /// </summary>
    [Header("BuildingSettings")] public bool scoutHome;

    /// <summary>
    /// Сколько ресурсов(или + к лимитам) дает здание при постройке
    /// </summary>
    public int resourceGive;

    /// <summary>
    /// Сколько дает ресурсов один рабочий
    /// </summary>
    public int resourceOneWorker;

    public int ResourceOneWorker => resourceOneWorker;
    public float timeToChange; 
    public float TimeToChange => timeToChange;

    /// <summary>
    /// Количество материалов для постройки
    /// </summary>
    public float materialsNeed;

    /// <summary>
    /// Количество спец материалов для постройки
    /// </summary>
    public float specMaterialsNeed;

    /// <summary>
    /// Количество энергии, для работы здания
    /// </summary>
    public float energyNeed;

    /// <summary>
    /// Количество "Работы" для постройки здания
    /// </summary>
    [Range(0, 10f)] public float stepsNeed;
}