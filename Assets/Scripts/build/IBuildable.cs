/// <summary>
/// Интерфейс любого строения(ресурсы тоже относятся)
/// </summary>
public interface IBuildable
{
    /// <summary>
    /// Отображаемое имя
    /// </summary>
    public string BuildingName { get; }

    /// <summary>
    /// Отображаемое описание
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Тип ресурса, производимый строением
    /// </summary>
    public Resources TypeResource { get; }

    /// <summary>
    /// Максимальное количество работников
    /// </summary>
    public int MaxWorkers { get; }

    /// <summary>
    /// Сколько дается ресурсов за одного работника
    /// </summary>
    public int ResourceOneWorker { get; }
}