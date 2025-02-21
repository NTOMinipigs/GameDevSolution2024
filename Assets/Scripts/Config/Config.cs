using UnityEngine;

namespace Config
{
    /// <summary>
    /// ScriptableObject в котором хрнанятся все значения из конфига
    /// </summary>
    public class Config : ScriptableObject
    {
        /// <summary>
        /// Debug флаг, если true, пропускаются комиксы, выдается начальный сет ресурсов, и прочее
        /// Для прода флаг должен быть выключен
        /// </summary>
        public bool debug;

        /// <summary>
        /// API токен игры, оставьте его в секрете
        /// </summary>
        public string api_key;
    }
}