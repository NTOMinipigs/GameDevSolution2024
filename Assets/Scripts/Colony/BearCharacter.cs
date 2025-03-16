/// <summary>
/// Черта характера для медведя
/// </summary>
[System.Serializable]
public class BearCharacter
{
    /// <summary>
    /// Визуальное название черты характера
    /// </summary>
    public string gameName;

    /// <summary>
    /// Визуальное описание черты харакетр
    /// </summary>
    public string description;

    /// <summary>
    /// Влияние черты характера на медведя
    /// </summary>
    public CharacterModificators[] characterModificators = new CharacterModificators[0];

    /// <summary>
    /// Временная черта характера
    /// </summary>
    public bool characterToTime;
}

[System.Serializable]
public class CharacterModificators
{
    public enum CharacterChanges
    {
        ModifyWork,
        ModifyHungry,
        ModifyTired,
        ModifySpeed
    }

    public CharacterChanges characterChanges;

    /// <summary>
    /// На сколько изменяется параметр
    /// </summary>
    public float modif;
}