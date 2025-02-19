using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Building")]
public class Building : ScriptableObject
{
    [Header("MainInformation")] public string buildingName;
    
    public enum TypesOfBuilding
    {
        Building,
        Resource
    }

    public TypesOfBuilding typeOfBuilding;
    public Resources typeResource;
    [Header("Workers")] public bool canWork;
    public TypeOfWorkers typeOfWorkers;

    public enum TypeOfWorkers
    {
        Any,
        Beekeepers,
        Constructors,
        Programmers,
        BioEngineers,
        Drone
    }

    [Header("BuildingSettings")] public bool scoutHome;
    public Vector2Int size = Vector2Int.one;
    public int maxDrones, maxBears;

    public int
        resourceGive,
        resourceOneWorker; // Сколько здание дает баффов при постройке + после производства +сколько "работы" за одного рабочего

    public float materialsNeed, specMaterialsNeed, stepsNeed, energyNeed;
}