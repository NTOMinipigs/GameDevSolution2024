using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс, ради отображения в магазине
/// </summary>
public class BuildingBuyInfo : MonoBehaviour
{
    public Building building;
    [HideInInspector] public Button button;
    [HideInInspector] public TextMeshProUGUI textPriceMaterial, textPriceEnergy;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        textPriceMaterial = transform.Find("TextPrice").GetComponent<TextMeshProUGUI>();
        textPriceEnergy = transform.Find("TextPriceEnergy").GetComponent<TextMeshProUGUI>();
    }
}