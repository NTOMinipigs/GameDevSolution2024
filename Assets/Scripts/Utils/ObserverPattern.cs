using System;
using System.Collections.Generic;

namespace Utils
{
    #region Интерфейсы и базовые классы

    /// <summary>
    /// Интерфейс для объектов, которые могут отправлять уведомления
    /// </summary>
    /// <typeparam name="T">Тип данных уведомления</typeparam>
    public interface IObservable<T>
    {
        /// <summary>
        /// Добавляет наблюдателя
        /// </summary>
        /// <param name="observer">Наблюдатель</param>
        void AddObserver(IObserver<T> observer);

        /// <summary>
        /// Удаляет наблюдателя
        /// </summary>
        /// <param name="observer">Наблюдатель</param>
        void RemoveObserver(IObserver<T> observer);

        /// <summary>
        /// Уведомляет всех наблюдателей
        /// </summary>
        /// <param name="data">Данные уведомления</param>
        void NotifyObservers(T data);
    }

    /// <summary>
    /// Интерфейс для объектов, которые могут получать уведомления
    /// </summary>
    /// <typeparam name="T">Тип данных уведомления</typeparam>
    public interface IObserver<T>
    {
        /// <summary>
        /// Обрабатывает полученное уведомление
        /// </summary>
        /// <param name="data">Данные уведомления</param>
        void OnNotify(T data);
    }

    /// <summary>
    /// Базовый класс для объектов, которые могут отправлять уведомления
    /// </summary>
    /// <typeparam name="T">Тип данных уведомления</typeparam>
    public abstract class Observable<T> : IObservable<T>
    {
        /// <summary>
        /// Список наблюдателей
        /// </summary>
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        /// <summary>
        /// Добавляет наблюдателя
        /// </summary>
        /// <param name="observer">Наблюдатель</param>
        public void AddObserver(IObserver<T> observer)
        {
            if (observer == null) return;
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        /// <summary>
        /// Удаляет наблюдателя
        /// </summary>
        /// <param name="observer">Наблюдатель</param>
        public void RemoveObserver(IObserver<T> observer)
        {
            if (observer == null) return;
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        /// <summary>
        /// Уведомляет всех наблюдателей
        /// </summary>
        /// <param name="data">Данные уведомления</param>
        public void NotifyObservers(T data)
        {
            // Создаем копию списка на случай, если наблюдатели будут удалены во время уведомления
            List<IObserver<T>> observersCopy = new List<IObserver<T>>(_observers);

            foreach (var observer in observersCopy)
            {
                if (observer != null)
                {
                    try
                    {
                        observer.OnNotify(data);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Ошибка при уведомлении наблюдателя {observer}: {ex.Message}");
                    }
                }
            }
        }
    }

    #endregion

    #region События игры

    /// <summary>
    /// Базовое событие игры
    /// </summary>
    public abstract class GameEvent
    {
        public DateTime Timestamp { get; }

        protected GameEvent()
        {
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Событие изменения ресурсов
    /// </summary>
    public class ResourceChangedEvent : GameEvent
    {
        public string ResourceName { get; }
        public int OldValue { get; }
        public int NewValue { get; }
        public string Source { get; }

        public ResourceChangedEvent(string resourceName, int oldValue, int newValue, string source = null)
            : base()
        {
            ResourceName = resourceName;
            OldValue = oldValue;
            NewValue = newValue;
            Source = source;
        }

        public int Delta => NewValue - OldValue;
        public bool IsIncrease => Delta > 0;
    }

    /// <summary>
    /// Событие изменения в магазине
    /// </summary>
    public class ShopEvent : GameEvent
    {
        public string ShopName { get; }
        public string ResourceName { get; }
        public int Amount { get; }
        public bool IsPurchase { get; }

        public ShopEvent(string shopName, string resourceName, int amount, bool isPurchase)
            : base()
        {
            ShopName = shopName;
            ResourceName = resourceName;
            Amount = amount;
            IsPurchase = isPurchase;
        }
    }

    #endregion
}
