using Newtonsoft.Json;
using UnityEngine;

namespace UGC
{
    [System.Serializable]
    public class UGC
    {
        
        /// <summary>
        /// Sprite of icon
        /// </summary>
        [JsonIgnore] private Sprite _icon;

        /// <summary>
        /// Название мода
        /// </summary>
        [JsonProperty("name")] public string Name;

        /// <summary>
        /// Описание мода
        /// </summary>
        [JsonProperty("description")] public string Description;
    }
}