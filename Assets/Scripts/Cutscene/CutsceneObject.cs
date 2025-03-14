
using UnityEngine;


namespace Cutscene {

    /// <summary>
    /// Минимальная единица катсцен
    /// </summary>
    public class CutsceneObject : MonoBehaviour
    {
        /// <summary>
        /// Текста для каждого шага
        /// </summary>
        [TextArea] [SerializeField] private string[] stepText = new string[0];

        /// <summary>
        /// Бекграунд для каждого спрайта 
        /// </summary>
        [SerializeField] private Sprite[] stepSprite = new Sprite[0];

        /// <summary>
        /// Текущий шаг, по дефолту изначально -1 для удобной арифметики в NextStep
        /// </summary>
        private int _currentStep = -1;

        /// <summary>
        /// Перелистнуть катсцену
        /// </summary>
        public void NextStep()
        {
            _currentStep++;

            if (stepText.Length - 1 < _currentStep)
            {
                DropCutscene();
                return;
            }
            
            CutsceneView.Singleton.Sprite = stepSprite[_currentStep];
            CutsceneView.Singleton.CutsceneText = stepText[_currentStep];
        }

        /// <summary>
        /// Остановите катсцену
        /// </summary>
        private void DropCutscene()
        {
            CutsceneView.Singleton.CloseCutscene();
            _currentStep = -1;
        }
        
    }
}
