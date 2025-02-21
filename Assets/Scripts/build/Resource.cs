using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Resource")]
public class Resource : ScriptableObject, IBuildable
{
    [Header("MainInformation")] public string resourceName;
    public string BuildingName => resourceName;

    public Resources typeResource;
    public Resources TypeResource => typeResource;

    [Header("BuildingSettings")] public int maxDrones;
    public int MaxWorkers => maxDrones;

    public int resourceOneWorker; // сколько ресурсов за одного рабочего
    public int ResourceOneWorker => resourceOneWorker;
    [Range(0, 10f)] public float stepsNeed;
}