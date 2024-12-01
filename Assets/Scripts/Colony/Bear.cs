using UnityEngine;


/// <summary>
/// Объект медведя, хранит по медведю всю информацию
/// </summary>
[System.Serializable]
public class Bear
{
    public string gameName;
    public string bearName;
    public Sprite sprite;
    public enum Traditions { None, Beekeepers, Constructors, Programmers, BioEngineers, Special, Chrom }
    public Traditions tradition;

    /// <summary>
    /// Получить строку традиции
    /// </summary>
    /// <exception cref="ArgumentException">Если активность не найдена. АРТЕМ НЕ НАДО ВЫРЕЗАТЬ ARGUMENTEXCEIPTIONS! Если ты обосрешься, то благодаря ошибке ты увидишь это быстрее</exception>
    [HideInInspector]
    public string TraditionStr
    {
        get
        {
            return tradition switch
            {
                Traditions.Beekeepers => "Пасечник",
                Traditions.Constructors => "Конструктор",
                Traditions.Programmers => "Программист",
                Traditions.BioEngineers => "Биоинженер",
                Traditions.Chrom => "Первопроходец",
                _ => throw new System.ArgumentException("Tradition " + tradition + " not found!")
            };
        }
    }

    public enum Activities { chill, work, eat }
    public Activities activity;

    /// <summary>
    /// Получить строку активности
    /// </summary>
    /// <exception cref="ArgumentException">Если активность не найдена. АРТЕМ НЕ НАДО ВЫРЕЗАТЬ ARGUMENTEXCEIPTIONS! Если ты обосрешься, то благодаря ошибке ты увидишь это быстрее</exception>
    public string ActivityStr
    {
        get
        {
            return activity switch
            {
                Activities.chill => "отдыхаю",
                Activities.work => "работаю",
                Activities.eat => "ем",
                _ => throw new System.ArgumentException("Activity " + activity + " not found!")
            };
        }
    }

    public float lvl = 0f;
    public float hungry, tired;

    public Bear(string _gameName, string _bearName, Traditions _tradition, Sprite _sprite)
    {
        gameName = _gameName;
        bearName = _bearName;
        tradition = _tradition;
        sprite = _sprite;
    }
}
