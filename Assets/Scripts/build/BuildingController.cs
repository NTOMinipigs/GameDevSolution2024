﻿using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [Header("MainInformation")]
    // Я не придумал, как лучше определять SelectedBuilding. Костыль.
    // Назначается либо buildig, либо resource
    public IBuildable Building;

    [SerializeField] private Building building;
    [SerializeField] private Resource resource;

    /// <summary>
    /// Уже построено?
    /// </summary>
    public bool isBuild;

    /// <summary>
    /// Активно/работает?
    /// </summary>
    public bool isReady;

    public float health = 100f;
    public Vector2Int size;

    /// <summary>
    /// Условный таймер работы
    /// </summary>
    [Header("Workers")] public float steps;

    /// <summary>
    /// Число рабочих в здании
    /// </summary>
    public int workersCount;

    private MeshRenderer _mainRenderer;
    [HideInInspector] public RevealByProgress reveal; // Штука для редактирования материала
    private Color _standardMaterialColor;

    private void Awake()
    {
        _mainRenderer = transform.Find("model").GetComponent<MeshRenderer>();
        _standardMaterialColor = _mainRenderer.material.color;
        reveal = transform.Find("model").GetComponent<RevealByProgress>();
        if (building)
            Building = building;
        if (resource)
            Building = resource;
    }

    #region ChangeMaterial

    // Смена цвета по возможности расстановки
    public void SetTransparent(bool available) => _mainRenderer.material.color = available ? Color.green : Color.red;

    // Смена цвета на нормальный
    public void SetNormal() => _mainRenderer.material.color = _standardMaterialColor;

    // Процесс стройки
    public void SetBuilding() => reveal.progress = 0f;

    #endregion
    
    /// <summary>
    /// Переключаем состояние здания - работает или нет
    /// </summary>
    /// <param name="status">Новое состояние</param>
    public void ChangeIsReady(bool status)
    {
        if (status)
        {
            bool blockToChange = ColonyManager.Singleton.Energy + 1 > ColonyManager.Singleton.MaxEnergy;
            isReady = blockToChange;
            if (!blockToChange)
                ColonyManager.Singleton.Energy++;
        }
        else
        {
            isReady = false;
            ColonyManager.Singleton.Energy--;
        }
    }
    
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
        if (!isReady) return;
        if (workersCount > 0)
        {
            steps += 0.005f;
            if (resource) // Если ресурс
            {
                health -= 0.065f;
                if (health < 0)
                {
                    BearTaskManager.Singleton.FindAndEndTask(Traditions.Drone, gameObject, true);
                    BuildingSystem.Singleton.buildingCreateMenu.SetActive(false);
                    Destroy(gameObject);
                }
            }
        }

        if (steps >= 1)
        {
            steps = 0f;
            float earn = workersCount * Building.ResourceOneWorker;

            // Не стоит отправлять запрос на сервер, если кол-во ресурсов не изменяется
            if (earn == 0)
                return;

            string
                resourceChanged = ""; // Здесь хранится строчное представление ресурса, который изменили. Для логов
            switch (Building.TypeResource)
            {
                case Resources.Material:
                    ColonyManager.Singleton.Materials += earn;
                    resourceChanged = "materials";
                    break;
                case Resources.MaterialPlus:
                    ColonyManager.Singleton.MaterialsPlus += earn;
                    resourceChanged = "materialsPlus";
                    break;
                case Resources.Food:
                    ColonyManager.Singleton.Food += earn;
                    resourceChanged = "food";
                    break;
                case Resources.Honey:
                    ColonyManager.Singleton.Honey += earn;
                    resourceChanged = "honey";
                    break;
                case Resources.BioFuel:
                    ColonyManager.Singleton.Biofuel += earn;
                    resourceChanged = "bioFuel";
                    break;
            }

            if (resource) // Ну то есть это ресуурс
                BuildingSystem.Singleton.UpdateResourceText();
            else if (building)
                BuildingSystem.Singleton.UpdateBuildingText();

            // Лог
            APIClient.Instance.CreateLogRequest(
                "Новые ресурсы произведенные в результате работы некоторого строения",
                Player.Instance.playerName,
                new Dictionary<string, string>() { { resourceChanged, "+" + earn } });
        }
    }
}