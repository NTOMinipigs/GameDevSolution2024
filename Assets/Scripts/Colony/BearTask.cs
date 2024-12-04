using UnityEngine;

/// <summary>
/// Объект задачи для медведей
/// </summary>
[System.Serializable]
public class BearTask
{
    public enum TasksMode { build, getResource, create }
    public TasksMode taskMode;
    public float totalSteps; // Текущее
    public float needSteps; // Колво для завершения задачи

    public GameObject objectOfTask;
    public bool bearSelected;

    public BearTask(TasksMode _taskMode, GameObject _objectOfTask, float _needSteps)
    {
        taskMode = _taskMode;
        objectOfTask = _objectOfTask;
        needSteps = _needSteps;
    }
}
