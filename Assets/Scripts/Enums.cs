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
    Bears,
    [Description("Дроны")]
    Drones
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
    [Description("Первопроходец")]
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
    None,
    ChangeBearCharacter,
    Disaster, 
}

public enum TypeOfDisaster
{
    None,
    MinusTemperature,
    PlusTemperature,
    DeadFood,
    DisableEnergy,
    MassInfectinon
}

/// <summary>
/// Расширение для Enums, позволяет использовать DescriptionAttribute для задания поведения GetString метода
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Возвращает строковое представление enum значения из атрибута Description или стандартное имя
    /// </summary>
    /// <param name="value">Значение перечисления</param>
    /// <returns>Строковое представление значения</returns>
    public static string GetString(this Enum value)
    {
        if (value == null) return string.Empty;

        try
        {
            // Получаем информацию о поле
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            // Ищем атрибут Description
            DescriptionAttribute attribute = 
                Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            // Если атрибут Description найден, используем его значение, иначе вызываем ToString
            return attribute?.Description ?? value.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при получении строкового представления для {value}: {ex.Message}");
            return value.ToString();
        }
    }
}

