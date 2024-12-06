using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Этот класс был создан из-за того, что нужно было преобразовывать традиции в обе стороны
/// </summary>
public class TraditionsManager : MonoBehaviour
{
    public enum Traditions { None, Beekeepers, Constructors, Programmers, BioEngineers, Special, Chrom }

    // Количество традиций, нужно для того, чтобы задать размер хешмапе. Изменить при добавлении новой традиции!
    private static byte traditionsAmount = 7;
    
    // Словари в обе стороны. Поиск по словарям О<1>
    private static Dictionary<Traditions, string> traditionsToStr = new Dictionary<Traditions, string>(traditionsAmount);
    private static Dictionary<string, Traditions> strToTraditions = new Dictionary<string, Traditions>(traditionsAmount);

    /// <summary>
    /// Это чтобы создать все традиции в хешмапах. Мне похуй что это не красиво, время 2 ночи
    /// </summary>
    public void Start()
    {
        AddToDictionaries(Traditions.Beekeepers, "Пасечник");
        AddToDictionaries(Traditions.Constructors, "Конструктор");
        AddToDictionaries(Traditions.Programmers, "Программист");
        AddToDictionaries(Traditions.BioEngineers, "Биоинженер");
        AddToDictionaries(Traditions.Chrom, "Первопроходец");
    }
    
    /// <summary>
    /// Добавить традицию и строку в обе хешмапы
    /// </summary>
    /// <param name="tradition">Традиция</param>
    /// <param name="str">названиее традиции</param>
    private static void AddToDictionaries(Traditions tradition, string str)
    {
        traditionsToStr[tradition] = str;
        strToTraditions[str] = tradition;
    }

    // Геттеры
    public static string GetStrByTradition(Traditions tradition) { return traditionsToStr[tradition]; }
    public static Traditions GetTraditionByStr(string tradition) { return strToTraditions[tradition]; }
}
