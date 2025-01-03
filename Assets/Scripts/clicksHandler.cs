using UnityEngine;
using TMPro;

public class ClicksHandler : MonoBehaviour
{
    private Bear selectedBear;
    [SerializeField] private TextMeshProUGUI textRayTotal;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private allScripts scripts;

    /// <summary>
    /// Показать текущий статус сущности после клика
    /// </summary>
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Выделение выбранного предмета
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
        {
            selectedBear = null;
            Building building; // double initialize in switch fix
            switch (hit.collider.gameObject.tag)
            {
                case "bear":
                    selectedBear = scripts.colonyManager.GetBear(hit.collider.gameObject.name);
                    textRayTotal.text = selectedBear.TraditionStr;
                    break;
                case "building":
                    building = hit.collider.gameObject.GetComponent<Building>();
                    if (building.builded)
                        textRayTotal.text = building.buildingName;
                    else
                        textRayTotal.text = building.buildingName + "(Строится...)";
                    break;
                case "materialStack":
                    building= hit.collider.gameObject.GetComponent<Building>();
                    if (building.builded)
                        textRayTotal.text = building.buildingName;
                    else
                        textRayTotal.text = building.buildingName + "(Добывается...)";
                    break;
            }
        }
        else
            textRayTotal.text = "";
    }
}
