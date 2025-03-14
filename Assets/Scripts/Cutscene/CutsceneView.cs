
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cutscene
{
    /// <summary>
    /// Cutscene - low level object, он не может сам обращаться к CutsceneManager
    /// CutsceneManager, объект, манипулирующий множеством Cutscene, он не должен выполнять задачу отображения катсцен
    /// 
    /// Отсюда получаем CutsceneView отвечающий за отображение катсцен
    /// </summary>
    public class CutsceneView : MonoBehaviour
    {
        
        /// <summary>
        /// Singleton
        /// </summary>
        public static CutsceneView Singleton { get; private set; }

        
        [SerializeField] private TextMeshProUGUI textHistory;
        [SerializeField] private Image bg;
        [SerializeField] private GameObject fadeObj; // Лол
        [SerializeField] public GameObject cutsceneView;

        /// <summary>
        /// Это должно быть приватным, однако у меня не вышло :sadness:
        /// </summary>
        public bool AnimatingText { get; private set; }
        private bool _canStepNext;


        private string _cutsceneText;
        
        #region setters and getters
        
        /// <summary>
        /// Текст в текущей катсцене
        /// </summary>
        public string CutsceneText
        {
            get => _cutsceneText;
            set
            {
                _cutsceneText = value;
                StartCoroutine(UpdateText(value));
            }
        }

        /// <summary>
        /// Текущий спрайт
        /// </summary>
        public Sprite Sprite
        {
            set
            {
                if (bg.sprite != value)
                {
                    fadeObj.gameObject.SetActive(false);
                    fadeObj.gameObject.SetActive(true);
                    bg.sprite = value;
                }
            }
        }
        
        
        
        # endregion
        
        
        private void Awake()
        {
            Singleton = this;
        }

        
        /// <summary>
        /// Эффект "ввода" текста, буквы появляются не мгновенно, а с эффектом
        /// </summary>
        /// <param name="text">текст</param>
        /// <returns>Все корутины возвращают IEnumerator</returns>
        private IEnumerator UpdateText(string text)
        {
            textHistory.text = "";
            AnimatingText = true;
            char[] textChar = text.ToCharArray();
            foreach (char tChar in textChar)
            {
                if (AnimatingText)
                {
                    textHistory.text += tChar;
                    yield return new WaitForSeconds(0.05f);
                }
            }

            AnimatingText = false;
        }

        /// <summary>
        /// Пропустите эффект ввода текста
        /// </summary>
        public void DropText()
        {
            AnimatingText = false;
            StopAllCoroutines();
            textHistory.text = CutsceneText;
        }
        
        /// <summary>
        /// Закройте катсцену
        /// </summary>
        public void CloseCutscene()
        {
            cutsceneView.SetActive(false);
        }
        
    }
}