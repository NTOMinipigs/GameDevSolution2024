using UnityEngine;

public class BearTaskManager
{
    public static BearTaskManager Singleton { get; private set; }

    public void Initialize() => Singleton = this;
    
    /// <summary>
    /// Создать задачу на медведе
    /// </summary>
    public void CreateNewTask(TasksMode newTaskMode, GameObject objectOfTask, Traditions traditionToTask, float steps)
    {
        BearTask task = new BearTask(newTaskMode, objectOfTask, traditionToTask, steps);

        Bear chillBear = ColonyManager.Singleton.GetChillBear(traditionToTask);
        if (chillBear != null)
        {
            task.selectedBear = chillBear;
            chillBear.activity = Activities.Work;
            ColonyManager.Singleton.UpdateWorkersCount();
        }

        ColonyManager.Singleton.bearTasks.Add(task);
    }

    /// <summary>
    /// Выдать освободившемуся медведю новую задачу
    /// </summary>
    private void SetTaskToBear(Bear bear)
    {
        foreach (BearTask task in ColonyManager.Singleton.bearTasks)
        {
            if (task.selectedBear == null && bear.tradition == task.traditionForTask)
            {
                task.selectedBear = bear;
                bear.activity = Activities.Work;
                break;
            }
        }

        // Если работы не нашлось
        if (GetBearTask(bear) == null)
            bear.activity = Activities.Chill;
    }

    public BearTask GetBearTask(Bear bear)
    {
        if (ColonyManager.Singleton.bearTasks.Count > 0 && bear != null)
        {
            foreach (BearTask task in ColonyManager.Singleton.bearTasks)
            {
                if (task.selectedBear.gameName == bear.gameName)
                    return task;
            }
        }

        return null;
    }

    public void EndTask(BearTask task)
    {
        ColonyManager.Singleton.UpdateWorkersCount();
        if (task.taskMode == TasksMode.Build)
        {
            BuildingController buildingController = task.objectOfTask.GetComponent<BuildingController>();
            buildingController.SetNormal();
            buildingController.isBuild = true;
            buildingController.isReady = true;
            BuildingSystem.Singleton.SetBuildSettings(buildingController);
            if (buildingController.Building is Building building) // Настройки для зданий
            {
                ColonyManager.Singleton.Energy -= building.energyNeed;
                ColonyManager.Singleton.scoutHome = building.scoutHome;
            }
        }
        //else if (task.taskMode == TasksMode.GetResource)
        //BuildingSystem.Singleton.PickUpResource(task.objectOfTask);

        Bear selectedBear = task.selectedBear;
        ColonyManager.Singleton.bearTasks.Remove(task);

        if (selectedBear.tired >= 5 || selectedBear.hungry >= 5)
            selectedBear.activity = Activities.Chill;
        else
            SetTaskToBear(selectedBear);
    }

    public void FindAndEndTask(Traditions tradition, GameObject taskObj, bool endAllTask = false)
    {
        foreach (Bear bear in ColonyManager.Singleton.bearsInColony)
        {
            if (bear.tradition == tradition)
            {
                BearTask task = GetBearTask(bear);
                if (task != null && task.objectOfTask == taskObj)
                {
                    EndTask(task);
                    if (!endAllTask)
                        break;
                }
            }
        }
    }
}