using System.Linq;
using UnityEngine;

/// <summary>
/// Этот класс расширяет возможнности Building-системы, с минимальным воздействием на все остальныее ее компоненты
/// </summary>
[System.Serializable]
public class BuildingSaveSystem : MonoBehaviour
{
    /// <summary>
    /// Вставьте сюда systemSaver, он используется в коде в дальнейшем
    /// </summary>
    public SystemSaver _systemSaver;

    /// <summary>
    /// Вставьте сюда buildingSystem, он используется в коде в дальнейшем  
    /// </summary>
    public BuildingSystem _buildingSystem;

    // --------------------- prefabs ------------------------------
    
    public GameObject farm;
    public GameObject house;

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
    /// В этом методе опишите логику создания построек, при создании новой игры
    /// В этом методе НЕЛЬЗЯ самостоятельно создавать постройки на сцене, шаманить только с файлом сохранения
    /// </summary>
    public void CreateStartBuilds()
    {
        CreateBuildSave(-18, 7, "house");
        CreateBuildSave(-22, 4, "farm");
    }
    
    /// <summary>
    /// Создаст все постройки из сохранения. Используйте один раз при запуске игры
    /// </summary>
    public void PlaceBuildFromSave()
    {
        foreach (BuildingSave buildingSave in _systemSaver.gameSave.buildingSaves)
        {
            // Создаем объект на сцене
            BuildingController buildingController = GetPrefabByName(buildingSave.buildingName).GetComponent<BuildingController>();
            _buildingSystem.PlaceBuilding(buildingController, buildingSave.x, buildingSave.z);
            buildingController.transform.position.Set(buildingSave.x, 0, buildingSave.z);
            Instantiate(buildingController);
        }
    }

    /// <summary>
    /// Создайте новый сейв постройки
    /// </summary>
    /// <param name="x">x постройки</param>
    /// <param name="z">z постройки</param>
    /// <param name="prefabName">Название префаба, как поле в классе</param>

    public void CreateBuildSave(int x, int z, string prefabName)
    {
        // Создаем buildingSave объект, который в последствии будет сохранен
        BuildingSave buildingSave = new BuildingSave();
        buildingSave.x = x;
        buildingSave.z = z;
        buildingSave.buildingName = prefabName;
        
        _systemSaver.gameSave.buildingSaves.Add(buildingSave);
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
        for (int i = 0; i < _systemSaver.gameSave.buildingSaves.Count; i++)
        {
            BuildingSave buildingSaveFromGameSave = _systemSaver.gameSave.buildingSaves.ElementAt(i);
            if (buildingSaveFromGameSave.x == x)
            {
                if (buildingSaveFromGameSave.z == z)
                {
                    _systemSaver.gameSave.buildingSaves.RemoveAt(i);
                    return;
                }
            }
        }

    }

}