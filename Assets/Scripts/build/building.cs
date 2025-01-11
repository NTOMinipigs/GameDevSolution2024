using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("MainInformation")]
    public string buildingName;
    public bool builded;
    public enum TypesOfBuilding { Building, Resource }
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
    [SerializeField] private Renderer mainRenderer;
    public Vector2Int size = Vector2Int.one;
    public int maxDrones, maxBears;
    public int resourceGive, resourceOneWorker; // Сколько здание дает баффов при постройке + после производства +сколько "работы" за одного рабочего
    public float materialsNeed, specMaterialsNeed, stepsNeed, energyNeed;
    private Color _standartMaterialColor;
    private allScripts _scripts;

    private void Awake()
    {
        _standartMaterialColor = mainRenderer.material.color;
        _scripts = GameObject.Find("scripts").GetComponent<allScripts>();
    }

    // Смена цвета по возможности расстановки
    public void SetTransparent(bool available) => mainRenderer.material.color = available ? Color.green : Color.red;

    // Смена цвета на нормальный
    public void SetNormal() => mainRenderer.material.color = _standartMaterialColor;

    // Процесс стройки
    public void SetBuilding() => mainRenderer.material.color = Color.black;

    // Отрисовка в editor юнити сетки строения
    private void OnDrawGizmosSelected()
    {
        Vector3 offset = new Vector3(-size.x * 0.5f + 0.5f, 0, -size.y * 0.5f + 0.5f);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
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
        if (!builded || typeOfBuilding != TypesOfBuilding.Building || !canWork) return;
        steps += 0.0005f;
        if (steps >= 1)
        {
            steps = 0f;
            float earn = (countOfBears + countOfDrone) * resourceOneWorker;
            string resourceChanged = ""; // Здесь хранится строчное представление ресурса, который изменили. Для логов
            switch (typeResource)
            {
                case Resources.Material:
                    _scripts.colonyManager.Materials += earn;
                    resourceChanged = "materials";
                    break;
                case Resources.MaterialPlus:
                    _scripts.colonyManager.MaterialsPlus += earn;
                    resourceChanged = "materialsPlus";
                    break;
                case Resources.Food:
                    _scripts.colonyManager.Food += earn;
                    resourceChanged = "food";
                    break;
                case Resources.Honey:
                    _scripts.colonyManager.Honey += earn;
                    resourceChanged = "honey";
                    break;
                case Resources.BioFuel:
                    _scripts.colonyManager.Biofuel += earn;
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