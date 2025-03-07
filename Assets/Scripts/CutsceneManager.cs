using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    private int step;
    [TextArea][SerializeField] private string[] stepText = new string[0];
    [SerializeField] private Sprite[] stepSprite = new Sprite[0];
    [SerializeField] private TextMeshProUGUI textHistory;
    [SerializeField] private Image bg;
    [SerializeField] private GameObject fadeObj; // Лол
    private bool _animatingText, _canStepNext;

    private void Awake()
    {
        if (Config.ConfigManager.Instance.config.debug)
        {
            gameObject.SetActive(false);
            return;
        }
        textHistory.text = stepText[0];
        bg.sprite = stepSprite[0];
        StartCoroutine(SetText(stepText[0]));
    }

    private void DialogMoveNext()
    {
        step++;
        if (stepSprite[step] != stepSprite[step - 1])
        {
            fadeObj.gameObject.SetActive(false);
            fadeObj.gameObject.SetActive(true);
        }

        if ((step + 1) != stepText.Length)
        {
            textHistory.text = stepText[step];
            bg.sprite = stepSprite[step];
            StartCoroutine(SetText(stepText[step]));
        }
        else
        {
            this.gameObject.SetActive(false);
            DialogManager.Singleton.ActivateDialog("afterPrehistory");
        }
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

    private IEnumerator SetText(string text)
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
}
