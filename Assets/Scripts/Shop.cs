using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс отвечающий за открытие магазина
/// </summary>
public class Shop : MonoBehaviour
{

    public GameObject offerPrefab, contentGameObject; // Префаб предложения
    public Sprite[] resourcesSprites = new Sprite[0];
    
    private Resources[] colonyResource = new Resources[5]
    {
        Resources.Material,
        Resources.MaterialPlus,
        Resources.BioFuel,
        Resources.Food,
        Resources.Energy
    };

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {

        }
    }

    /// <summary>
    /// Событие клика
    /// </summary>
    /// <param name="buttonIndex"></param>
    public async void OnButtonClick(int buttonIndex)
    {
        // Чистим менюшку если там уже что-то есть
        foreach (Transform child in contentGameObject.transform)
        {
            Destroy(child.gameObject);
        }
        
        Resources resource = colonyResource[buttonIndex];
        
        string resourceStringName = resource.ToString();
        List<APIClient.UserInventory> users = await APIClient.Instance.GetUsersListRequest();
        foreach (APIClient.UserInventory user in users)
        {
            APIClient.ShopInfo shop = (await APIClient.Instance.GetShopsRequest(user.Name))[0]; // При создании игрока у него автоматически создается магазин, поэтому всегда берем первый элемент

            if (shop.Resources == null) continue;
            
            int countResources = shop.Resources[resourceStringName];
            GameObject gameObject = Instantiate(offerPrefab, contentGameObject.transform);
            
            Image resourceImage = gameObject.GetComponentInChildren<Image>();
            resourceImage.sprite = resourcesSprites[buttonIndex];
            
            TextMeshProUGUI sellerNickname = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            sellerNickname.text = user.Name + countResources;
        }

    }

/// <summary>
    /// Показывает меню магазина
    /// </summary>
    private void OpenShop()
    {
        
    }

}
