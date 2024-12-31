
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Класс хранящий все публичные enums в кодовой базе
/// </summary>
public static class Enums
{
    /// <summary>
    /// Перечисление активностей медведей   
    /// </summary>
    [SuppressMessage("ReSharper", "MissingXmlDoc")]
    public enum Activities
    {
        Chill, 
        Work,
        Eat
    }

    /// <summary>
    /// Типы ресурсов в игре
    /// </summary>
    [SuppressMessage("ReSharper", "MissingXmlDoc")]
    public enum Resources
    {
        Material,
        MaterialPlus,
        BioFuel,
        Honey,
        Food,
        Energy,
        Bears
    }
    
    /// <summary>
    /// Професиии медведей
    /// </summary>
    [SuppressMessage("ReSharper", "MissingXmlDoc")]
    public enum Traditions
    {
        None,
        Beekeepers, 
        Constructors, 
        Programmers,
        BioEngineers, 
        Special, 
        Chrom
    }

    [SuppressMessage("ReSharper", "MissingXmlDoc")]
    public enum TasksMode
    {
        None, 
        Build, 
        Destroy, 
        GetResource,
        Create
    }
}
