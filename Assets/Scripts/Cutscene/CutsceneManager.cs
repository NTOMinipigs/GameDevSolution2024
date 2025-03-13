using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cutscene
{

    public class CutsceneManager : MonoBehaviour
    {
        public CutsceneManager Singleton { get; private set; }
        
        // Катсцены лежат именно в отдельных филдах, так как к ним хотелось бы иметь доступ по имени
        // Dictionary не сериализуются в юнити, да и катсцен не много, поэтому удобнее использовать филды
        #region cutscenes
        [SerializeField] CutsceneObject FirstCutscene;
        #endregion
        
        private void Awake()
        {
            Singleton = this;
            if (Config.ConfigManager.Instance.config.debug)
            {
                gameObject.SetActive(false);
                return;
            }
            StartCutscene(FirstCutscene);
        }

        /// <summary>
        /// Запуск катсцены
        /// </summary>
        /// <param name="cutscene">катсцена</param>
        public void StartCutscene(CutsceneObject cutscene)
        {
            CutsceneView.Singleton.gameObject.SetActive(true);
            cutscene.NextStep();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_animatingText)
                {
                    _animatingText = false;
                    StopAllCoroutines();
                    textHistory.text = stepText[step];
                }
                else
                    DialogMoveNext();
            }
        }
    }
}