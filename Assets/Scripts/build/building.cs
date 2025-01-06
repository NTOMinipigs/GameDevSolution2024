using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("MainInformation")]
    public string buildingName;
    public bool builded;
    public enum TypesOfBuilding { building, resource }
    public TypesOfBuilding typeOfBuilding;
    public Resources typeResource;
    [Header("Workers")]
    public bool canWork;
    public float steps; // Текущее кол-во "работы" до обнуления
    public TypeOfWorkers typeOfWorkers;
    public enum TypeOfWorkers { Any, Beekeepers, Constructors, Programmers, BioEngineers, Drone }
    [HideInInspector] public int countOfBears, countOfDrone;
    [Header("BuildingSettings")]
    public bool scoutHome;
    [SerializeField] private Renderer MainRenderer;
    public Vector2Int Size = Vector2Int.one;
    public int maxDrones, maxBears;
    public int resourceGive, resourseOneWorker; // Сколько здание дает баффов при постройке + после производства +сколько "работы" за одного рабочего
    public float materialsNeed, specMaterialsNeed, stepsNeed, energyNeed;
    private Color standartMaterialColor;
    private allScripts scripts;

    private void Awake()
    {
        standartMaterialColor = MainRenderer.material.color;
        scripts = GameObject.Find("scripts").GetComponent<allScripts>();
    }

    // Смена цвета по возможности расстановки
    public void SetTransparent(bool available) => MainRenderer.material.color = available ? Color.green : Color.red;

    // Смена цвета на нормальный
    public void SetNormal() => MainRenderer.material.color = standartMaterialColor;

    // Стройка
    public void SetBuilding() => MainRenderer.material.color = Color.black;

    // Это ебать не должно
    private void OnDrawGizmosSelected()
    {
        Vector3 offset = new Vector3(-Size.x * 0.5f + 0.5f, 0, -Size.y * 0.5f + 0.5f);

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                Gizmos.color = (x + y) % 2 == 0 ? new Color(0.88f, 0f, 1f, 0.3f) : new Color(1f, 0.68f, 0f, 0.3f);
                Gizmos.DrawCube(transform.position + offset + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
            }
        }
    }

    /// <summary>
    /// Заработок с заводов, ферм и прочего
    /// </summary>
    private void FixedUpdate()
    {
        if (builded && typeOfBuilding == TypesOfBuilding.building && canWork)
        {
            steps += 0.0005f;
            if (steps >= 1)
            {
                steps = 0f;
                float earn = (countOfBears + countOfDrone) * resourseOneWorker;
                string resourceChanged = ""; // Здесь хранится строчное представление ресурса, который изменили. Для логов
                switch (typeResource)
                {
                    case Resources.Material:
                        scripts.colonyManager.Materials += earn;
                        resourceChanged = "materials";
                        break;
                    case Resources.MaterialPlus:
                        scripts.colonyManager.materialsPlus += earn;
                        resourceChanged = "materialsPlus";
                        break;
                    case Resources.Food:
                        scripts.colonyManager.Food += earn;
                        resourceChanged = "food";
                        break;
                    case Resources.Honey:
                        scripts.colonyManager.Honey += earn;
                        resourceChanged = "honey";
                        break;
                    case Resources.BioFuel:
                        scripts.colonyManager.Biofuel += earn;
                        resourceChanged = "bioFuel";
                        break;
                }

                // Лог
                APIClient.Instance.CreateLogRequest(
                    "Новые ресурсы произведенные в результате работы некоторого строения",
                    Player.Instance.playerName,
                    new Dictionary<string, string>() {{resourceChanged, "+" + earn}});
            }
        }
    }
}