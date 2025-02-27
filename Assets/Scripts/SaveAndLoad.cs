using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;


/// <summary>
/// Этот класс отвечает за подгрузку и сохранение прогресса
/// </summary>
public class SaveAndLoad : MonoBehaviour
{
    private AllScripts scripts;
    // Часть необходимых методов для инициализации

    void Start()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        
        bool loadResult = systemSaver.LoadGame();
        // Если файл сохранения не найден
        if (!loadResult)
        {
            // Загрузите игру в режиме debug, если 
            if (Config.ConfigManager.Instance.config.debug)
            {
                CreateDebugGame();  
            } 
            CreateNewGame();
        }
        
        // Загрузите игру в режиме debug, если 
        if (Config.ConfigManager.Instance.config.debug)
        {
            LoadDebugGame();  
        } 
        LoadGame();
    }

    /// <summary>
    /// Запустите это если флаг debug в конфиге true
    /// </summary>
    void CreateDebugGame()
    {
        // Проставляем начальные ресурсы игроку, сразу много чтобы можно было удобно дебажить
        scripts.colonyManager.Materials = 100;
        scripts.colonyManager.Energy = 100;
        scripts.colonyManager.Biofuel = 100;
        scripts.colonyManager.Food = 100;
        scripts.colonyManager.Honey = 100;
        scripts.colonyManager.MaterialsPlus = 100;
        scripts.colonyManager.MaxMaterials = 200;
        scripts.colonyManager.MaxEnergy = 200;
        scripts.colonyManager.MaxBiofuel = 200;
        scripts.colonyManager.MaxFood = 200;
        scripts.colonyManager.MaxHoney = 200;
        scripts.colonyManager.MaxMaterialsPlus = 200;
        
        // TODO: Добавь сюда дефолтных построек
    }

    /// <summary>
    /// Запустите это если флаг debug в конфиге true, чтобы загрузить игру
    /// </summary>
    void LoadDebugGame()
    {
        // Убиваем фпс чтобы игра не жрала много
        Application.targetFrameRate = 20;
        Destroy(Camera.main.GetComponentInChildren<ParticleSystem>()); // Убираем снег
    }

    /// <summary>
    /// При закрытии игры сработает это
    /// </summary>
    void OnApplicationQuit()
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
        LoadTasks();
    }

    /// <summary>
    /// Вызывает методы сохранения игры.
    /// Оставьте его публичным, чтобы игру можно было сохранять из любого класса
    /// </summary>
    public void SaveGame()
    {
        SaveBears();
        SaveTasks();
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        systemSaver.SaveGame();
    }


    // Часть иниициализации (Create)

    /// <summary>
    /// Вызывает методы создания игры, например создает медведей постройки и т.д.
    /// </summary>
    private void CreateNewGame()
    {
        CreateBears();
        scripts.questSystem.StartFirst();
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
    }

    /// <summary>
    /// Создать постройки в игре, если они не были созданы до этого
    /// </summary>
    private void CreateBuilds()
    {
    }


    // Часть Загрузки (Load)

    /// <summary>
    /// Загрузит медведей из json'а
    /// </summary>
    private void LoadBears()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();

        foreach (Bear bear in systemSaver.gameSave.bears)
            colonyManager.BearSpawn(bear);
    }

    /// <summary>
    /// Подргрузка тасков из json
    /// </summary>
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
            BearTask bearTask = new BearTask(tasksMode, objectOfTask, (float)(double)task["needSteps"]);
            bearTask.selectedBear = bearObject;
            bearTask.totalSteps = (float)(double)task["totalSteps"];
        }
    }

    /// <summary>
    /// Загрузить постройки из json
    /// </summary>
    private void LoadBuilds()
    {
    }


    // Часть сохранения (Save)

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

    /// <summary>
    /// Сохранять постройки в json
    /// </summary>
    private void SaveBuilds()
    {
    }
}