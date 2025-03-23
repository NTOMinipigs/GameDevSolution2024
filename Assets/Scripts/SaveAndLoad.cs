using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;


/// <summary>
/// Этот класс отвечает за подгрузку и сохранение прогресса
/// </summary>
public class SaveAndLoad : MonoBehaviour
{
    /// <summary>
    /// Этот флаг указывает на first boot, костыль
    /// </summary>
    private bool firstBoot = false;
    
    // Часть необходимых методов для инициализации

    private void Start()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();

        bool loadResult = systemSaver.LoadGame();
        // Если файл сохранения не найден
        if (!loadResult)
        {
            // Загрузите игру в режиме debug, если 
            if (Config.ConfigManager.Instance.config.debug)
                CreateDebugGame();
            CreateNewGame();
        }

        LoadGame();
        // Загрузите игру в режиме debug, если 
        if (Config.ConfigManager.Instance.config.debug)
            LoadDebugGame();
    }

    /// <summary>
    /// Запустите это если флаг debug в конфиге true
    /// </summary>
    private void CreateDebugGame()
    {
        // Проставляем начальные ресурсы игроку, сразу много чтобы можно было удобно дебажить
        ColonyManager.Singleton.MaxMaterials = 200;
        ColonyManager.Singleton.MaxEnergy = 200;
        ColonyManager.Singleton.MaxBiofuel = 200;
        ColonyManager.Singleton.MaxFood = 200;
        ColonyManager.Singleton.MaxHoney = 200;
        ColonyManager.Singleton.MaxMaterialsPlus = 200;
        ColonyManager.Singleton.MaterialsPlus = 100;
        ColonyManager.Singleton.Materials = 100;
        ColonyManager.Singleton.Energy = 100;
        ColonyManager.Singleton.Biofuel = 100;
        ColonyManager.Singleton.Food = 100;
        ColonyManager.Singleton.Honey = 100;
        // TODO: Добавь сюда дефолтных построек
    }

    /// <summary>
    /// Запустите это если флаг debug в конфиге true, чтобы загрузить игру
    /// </summary>
    private void LoadDebugGame()
    {
        // Убиваем фпс чтобы игра не жрала много
        Application.targetFrameRate = 20;
        Destroy(Camera.main.GetComponentInChildren<ParticleSystem>()); // Убираем снег
    }

    /// <summary>
    /// При закрытии игры сработает это
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// Вызывает методы загрузки игры, например спавн медведей из памяти
    /// Оставьте этот метод приватным, он должен вызываться только один раз в одном месте
    /// </summary>
    private void LoadGame()
    {
        LoadBears();
        LoadBuilds();
        //LoadTasks();
        LoadPreference();
        LoadInventory();
        LoadGameEvents();
        if (firstBoot) QuestSystem.Singleton.StartFirst();
    }

    /// <summary>
    /// Вызывает методы сохранения игры.
    /// Оставьте его публичным, чтобы игру можно было сохранять из любого класса
    /// </summary>
    public void SaveGame()
    {
        SaveBears();
        //SaveTasks();
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        systemSaver.SaveGame();
    }


    # region Create

    /// <summary>
    /// Вызывает методы создания игры, например создает медведей постройки и т.д.
    /// </summary>
    private void CreateNewGame()
    {
        CreateBears();
        CreateBuilds();
        CreatePreference();
        CreateInventory();
        CreateGameEvents();
        firstBoot = true;
    }

    /// <summary>
    /// СОЗДАСТ новых медведей в игре
    /// </summary>
    private void CreateBears()
    {
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        colonyManager.GenerateSpecialBear("Хром", GameObject.Find("BearChrom_0"), Traditions.Chrom);
        colonyManager.GenerateNewBear(Traditions.Beekeepers);
        colonyManager.GenerateNewBear(Traditions.Programmers);
        colonyManager.GenerateNewBear(Traditions.Constructors);
        colonyManager.GenerateNewBear(Traditions.BioEngineers);
        colonyManager.GenerateNewBear(Traditions.Drone);
        colonyManager.GenerateNewBear(Traditions.Drone);
        colonyManager.GenerateNewBear(Traditions.Drone);
        colonyManager.GenerateNewBear(Traditions.Drone);
    }

    /// <summary>
    /// Создать постройки в игре, если они не были созданы до этого
    /// </summary>
    private void CreateBuilds()
    {
        BuildingSaveSystem.Singleton.CreateStartBuilds();
    }

    private void CreatePreference()
    {
        SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity = 5f;
        SystemSaver.Singleton.gameSave.PreferenceSave.globalVolume = 100f;
        SystemSaver.Singleton.gameSave.PreferenceSave.postProcessing = true;
    }

    /// <summary>
    /// Создайте инвентарь если его еще нет
    /// </summary>
    private void CreateInventory()
    {
    Dictionary<string, int> initInventory = new()
    {
        {"materials", 0},
        {"food", 10},
        {"bioFuel", 15},
        {"honey", 0},
        {"materialPlus", 0},
        {"energy", 0},
        {"maxMaterials", 50},
        {"maxFood", 10},
        {"maxBioFuel", 15},
        {"maxHoney", 0},
        {"maxBears", 10},
        {"maxMaterialPlus", 0},
        {"maxEnergy", 0}
    };
        
        APIClient.Instance
            .CreatePlayerRequest(
                Player.Instance.playerName,
                initInventory);
    }

    /// <summary>
    /// Создайте новые значения для GameEventsManager
    /// </summary>
    private void CreateGameEvents()
    {
        GameEventsManager.Singleton.worldHours = 0;
        GameEventsManager.Singleton.worldMinuts = 0;
        GameEventsManager.Singleton.WorldTemperature = 0f;
    }

    #endregion

    # region load

    /// <summary>
    /// Загрузит медведей из json'а
    /// </summary>
    private void LoadBears()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();

        foreach (Bear bear in systemSaver.gameSave.bears)
            colonyManager.BearSpawn(bear);
        colonyManager.UpdateWorkersCount();
    }

    /// <summary>
    /// Подргрузка тасков из json
    /// </summary>
    [Obsolete("Таски больше не сохраняются")]
    private void LoadTasks()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();

        // Проходимся по всем сохраненным таскам
        foreach (Dictionary<string, object> task in systemSaver.gameSave.tasksSaves)
        {
            // Инициализируем все нужные переменные для создания задачи
            TasksMode tasksMode = (TasksMode)Enum.Parse(typeof(TasksMode), (string)task["taskMode"]);
            GameObject objectOfTask = GameObject.Find((string)task["ObjectOfTaskName"]);
            Bear bearObject = null;

            foreach (Bear bear in colonyManager.bearsInColony) // Ищем медведя
            {
                if (bear.gameName == (string)task["BearGameName"])
                    bearObject = bear;
            }

            if (bearObject == null) // Если медведь не найден
                throw new ArgumentException("Invalid bear game name: " + task["BearGameName"]);

            // Инициализируем задачу
            BearTask bearTask =
                new BearTask(tasksMode, objectOfTask, Traditions.None, (float)(double)task["needSteps"]);
            bearTask.selectedBear = bearObject;
            bearTask.totalSteps = (float)(double)task["totalSteps"];
        }
    }


    /// <summary>
    /// Загрузить постройки из json
    /// </summary>
    private void LoadBuilds()
    {
        BuildingSaveSystem buildingSaveSystem = gameObject.GetComponent<BuildingSaveSystem>();
        buildingSaveSystem.PlaceBuildFromSave();
    }


    /// <summary>
    /// Загрузить настройки из json
    /// </summary>
    private void LoadPreference()
    {
        Preference.Singleton.globalVolume = SystemSaver.Singleton.gameSave.PreferenceSave.globalVolume;
        Preference.Singleton.sensitivityOfCamera = SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity;
        Preference.Singleton.postProcessing = SystemSaver.Singleton.gameSave.PreferenceSave.postProcessing;
    }


    /// <summary>
    /// Загрузите инвентарь
    /// </summary>
    private async Task LoadInventory()
    {
        APIClient.UserInventory inventory =
            await APIClient.Instance.GetUserInventoryRequest(Player.Instance.playerName);
        if (inventory == null) return;
        ColonyManager.Singleton.SetInventory(inventory);
    }

    /// <summary>
    /// Загрузка GameEventsManager (время и температура)
    /// </summary>
    private void LoadGameEvents()
    {
        GameEventsManager.Singleton.WorldTemperature = SystemSaver.Singleton.gameSave.temperature;
        GameEventsManager.Singleton.worldHours = SystemSaver.Singleton.gameSave.hours;
        GameEventsManager.Singleton.worldMinuts = SystemSaver.Singleton.gameSave.minutes;
    }
    
    #endregion

    #region save

    /// <summary>
    /// Сохранить медведей, сведения о них, например позиции
    /// </summary>
    private void SaveBears()
    {
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        // Рассчитывается на то, что каждый медведь из bearsInColony, соответствует индексу в json 

        for (int i = 0; i < colonyManager.bearsInColony.Count; i++)

            // Получаем медведей медведя и сейв медведя из списков
            foreach (Bear bear in colonyManager.bearsInColony)
            {
                if (bear.tradition != Traditions.Chrom)
                {
                    // Сейвим координаты
                    GameObject bearObj = GameObject.Find(bear.gameName);
                    bear.x = bearObj.transform.position.x;
                    bear.z = bearObj.transform.position.z;
                }
            }
    }

    /// <summary>
    /// Сохранить задачи медведя в json
    /// </summary>
    [Obsolete("Таски больше не сохраняются")]
    private void SaveTasks()
    {
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();

        if (colonyManager.bearTasks.Count == 0) return;

        Type type = colonyManager.bearTasks[0].GetType();
        foreach (BearTask bearTask in colonyManager.bearTasks) // Проходимся по всем медведям
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();
            // Проходимся по всем публичным филдам, чтобы найти помеченные аттрибутом
            foreach (var field in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                JsonPropertyAttribute attribute = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute != null) // Если у поля есть такой аттрибут, достаем из него значение
                {
                    saveData[attribute.PropertyName] = field.GetValue(bearTask);
                }
            }

            systemSaver.gameSave.tasksSaves.Add(saveData);
        }
    }

    #endregion
}