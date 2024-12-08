using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Этот класс был создан из-за того, что нужно было преобразовывать традиции в обе стороны
/// </summary>
public class ResourcesManager : MonoBehaviour
{
    public enum Resources { Material, MaterialPlus, BioFuel, Honey, Food, Energy, Bears}

    // Количество традиций, нужно для того, чтобы задать размер хешмапе. Изменить при добавлении новой традиции!
    private static byte traditionsAmount = 7;
    
    // Словари в обе стороны. Поиск по словарям О<1>
    private static Dictionary<Resources, string> resourceToStr = new Dictionary<Resources, string>(traditionsAmount);
    private static Dictionary<string, Resources> strToResource = new Dictionary<string, Resources>(traditionsAmount);

    /// <summary>
    /// Это чтобы создать все традиции в хешмапах. Мне похуй что это не красиво, время 2 ночи
    /// </summary>
    public void Start()
    {
        AddToDictionaries(Resources.Material, "material");
        AddToDictionaries(Resources.MaterialPlus, "plusMaterial");
        AddToDictionaries(Resources.BioFuel, "bioFuel");
        AddToDictionaries(Resources.Honey, "honeyBox");
        AddToDictionaries(Resources.Food, "food");
        AddToDictionaries(Resources.Energy, "energy");
    }
    
    /// <summary>
    /// Добавить традицию и строку в обе хешмапы
    /// </summary>
    /// <param name="tradition">Традиция</param>
    /// <param name="str">названиее традиции</param>
    private static void AddToDictionaries(Resources tradition, string str)
    {
        resourceToStr[tradition] = str;
        strToResource[str] = tradition;
    }

    // Геттеры
    public static string GetStrByResource(Resources tradition) { return resourceToStr[tradition]; }
    public static Resources GetResourceByStr(string tradition) { return strToResource[tradition]; }
}