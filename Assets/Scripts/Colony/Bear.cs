using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;


/// <summary>
/// Объект медведя, хранит по медведю всю информацию
/// </summary>
[System.Serializable]
public class Bear
{
    [JsonIgnore] public Sprite sprite;
    [JsonConverter(typeof(StringEnumConverter))] [JsonProperty("tradition")] public TraditionsManager.Traditions tradition;
    [JsonConverter(typeof(StringEnumConverter))] [JsonProperty("activity")]  public ActivityManager.Activities activity;

    /// <summary>
    /// gameName - имя медведя которое видит разработчик (gameObject name)
    /// </summary>
    [JsonProperty("gameName")] public string gameName { get; set; }
    
    /// <summary>
    ///  bearName - имя медведя которое видит игрок
    /// </summary>
    [JsonProperty("bearName")] public string bearName { get; set; }
    
    /// <summary>
    /// serializableBear - Название объекта из иерархии BearSprites
    /// </summary>
    [JsonProperty("serializableBear")] public string serializableBear { get; set; }
    
    /// <summary>
    /// Сытость  медведя
    /// </summary>
    [JsonProperty("hungry")] public float hungry { get; set; }
    
    /// <summary>
    /// Настроение медведя
    /// </summary>
    [JsonProperty("tired")] public float tired { get; set; }
    
    /// <summary>
    /// x
    /// </summary>
    [JsonProperty("x")] public float x { get; set; }
    
    /// <summary>
    /// y
    /// </summary>
    [JsonProperty("y")] public float y { get; set; }
    
    /// <summary>
    /// z
    /// </summary>
    [JsonProperty("z")] public float z { get; set; }

    
    /// <summary>
    /// Получить строку традиции. УСТАРЕЛО! Оставил для обратной совместимки, используй TraditionManager
    /// </summary>
     public string TraditionStr => TraditionsManager.GetStrByTradition(tradition);

    /// <summary>
    /// Получить строку активности. УСТАРЕЛО! Оставил для обратной совместимки, используй ActivityManager
    /// </summary>
    public string ActivityStr => ActivityManager.GetStrByActivity(activity);


    public Bear(string _gameName, string _bearName, TraditionsManager.Traditions _tradition, Sprite _sprite)
    {
        gameName = _gameName;
        bearName = _bearName;
        tradition = _tradition;
        sprite = _sprite;
    }
}
