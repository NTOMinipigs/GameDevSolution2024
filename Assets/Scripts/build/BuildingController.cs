using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [Header("MainInformation")] public Building building;
    public bool isReady;

    [Header("Workers")] public float steps; // Текущее кол-во "работы" до обнуления

    public int workersOfBears, workersOfDrone;

    [SerializeField] private MeshRenderer mainRenderer;
    private Color _standartMaterialColor;
    private AllScripts _scripts;

    private void Awake()
    {
        _standartMaterialColor = mainRenderer.material.color;
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
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
        Vector3 offset = new Vector3(-building.size.x * 0.5f + 0.5f, 0, -building.size.y * 0.5f + 0.5f);

        for (int x = 0; x < building.size.x; x++)
        {
            for (int y = 0; y < building.size.y; y++)
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
        if (!isReady || building.typeOfBuilding != Building.TypesOfBuilding.Building || !building.canWork) return;
        steps += 0.0005f;
        if (steps >= 1)
        {
            steps = 0f;
            float earn = (workersOfBears + workersOfDrone) * building.resourceOneWorker;
            string resourceChanged = ""; // Здесь хранится строчное представление ресурса, который изменили. Для логов
            switch (building.typeResource)
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
                new Dictionary<string, string>() { { resourceChanged, "+" + earn } });
        }
    }
}