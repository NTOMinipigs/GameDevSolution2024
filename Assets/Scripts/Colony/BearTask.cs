using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

/// <summary>
/// Объект задачи для медведей
/// </summary>
[Serializable]
public class BearTask
{
    [JsonConverter(typeof(StringEnumConverter))] [JsonProperty("taskMode")] public Enums.TasksMode taskMode;
    public GameObject objectOfTask;
    public Bear selectedBear;

    /// <summary>
    /// Текущее
    /// </summary>
    [JsonProperty("totalSteps")] public float totalSteps;
    
    /// <summary>
    /// Колво для завершения задачи
    /// </summary>
    [JsonProperty("needSteps")] public float needSteps;
    
    /// <summary>
    /// Имя объекта к которому применяется таска, приведение к строке для записи в json
    /// </summary>
    [JsonProperty("ObjectOfTaskName")]
    public string taskName => objectOfTask.name;
    
    /// <summary>
    /// Вернет имя медведя в игре в формате строки для записи в json
    /// </summary>
    [JsonProperty("BearGameName")]
    public string bearGameName => selectedBear.gameName;

    public BearTask(Enums.TasksMode taskMode, GameObject objectOfTask, float needSteps)
    {
        this.taskMode = taskMode;
        this.objectOfTask = objectOfTask;
        this.needSteps = needSteps;
    }
}
