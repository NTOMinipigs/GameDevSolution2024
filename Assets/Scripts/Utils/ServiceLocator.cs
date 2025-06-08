using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Реализация паттерна Service Locator для управления глобальными сервисами
    /// Позволяет избежать жестких зависимостей между компонентами
    /// </summary>
    public class ServiceLocator
    {
        #region Singleton Implementation

        private static ServiceLocator _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Экземпляр ServiceLocator
        /// </summary>
        public static ServiceLocator Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServiceLocator();
                    }

                    return _instance;
                }
            }
        }

        #endregion

        /// <summary>
        /// Словарь зарегистрированных сервисов
        /// </summary>
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private ServiceLocator()
        {
            Debug.Log("ServiceLocator инициализирован");
        }

        /// <summary>
        /// Регистрирует сервис указанного типа
        /// </summary>
        /// <typeparam name="T">Тип сервиса</typeparam>
        /// <param name="service">Экземпляр сервиса</param>
        /// <exception cref="ArgumentNullException">Если сервис равен null</exception>
        /// <exception cref="InvalidOperationException">Если сервис данного типа уже зарегистрирован</exception>
        public void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Нельзя зарегистрировать null в качестве сервиса");
            }

            Type serviceType = typeof(T);

            lock (_lock)
            {
                if (_services.ContainsKey(serviceType))
                {
                    throw new InvalidOperationException($"Сервис типа {serviceType.Name} уже зарегистрирован");
                }

                _services[serviceType] = service;
                Debug.Log($"Зарегистрирован сервис типа {serviceType.Name}");
            }
        }

        /// <summary>
        /// Отменяет регистрацию сервиса указанного типа
        /// </summary>
        /// <typeparam name="T">Тип сервиса</typeparam>
        /// <returns>True, если регистрация сервиса успешно отменена</returns>
        public bool Unregister<T>() where T : class
        {
            Type serviceType = typeof(T);

            lock (_lock)
            {
                bool result = _services.Remove(serviceType);

                if (result)
                {
                    Debug.Log($"Отменена регистрация сервиса типа {serviceType.Name}");
                }

                return result;
            }
        }

        /// <summary>
        /// Получает зарегистрированный сервис указанного типа
        /// </summary>
        /// <typeparam name="T">Тип сервиса</typeparam>
        /// <returns>Экземпляр сервиса или null, если сервис не зарегистрирован</returns>
        public T Get<T>() where T : class
        {
            Type serviceType = typeof(T);

            lock (_lock)
            {
                if (_services.TryGetValue(serviceType, out var service))
                {
                    return (T)service;
                }

                Debug.LogWarning($"Сервис типа {serviceType.Name} не зарегистрирован");
                return null;
            }
        }

        /// <summary>
        /// Проверяет, зарегистрирован ли сервис указанного типа
        /// </summary>
        /// <typeparam name="T">Тип сервиса</typeparam>
        /// <returns>True, если сервис зарегистрирован</returns>
        public bool IsRegistered<T>() where T : class
        {
            Type serviceType = typeof(T);

            lock (_lock)
            {
                return _services.ContainsKey(serviceType);
            }
        }

        /// <summary>
        /// Удаляет все зарегистрированные сервисы
        /// </summary>
        public void ClearAll()
        {
            lock (_lock)
            {
                _services.Clear();
                Debug.Log("Все сервисы удалены из ServiceLocator");
            }
        }
    }
}
