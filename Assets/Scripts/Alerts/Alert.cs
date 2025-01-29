
using System.Collections;
using TMPro;
using UnityEngine;

namespace Alerts
{
    /// <summary>
    /// Объект управляющий Alert сообщением (буквально само алерт сообщение
    /// </summary>
    internal class Alert : MonoBehaviour
    {

        // ------------ public -----------------

        /// <summary>
        /// Длительность жизни сообщения
        /// </summary>
        public float alertLifeDuration = 40.0f;

        /// <summary>
        /// Управляйте текстом сообщения через этот параметр
        /// </summary>
        public string Text
        {
            get => textMeshPro.text;
            set => textMeshPro.text = value;
        }


        // ------------- private --------------------

        // ------------- ui elements ----------------

        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private SpriteRenderer progressBar;
        [SerializeField] private CanvasGroup canvasGroup;


        // ------------- coroutines -----------------

        /// <summary>
        /// Корутина ожидания закрытия алерта. Храним чтобы можно было аварийно убить
        /// </summary>
        private Coroutine _destroyCoroutine;

        /// <summary>
        /// Корутина прогрессбара. Храним чтобы можно было аварийно убить 
        /// </summary>
        private Coroutine _progressBarCoroutine;


        // ------------ destroy --------------
        /// <summary>
        /// Время анимации уничтожения алерта
        /// </summary>
        private const float DestroyTime = 0.5f;

        /// <summary>
        /// Скорость мерцания перед уничтожением
        /// </summary>
        private const float BlinkSpeed = 0.1f; // Скорость мерцания


        void Start()
        {
            _destroyCoroutine = StartCoroutine(WaitAlertDestroy());
            _progressBarCoroutine = StartCoroutine(ProgressBarEffects());
        }

        /// <summary>
        /// Ожидание конца жизни алерта
        /// </summary>
        /// <returns>Корутины возвращают IEnumerator</returns>
        private IEnumerator WaitAlertDestroy()
        {
            yield return new WaitForSeconds(alertLifeDuration - DestroyTime);
            yield return DestroyEffect();
        }

        /// <summary>
        /// Спецэффект-мерцание голограммы перед исчезновением алерта
        /// </summary>
        /// <returns>Корутины возвращают IEnumerator</returns>
        private IEnumerator DestroyEffect()
        {
            float elapsedTime = 0f;

            // Эффект мерцания
            while (elapsedTime < DestroyTime)
            {
                canvasGroup.alpha = Mathf.PingPong(Time.time * (1 / BlinkSpeed), 1);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Уменьшение прогрессбара с течением времени
        /// </summary>
        /// <returns>Корутины возвращают IEnumerator</returns>
        private IEnumerator ProgressBarEffects()
        {
            float elapsedTime = 0f;
            float startWidth = progressBar.size.x;

            while (elapsedTime < alertLifeDuration)
            {
                float newWidth = Mathf.Lerp(startWidth, 0, elapsedTime / alertLifeDuration);
                progressBar.size = new Vector2(newWidth, progressBar.size.y);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Поведение кнопки закрытия
        /// </summary>
        public void Close()
        {
            // Убиваем сам объект
            Destroy(gameObject);

            // Убиваем корутины алерта
            StopCoroutine(_destroyCoroutine);
            StopCoroutine(_progressBarCoroutine);
        }
    }
}
