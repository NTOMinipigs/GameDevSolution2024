/// <summary>
/// Инфа по каждому медведю для сериализации в другого медведя
/// </summary>
[System.Serializable]
public class BearSave
{
    public string gameName;
    public string bearName;
    public string serializableBear; // Название объекта из иерархии BearSprites 
    public string tradition;
    public string activity;
    public float hungry;
    public float tired;
    public float x;
    public float z;
}
        
