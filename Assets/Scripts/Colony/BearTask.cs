using System;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Объект задачи для медведей
/// </summary>
[Serializable]
public class BearTask
{
    public enum TasksMode { None, build, destroy, getResource, create}
    public TasksMode taskMode;
    public GameObject objectOfTask;
    public Bear selectedBear;

    [JsonProperty("totalSteps")] public float totalSteps { get; set; } // Текущее
    [JsonProperty("needSteps")] public float needSteps { get; set; } // Колво для завершения задачи

    /// <summary>
    /// Приведение режима задачи в строчный вид, для записи json
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Если в свитчкейсе нет текущего таск мода</exception>
    [JsonProperty("taskMode")]
    public string taskModeString
    {
        get
        {
            return taskMode switch
            {
                TasksMode.build => "build",
                TasksMode.destroy => "destroy",
                TasksMode.getResource => "getResource",
                TasksMode.create => "create",
                _ => throw new ArgumentOutOfRangeException("Такой вид задач не был найден")
            };
        }
    }
    
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
    
    /// <summary>
    /// Вернет режим задания по строке
    /// </summary>
    /// <param name="taskMode">название задания из json</param>
    /// <returns>Вернет режим задания</returns>
    /// <exception cref="ArgumentOutOfRangeException">Если строка не валидна и такого режима нет</exception>
    public static TasksMode GetTaskMode(string taskMode)
    {
        return taskMode switch
        {
            "build" => TasksMode.build,
            "destroy" => TasksMode.destroy,
            "getResource" => TasksMode.getResource,
            "create" => TasksMode.create,
            _ => throw new ArgumentOutOfRangeException(nameof(taskMode), taskMode, null)
        };
    }

    public BearTask(TasksMode _taskMode, GameObject _objectOfTask, float _needSteps)
    {
        taskMode = _taskMode;
        objectOfTask = _objectOfTask;
        needSteps = _needSteps;
    }
}
