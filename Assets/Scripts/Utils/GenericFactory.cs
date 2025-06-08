using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Интерфейс для фабрики объектов
    /// </summary>
    /// <typeparam name="TKey">Тип ключа для идентификации создаваемых объектов</typeparam>
    /// <typeparam name="TProduct">Тип создаваемых объектов</typeparam>
    public interface IFactory<TKey, TProduct>
    {
        /// <summary>
        /// Создает новый объект указанного типа
        /// </summary>
        /// <param name="key">Идентификатор типа создаваемого объекта</param>
        /// <param name="parameters">Дополнительные параметры для создания объекта</param>
        /// <returns>Созданный объект</returns>
        TProduct Create(TKey key, params object[] parameters);

        /// <summary>
        /// Регистрирует создателя для указанного типа объектов
        /// </summary>
        /// <param name="key">Идентификатор типа создаваемых объектов</param>
        /// <param name="creator">Функция-создатель объектов</param>
        void RegisterCreator(TKey key, Func<object[], TProduct> creator);
    }

    /// <summary>
    /// Базовая реализация фабрики объектов
    /// </summary>
    /// <typeparam name="TKey">Тип ключа для идентификации создаваемых объектов</typeparam>
    /// <typeparam name="TProduct">Тип создаваемых объектов</typeparam>
    public class GenericFactory<TKey, TProduct> : IFactory<TKey, TProduct>
    {
        private readonly Dictionary<TKey, Func<object[], TProduct>> _creators = new Dictionary<TKey, Func<object[], TProduct>>();

        /// <summary>
        /// Создает новый объект указанного типа
        /// </summary>
        /// <param name="key">Идентификатор типа создаваемого объекта</param>
        /// <param name="parameters">Дополнительные параметры для создания объекта</param>
        /// <returns>Созданный объект</returns>
        /// <exception cref="ArgumentException">Если для указанного типа не зарегистрирован создатель</exception>
        public TProduct Create(TKey key, params object[] parameters)
        {
            if (!_creators.TryGetValue(key, out var creator))
            {
                throw new ArgumentException($"Не найден создатель для типа {key}");
            }

            return creator(parameters);
        }

        /// <summary>
        /// Проверяет, зарегистрирован ли создатель для указанного типа
        /// </summary>
        /// <param name="key">Идентификатор типа</param>
        /// <returns>True, если создатель зарегистрирован</returns>
        public bool HasCreator(TKey key)
        {
            return _creators.ContainsKey(key);
        }

        /// <summary>
        /// Регистрирует создателя для указанного типа объектов
        /// </summary>
        /// <param name="key">Идентификатор типа создаваемых объектов</param>
        /// <param name="creator">Функция-создатель объектов</param>
        /// <exception cref="ArgumentNullException">Если создатель равен null</exception>
        public void RegisterCreator(TKey key, Func<object[], TProduct> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            _creators[key] = creator;
        }

        /// <summary>
        /// Удаляет создателя для указанного типа
        /// </summary>
        /// <param name="key">Идентификатор типа</param>
        /// <returns>True, если создатель был успешно удален</returns>
        public bool UnregisterCreator(TKey key)
        {
            return _creators.Remove(key);
        }
    }

    /// <summary>
    /// Фабрика GameObject для создания различных игровых объектов из префабов
    /// </summary>
    public class GameObjectFactory : GenericFactory<string, GameObject>
    {
        private readonly Transform _defaultParent;

        /// <summary>
        /// Создает новую фабрику GameObject
        /// </summary>
        /// <param name="defaultParent">Родительский объект по умолчанию для создаваемых объектов</param>
        public GameObjectFactory(Transform defaultParent = null)
        {
            _defaultParent = defaultParent;
        }

        /// <summary>
        /// Регистрирует префаб для создания игровых объектов
        /// </summary>
        /// <param name="key">Идентификатор префаба</param>
        /// <param name="prefab">Префаб для создания объектов</param>
        public void RegisterPrefab(string key, GameObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            RegisterCreator(key, parameters =>
            {
                Transform parent = _defaultParent;
                Vector3 position = Vector3.zero;
                Quaternion rotation = Quaternion.identity;

                // Разбираем параметры
                if (parameters.Length > 0 && parameters[0] is Transform parentTransform)
                {
                    parent = parentTransform;
                }

                if (parameters.Length > 1 && parameters[1] is Vector3 pos)
                {
                    position = pos;
                }

                if (parameters.Length > 2 && parameters[2] is Quaternion rot)
                {
                    rotation = rot;
                }

                // Создаем объект из префаба
                GameObject instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
                instance.name = prefab.name;

                return instance;
            });
        }
    }
}
