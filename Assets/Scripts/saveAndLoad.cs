using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/// <summary>
/// Этот класс отвечает за подгрузку и сохранение прогресса
/// </summary>
public class SaveAndLoad : MonoBehaviour
{
    private allScripts scripts;
    // Часть необходимых методов для инициализации

    void Start()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        scripts = GameObject.Find("scripts").GetComponent<allScripts>();
        bool loadResult = systemSaver.LoadGame();
        // Если файл сохранения не найден
        if (!loadResult)
        {
            CreateNewGame();
        }

        LoadGame();
    }

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
        colonyManager.GenerateChrom();
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Beekeepers);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Programmers);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Constructors);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.BioEngineers);
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

        foreach (BearSave bearSave in systemSaver.gameSave.bearSaves)
        {
            colonyManager.BearSpawn(bearSave);
        }
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
            BearTask.TasksMode tasksMode = BearTask.GetTaskMode((string)task["taskMode"]);
            GameObject objectOfTask = GameObject.Find((string)task["ObjectOfTaskName"]);
            Bear bearObject = null;

            foreach (Bear bear in colonyManager.bearsInColony) // Ищем медведя
            {
                if (bear.gameName == (string)task["BearGameName"])
                {
                    bearObject = bear;
                }
            }

            if (bearObject == null) // Если медведь не найден
            {
                throw new ArgumentException("Invalid bear game name: " + task["BearGameName"]);
            }

            // Инициализируем задачу
            BearTask bearTask = new BearTask(tasksMode, objectOfTask, (float)(double) task["needSteps"]);
            bearTask.selectedBear = bearObject;
            bearTask.totalSteps = (float)(double) task["totalSteps"];
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
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        // Рассчитывается на то, что каждый медведь из bearsInColony, соответствует индексу в json 

        for (int i = 0; i < colonyManager.bearsInColony.Count; i++)
        {
            // Получаем медведей медведя и сейв медведя из списков
            Bear bear = colonyManager.bearsInColony[i];
            BearSave bearSave = systemSaver.gameSave.bearSaves[i];

            if (bear.tradition != TraditionsManager.Traditions.Chrom)
            {
                // Сейвим координаты
                GameObject bearObj = GameObject.Find(bear.gameName);
                bearSave.x = bearObj.transform.position.x;
                bearSave.z = bearObj.transform.position.z;
            }

            // Настроение голод и активность
            bearSave.hungry = bear.hungry;
            bearSave.tired = bear.tired;
            bearSave.activity = ActivityManager.GetStrByActivity(bear.activity);
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
            foreach (var field in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))  // Проходимся по всем публичным филдам, чтобы найти помеченные аттрибутом
            {
                JsonSerializeAttribute attribute = field.GetCustomAttribute<JsonSerializeAttribute>();
                if (attribute != null) // Если у поля есть такой аттрибут, достаем из него значение
                {
                     saveData[attribute.inJsonName] = field.GetValue(bearTask);
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
