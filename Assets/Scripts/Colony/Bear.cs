using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;


/// <summary>
/// Объект медведя, хранит по медведю всю информацию
/// </summary>
[System.Serializable]
public class Bear
{
    /// <summary>
    /// 2д спрайт медведя в диалоге
    /// </summary>
    [JsonIgnore] public Sprite sprite;
    
    /// <summary>
    /// Объект Tradition из enum. Определяет профессию медвдея
    /// Сереализуемый объект в json
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))] [JsonProperty("tradition")] public Traditions tradition;
    
    /// <summary>
    /// Объект Activity из enum. Определяет активность медведя
    /// Сереализуемый объект в json
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))] [JsonProperty("activity")]  public Activities activity;

    /// <summary>
    /// gameName - имя медведя которое видит разработчик (gameObject name)
    /// </summary>
    [JsonProperty("gameName")] public string gameName;

    /// <summary>
    ///  bearName - имя медведя которое видит игрок
    /// </summary>
    [JsonProperty("bearName")] public string bearName;

    /// <summary>
    /// serializableBear - Название объекта из иерархии BearSprites
    /// </summary>
    [JsonProperty("serializableBear")] public string serializableBear;

    /// <summary>
    /// Сытость  медведя
    /// </summary>
    [JsonProperty("hungry")] public float hungry;

    /// <summary>
    /// Настроение медведя
    /// </summary>
    [JsonProperty("tired")] public float tired;

    /// <summary>
    /// x
    /// </summary>
    [JsonProperty("x")] public float x;

    /// <summary>
    /// y
    /// </summary>
    [JsonProperty("y")] public float y;

    /// <summary>
    /// z
    /// </summary>
    [JsonProperty("z")] public float z;

    
    /// <summary>
    /// Получить строку традиции. УСТАРЕЛО! Оставил для обратной совместимки, используй TraditionManager
    /// </summary>
     public string TraditionStr => tradition.GetString();

    /// <summary>
    /// Получить строку активности. УСТАРЕЛО! Оставил для обратной совместимки, используй ActivityManager
    /// </summary>
    public string ActivityStr => activity.GetString();

    
    /// <summary>
    /// Конструктор медведя
    /// </summary>
    /// <param name="gameName">имя медведя как gameObject</param>
    /// <param name="bearName">Имя медведя которое видит пользователь</param>
    /// <param name="tradition">Профессия медведя</param>
    /// <param name="sprite">Спрайт медведя</param>
    public Bear(string gameName, string bearName, Traditions tradition, Sprite sprite)
    {
        this.gameName = gameName;
        this.bearName = bearName;
        this.tradition = tradition;
        this.sprite = sprite;
    }
}
