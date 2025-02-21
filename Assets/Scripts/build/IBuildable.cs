public interface IBuildable
{
    public string BuildingName { get;}
    public string Description { get; }
    public Resources TypeResource { get;}
    public int MaxWorkers { get; }
    public int ResourceOneWorker { get;}
}
