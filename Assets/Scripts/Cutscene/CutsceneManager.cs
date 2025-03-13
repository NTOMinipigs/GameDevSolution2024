using UnityEngine;

namespace Cutscene
{
    /// <summary>
    /// Объект управляющий всеми Cutscene объектами
    /// </summary>
    public class CutsceneManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static CutsceneManager Singleton { get; private set; }

        /// <summary>
        /// Текущий объект катсцены
        /// </summary>
        private CutsceneObject _currentCutsceneObject;


        // Катсцены лежат именно в отдельных филдах, так как к ним хотелось бы иметь доступ по имени
        // Dictionary не сериализуются в юнити, да и катсцен не много, поэтому удобнее использовать филды

        #region cutscenes

        [SerializeField] public CutsceneObject FirstCutscene;

        #endregion

        private void Awake()
        {
            Singleton = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Дыра в архитектуре, это лучше исправить
                // CutsceneManager не должен напрямую обращаться к CutsceneView
                if (CutsceneView.Singleton.AnimatingText)
                {
                    CutsceneView.Singleton.DropText();
                }
                else
                    NextStep();
            }
        }

        /// <summary>
        /// Запуск катсцены
        /// </summary>
        /// <param name="cutscene">катсцена</param>
        public void StartCutscene(CutsceneObject cutscene)
        {
            CutsceneView.Singleton.cutsceneView.SetActive(true);
            _currentCutsceneObject = cutscene;
            cutscene.NextStep();
        }

        /// <summary>
        /// Перелистнуть катсцену на следующий слайд
        /// </summary>
        public void NextStep()
        {
            _currentCutsceneObject.NextStep();
        }
    }
}