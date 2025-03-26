using System;
using UnityEngine;

namespace UGC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UGCClassAttribute : Attribute
    {
        
        /// <summary>
        /// Префаб для класса
        /// </summary>
        private GameObject classPrefab;

        /// <summary>
        /// Список полей в конкретном классе
        /// </summary>
        /// <param name="name"></param>
        private UGCClassAttribute(string name) {}

    }
}