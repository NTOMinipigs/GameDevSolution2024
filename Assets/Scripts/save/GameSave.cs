using System.Collections.Generic;
/// <summary>
/// Общая структура хранения в json
/// </summary>

[System.Serializable]
public class GameSave
{
    public List<Bear> bears = new();
    public List<BuildingsSave> buildingSaves = new();
    public List<Dictionary<string, object>> tasksSaves = new();
}
