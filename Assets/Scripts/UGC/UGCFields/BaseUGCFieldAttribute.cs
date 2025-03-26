using System;
using UnityEngine;

namespace UGC.UGCFields
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BaseUGCFieldAttribute : Attribute
    {
        
        /// <summary>
        /// Префаб текущего UGC филда
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">имя поля</param>
        /// <param name="description">название поля</param>
        public BaseUGCFieldAttribute(string name, string description)
        {
            throw new NotImplementedException();
        }
    }
}