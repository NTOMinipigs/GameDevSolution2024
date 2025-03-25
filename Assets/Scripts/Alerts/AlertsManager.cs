using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Alerts
{
    /// <summary>
    /// Менеджер алертов
    /// </summary>
    public class AlertsManager : MonoBehaviour
    {
        public static AlertsManager Singleton { get; private set; }

        // Филды хранящие геймобжекты сериализующиеся в юнити
        [SerializeField] private Alert alertPrefab;
        [SerializeField] private Transform alertContainer;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject.transform.parent.gameObject);
            if (Singleton == null)
                Singleton = this;
            else
                Destroy(gameObject.transform.parent.gameObject);
        }

        private void Start()
        {
            ShowAlert("С возвращением!");
        }

        /// <summary>
        /// Покажите алерт
        /// </summary>
        public void ShowAlert(string text)
        {
            // Создаем новый элемент
            Alert alert = Instantiate(alertPrefab, gameObject.transform);
            alert.Text = text;
            alert.transform.SetAsLastSibling(); // Помещаем в конец контейнера

            // Запускаем корутину для плавного появления
            StartCoroutine(SmoothAppear(alert.GetComponent<RectTransform>()));
        }

        /// <summary>
        /// Корутина для плавного появления элемента
        /// </summary>
        /// <param name="item">Новый айтем в VerticalLayout</param>
        /// <returns>coroutines should return null</returns>
        private IEnumerator SmoothAppear(RectTransform item)
        {
            float duration = 0.5f; // Длительность анимации
            float elapsed = 0f;

            // Начальные параметры
            Vector2 startSize = new Vector2(item.sizeDelta.x, 0);
            Vector2 targetSize = item.sizeDelta;

            item.sizeDelta = startSize;

            // Анимация
            while (elapsed < duration)
            {
                item.sizeDelta = Vector2.Lerp(startSize, targetSize, elapsed / duration);
                elapsed += Time.fixedDeltaTime;
                yield return null;
            }

            item.sizeDelta = targetSize; // Фиксируем конечный размер
        }
    }
}