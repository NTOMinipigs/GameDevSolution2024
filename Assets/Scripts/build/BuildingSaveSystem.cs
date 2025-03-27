using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Этот класс расширяет возможнности Building-системы, с минимальным воздействием на все остальныее ее компоненты
/// </summary>
[System.Serializable]
public class BuildingSaveSystem : MonoBehaviour
{
    public static BuildingSaveSystem Singleton { get; private set; }
    /// <summary>
    /// Вставьте сюда systemSaver, он используется в коде в дальнейшем
    /// </summary>
    public SystemSaver systemSaver;

    /// <summary>
    /// Вставьте сюда buildingSystem, он используется в коде в дальнейшем  
    /// </summary>
    public BuildingSystem buildingSystem;

    // --------------------- prefabs ------------------------------

    public GameObject sunPanel;
    public GameObject farm;
    public GameObject house;
    public GameObject metallFabric;
    public GameObject beeFabric;
    public GameObject stone;
    public GameObject foodBox;
    public GameObject honeyBox;
    public GameObject bioFuel;
    public GameObject Expedit;
    public GameObject dronesFabric;

    private void Awake()
    {
        Singleton = this;
    }

    /// <summary>
    /// Получите префаб по имени
    /// </summary>
    /// <param name="pfefabName">название префаба</param>
    /// <returns></returns>
    private GameObject GetPrefabByName(string pfefabName)
    {
        return (GameObject)typeof(BuildingSaveSystem).GetField(pfefabName).GetValue(this);
    }

    /// <summary>
    /// Дай мне count случайных координат в диапазоне от minX до maxX и от minY до maxY 
    /// </summary>
    /// <param name="count">Количество требуемых координат</param>
    /// <param name="minX">min X</param>
    /// <param name="maxX">max X</param>
    /// <param name="minY">min Y</param>
    /// <param name="maxY">max Y</param>
    /// <returns>генератор - count раз вернет случайнуюю координату в диапазоне</returns>
    public static IEnumerable<Vector2> GenerateUniqueCoordinates(int count, float minX, float maxX, float minY,
        float maxY)
    {
        HashSet<Vector2> coordinates = new HashSet<Vector2>();

        while (coordinates.Count < count)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector2 newCoord = new Vector2(x, y);

            if (coordinates.Add(newCoord)) // HashSet.Add вернёт true, если элемент уникален
                yield return newCoord;
        }
    }

    /// <summary>
    /// В этом методе опишите логику создания построек, при создании новой игры
    /// В этом методе НЕЛЬЗЯ самостоятельно создавать постройки на сцене, шаманить только с файлом сохранения
    /// </summary>
    public void CreateStartBuilds()
    {
        // Случайная генерация руд
        foreach (Vector2 vector2 in GenerateUniqueCoordinates(5, 80, -80, 80, -80f))
            CreateBuildSave((int)vector2.x, (int)vector2.y, "stone", true, true, Random.Range(0, 360));

        // Случайная генерация боксов с едой
        foreach (Vector2 vector2 in GenerateUniqueCoordinates(3, 80, -80, 80, -80f))
            CreateBuildSave((int)vector2.x, (int)vector2.y, "foodBox", true, true, Random.Range(0, 360));

        // Случайная генерация боксов с медом
        foreach (Vector2 vector2 in GenerateUniqueCoordinates(2, 80, -80, 80, -80f))
            CreateBuildSave((int)vector2.x, (int)vector2.y, "honeyBox", true, true, Random.Range(0, 360));

        // Случайная генерация боксов с био топливом
        foreach (Vector2 vector2 in GenerateUniqueCoordinates(1, 80, -80, 80, -80f))
            CreateBuildSave((int)vector2.x, (int)vector2.y, "bioFuel", true, true, Random.Range(0, 360));

        if (Config.ConfigManager.Instance.config.debug)
        {
            CreateBuildSave(-22, 40, "house", true, true, Random.Range(0, 360));
            CreateBuildSave(-22, 4, "farm", true, true, Random.Range(0, 360));
        }
    }

    /// <summary>
    /// Создаст все постройки из сохранения. Используйте один раз при запуске игры
    /// </summary>
    public void PlaceBuildFromSave()
    {
        BuildingSystem.Singleton.Grid = new BuildingController[BuildingSystem.Singleton.gridSize.x, BuildingSystem.Singleton.gridSize.y];
        foreach (BuildingSave buildingSave in systemSaver.gameSave.buildingSaves)
        {
            // Создаем объект на сцене
            BuildingController buildingController =
                GetPrefabByName(buildingSave.buildingName).GetComponent<BuildingController>();
            buildingSystem.PlaceBuilding(buildingController, buildingSave.x, buildingSave.z);
            buildingController.transform.position = new Vector3(buildingSave.x, 3f, buildingSave.z);
            buildingController.transform.eulerAngles =  new Vector3(0, buildingSave.rotation, 0);
            buildingController.isBuild = buildingSave.isBuild;
            buildingController.isReady = buildingSave.isReady;
            var buildingInWorld = Instantiate(buildingController);
            buildingInWorld.gameObject.name = buildingInWorld.name.Replace("(Clone)", "");
        }
    }

    /// <summary>
    /// Создайте новый сейв постройки
    /// </summary>
    /// <param name="x">x постройки</param>
    /// <param name="z">z постройки</param>
    /// <param name="prefabName">Название префаба, как поле в классе</param>
    /// <param name="isReady">Постройка уже выполняет свои функции?</param>
    /// <param name="isBuild">Постройка уже построена?</param>
    /// <param name="rotation">transofm.rotation.y (поворот объекта по Y</param>
    public void CreateBuildSave(int x, int z, string prefabName, bool isReady, bool isBuild, float rotation)
    {
        // Создаем buildingSave объект, который в последствии будет сохранен
        BuildingSave buildingSave = new BuildingSave();
        buildingSave.x = x;
        buildingSave.z = z;
        buildingSave.buildingName = prefabName;
        buildingSave.isReady = isReady;
        buildingSave.isBuild = isBuild;
        buildingSave.rotation = rotation;

        systemSaver.gameSave.buildingSaves.Add(buildingSave);
    }

    /// <summary>
    /// Удалите building из сохраненных построек
    /// Удаление построек можно реализовать более быстрым, но мне лень
    /// </summary>
    /// <param name="x">x постройки</param>
    /// <param name="z">z постройки</param>
    public void DestroyBuildingSave(int x, int z)
    {
        //  Перебираем все элементы, для того чтобы найти совпадающие координаты
        for (int i = 0; i < systemSaver.gameSave.buildingSaves.Count; i++)
        {
            BuildingSave buildingSaveFromGameSave = systemSaver.gameSave.buildingSaves.ElementAt(i);
            if (buildingSaveFromGameSave.x == x)
            {
                if (buildingSaveFromGameSave.z == z)
                {
                    systemSaver.gameSave.buildingSaves.RemoveAt(i);
                    return;
                }
            }
        }
    }
}