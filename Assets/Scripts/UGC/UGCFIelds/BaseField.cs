using UnityEngine;

namespace UGC.UGCFIelds
{
    /// <summary>
    /// Базовый класс для всех полей
    /// </summary>
    public class BaseField : MonoBehaviour
    {
        
        /// <summary>
        /// Префаб для текущего поля
        /// </summary>
        [SerializeField] private GameObject prefab;
    }
}