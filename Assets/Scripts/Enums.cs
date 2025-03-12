using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary>
/// Перечисление активностей медведей   
/// </summary>
[SuppressMessage("ReSharper", "MissingXmlDoc")]
public enum Activities
{
    [Description("Отдыхаю")]
    Chill, 
    [Description("Работаю")]
    Work,
    [Description("Ем")]
    Eat
}

/// <summary>
/// Типы ресурсов в игре
/// </summary>
[SuppressMessage("ReSharper", "MissingXmlDoc")]
public enum Resources
{
    [Description("Материалы")]
    Material,
    [Description("Материалы++")]
    MaterialPlus,
    [Description("Биотопливо")]
    BioFuel,
    [Description("Мёд")]
    Honey,
    [Description("Еда")]
    Food,
    [Description("Энергия")]
    Energy,
    [Description("Население")]
    Bears
}

/// <summary>
/// Професиии медведей
/// </summary>
[SuppressMessage("ReSharper", "MissingXmlDoc")]
public enum Traditions
{
    None,
    [Description("Пасечник")]
    Beekeepers,
    [Description("Инженер")]
    Constructors, 
    [Description("Программист")]
    Programmers,
    [Description("Био инженер")]
    BioEngineers,
    [Description("Дрон")]
    Drone,
    [Description("Хром")]
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

[Serializable]
public class Reward
{
    public float count;
    public Resources typeOfReward;
}

/// <summary>
/// Типы различных ивентов
/// </summary>
public enum TypeOfEvent
{
    NewResource,
    Disaster, 
    ChangeResourceAndDisaster
}

/// <summary>
/// Расширение для Enums, позволяет использовать DescriptionAttribute для задания поведения GetString метода
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Замена ToString метода с расширением
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetString(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString()); // Получаем филд
        DescriptionAttribute attribute = 
            Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute; // Ищем у него аттрибут
        // Если аттрибут Description, используй его методы. Иначе вызываем ToString
        return attribute == null ? value.ToString() : attribute.Description;
    }
}

