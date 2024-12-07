using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Этот класс был создан из-за того, что нужно было преобразовывать активности в обе стороны
/// </summary>
public class ActivityManager : MonoBehaviour
{
    public enum Activities { Chill, Work, Eat }

    // Количество традиций, нужно для того, чтобы задать размер хешмапе. Изменить при добавлении новой активности!
    private static byte activityAmount = 7;
    
    // Словари в обе стороны. Поиск по словарям О<1>
    private static Dictionary<Activities, string> activityToStr = new Dictionary<Activities, string>(activityAmount);
    private static Dictionary<string, Activities> strToActivity = new Dictionary<string, Activities>(activityAmount);    
    
    /// <summary>
    /// Это чтобы создать все активности в хешмапах. Мне похуй что это не красиво, время 2 ночи
    /// </summary>
    public void Start()
    {
        AddToDictionaries(Activities.Chill, "Отдыхаю");
        AddToDictionaries(Activities.Work, "Работаю");
        AddToDictionaries(Activities.Eat, "Ем");
    }
    
    /// <summary>
    /// Добавить традицию и строку в обе хешмапы
    /// </summary>
    /// <param name="activity">Активность</param>
    /// <param name="str">названиее традиции</param>
    private static void AddToDictionaries(Activities activity, string str)
    {
        activityToStr[activity] = str;
        strToActivity[str] = activity;
    }
    
    // Геттеры
    public static string GetStrByActivity(Activities tradition) { return activityToStr[tradition]; }
    public static Activities GetActivityByStr(string tradition) { return strToActivity[tradition]; }
    
}
