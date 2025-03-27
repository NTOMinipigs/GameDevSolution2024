using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Класс обучения. Очень прототипный, т.к писался на скорую руку
/// </summary>
public class Tutorial : MonoBehaviour
{
    public static Tutorial Singleton { get; private set; }

    [Header("UI")]
    public GameObject tutorialMenu;
    [SerializeField] private TextMeshProUGUI textHeader, textInfo, textTotalPage;
    [SerializeField] private Image screenshotImage;
    [SerializeField] private Button buttonNext, buttonBack;

    [Header("Pages")]
    [SerializeField] private TutorialPage[] tutorialPages = new TutorialPage[0];
    private int selectedPage;

    private void Awake()
    {
        Singleton = this;
    }

    private void UpdatePage()
    {
        TutorialPage page = tutorialPages[selectedPage];
        textHeader.text = page.textHeader;
        textInfo.text = page.textInfo;
        screenshotImage.sprite = page.screenshot;
        textTotalPage.text = (selectedPage + 1).ToString();
        buttonBack.interactable = selectedPage != 0;
        buttonNext.interactable = selectedPage != tutorialPages.Length - 1;
    }

    public void ActivateTutorialMenu()
    {
        tutorialMenu.SetActive(true);
        UpdatePage();
    }

    public void DisableTutorialMenu() => tutorialMenu.SetActive(false);

    public void MovePage(int changePage)
    {
        selectedPage += changePage;
        UpdatePage();
    }
}
