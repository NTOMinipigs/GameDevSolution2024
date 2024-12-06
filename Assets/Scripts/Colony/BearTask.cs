using UnityEngine;

/// <summary>
/// Объект задачи для медведей
/// </summary>
[System.Serializable]
public class BearTask
{
    public enum TasksMode { None, build, destroy, getResource, create}
    public TasksMode taskMode;
    public float totalSteps; // Текущее
    public float needSteps; // Колво для завершения задачи

    public GameObject objectOfTask;
    public Bear selectedBear;

    public BearTask()
    {

    }

    public BearTask(TasksMode _taskMode, GameObject _objectOfTask, float _needSteps)
    {
        taskMode = _taskMode;
        objectOfTask = _objectOfTask;
        needSteps = _needSteps;
    }
}
