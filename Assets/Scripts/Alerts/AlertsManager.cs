using UnityEngine;

namespace Alerts
{
    
    /// <summary>
    /// Менеджер алертов
    /// </summary>
    public class AlertsManager : MonoBehaviour
    {
        // Филды хранящие геймобжекты сериализующиеся в юнити
        [SerializeField] private Alert alertPrefab;
        [SerializeField] private Transform alertContainer;
        
        void Start()
        {
            ShowAlert("пиг");
            ShowAlert("лол");
        }

        /// <summary>
        /// Показать алерт
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        public void ShowAlert(string text)
        {
            Alert alert = Instantiate(alertPrefab, alertContainer);
            alert.Text = text;
        }
    }
}