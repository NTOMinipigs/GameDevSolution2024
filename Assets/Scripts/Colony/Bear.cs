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
    public TraditionsManager.Traditions tradition;
    public ActivityManager.Activities activity;
    public BearSave bearSave;
    
    /// <summary>
    /// Получить строку традиции. УСТАРЕЛО! Оставил для обратной совместимки, используй TraditionManager
    /// </summary>
    /// <exception cref="ArgumentException">Если активность не найдена. АРТЕМ НЕ НАДО ВЫРЕЗАТЬ ARGUMENTEXCEIPTIONS! Если ты обосрешься, то благодаря ошибке ты увидишь это быстрее</exception>
    [HideInInspector]
    public string TraditionStr => TraditionsManager.GetStrByTradition(tradition);

    /// <summary>
    /// Получить строку активности. УСТАРЕЛО! Оставил для обратной совместимки, используй ActivityManager
    /// </summary>
    /// <exception cref="ArgumentException">Если активность не найдена. АРТЕМ НЕ НАДО ВЫРЕЗАТЬ ARGUMENTEXCEIPTIONS! Если ты обосрешься, то благодаря ошибке ты увидишь это быстрее</exception>
    public string ActivityStr => ActivityManager.GetStrByActivity(activity);

    public float hungry, tired;

    public Bear(string _gameName, string _bearName, TraditionsManager.Traditions _tradition, Sprite _sprite)
    {
        gameName = _gameName;
        bearName = _bearName;
        tradition = _tradition;
        sprite = _sprite;
    }
}
