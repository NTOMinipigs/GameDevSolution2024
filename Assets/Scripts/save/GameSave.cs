using System.Collections.Generic;
/// <summary>
/// Общая структура хранения в json
/// </summary>

[System.Serializable]
public class GameSave
{
    public List<BearSave> bearSaves;
    public List<BuildingsSave> buildingSaves;
    public List<Dictionary<string, object>> tasksSaves = new();
}
