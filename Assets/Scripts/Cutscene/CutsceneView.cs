
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
        
        private bool _animatingText, _canStepNext;
        
        #region setters and getters
        
        /// <summary>
        /// Текст в текущей катсцене
        /// </summary>
        public string cutsceneText
        {
            get => cutsceneText;
            set => StartCoroutine(UpdateText(value));
        }

        /// <summary>
        /// Текущий спрайт
        /// </summary>
        public Sprite sprite
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
            _animatingText = true;
            char[] textChar = text.ToCharArray();
            foreach (char tChar in textChar)
            {
                if (_animatingText)
                {
                    textHistory.text += tChar;
                    yield return new WaitForSeconds(0.05f);
                }
            }

            _animatingText = false;
        }

        public void CloseCutscene()
        {
            gameObject.SetActive(false);
        }
        
    }
}