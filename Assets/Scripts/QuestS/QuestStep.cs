
/// <summary>
/// Класс задач в квесте
/// </summary>
[System.Serializable]
public class QuestStep
{
    /// <summary>
    /// Название подквеста
    /// </summary>
    public string stepName;

    /// <summary>
    /// Как только задача началась - активировать диалог?(опц)
    /// </summary>
    public string startStepWithDialog;

    /// <summary>
    /// Если эта строка схожа с другой(при взаимодействии, например с gameName) = задача выполнена
    /// </summary>
    public string endTrigger;
}