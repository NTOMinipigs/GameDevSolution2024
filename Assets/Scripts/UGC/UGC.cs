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
        
        [JsonProperty("bear_beekepers")] public string bearBeekepers;
        [JsonProperty("bear_bio")] public string bearBio;
        [JsonProperty("bear_construct")] public string bearConstruct;
        [JsonProperty("bear_progers")] public string bearProgers;
        [JsonProperty("bear_chrom")] public string bearChrom;
        [JsonProperty("drones")] public string Drones;


    }
}