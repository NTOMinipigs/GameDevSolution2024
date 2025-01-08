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
    [Description("Пасечник")]
    Beekeepers,
    [Description("Инженер")]
    Constructors, 
    [Description("Программист")]
    Programmers,
    [Description("БиоИнженер")]
    BioEngineers, 
    Special, 
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

