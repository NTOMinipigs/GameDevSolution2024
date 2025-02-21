using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Building")]
public class Building : ScriptableObject, IBuildable
{
    [Header("MainInformation")] public string buildingName;
    public string BuildingName => buildingName;
    public Resources typeResource;
    public Resources TypeResource => typeResource;
    [Header("Workers")] public bool canWork;
    public Traditions typeOfWorkers;

    [Header("BuildingSettings")] public bool scoutHome;
    public int maxWorkers;
    public int MaxWorkers => maxWorkers;

    public int
        resourceGive,
        resourceOneWorker; // Сколько здание дает баффов при постройке + после производства +сколько "работы" за одного рабочего

    public int ResourceOneWorker => resourceOneWorker;
    public float materialsNeed, specMaterialsNeed, energyNeed;
    [Range(0, 10f)] public float stepsNeed;
}