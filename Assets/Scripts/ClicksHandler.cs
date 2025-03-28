using UnityEngine;
using TMPro;

public class ClicksHandler : MonoBehaviour
{
    private Bear _selectedBear;
    [SerializeField] private TextMeshProUGUI textRayTotal;
    [SerializeField] private LayerMask layerMaskInteract;

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
            _selectedBear = null;
            BuildingController buildingController; // double initialize in switch fix
            switch (hit.collider.gameObject.tag)
            {
                case "bear":
                    _selectedBear = ColonyManager.Singleton.GetBear(hit.collider.gameObject.name);
                    textRayTotal.text = _selectedBear.tradition.GetString();
                    break;
                case "building":
                    buildingController = hit.collider.gameObject.GetComponent<BuildingController>();
                    if (buildingController.isReady)
                        textRayTotal.text = buildingController.Building.BuildingName;
                    else
                        textRayTotal.text = buildingController.Building.BuildingName + "(Строится...)";
                    break;
                case "materialStack":
                    buildingController= hit.collider.gameObject.GetComponent<BuildingController>();
                    if (buildingController.isReady)
                        textRayTotal.text = buildingController.Building.BuildingName;
                    else
                        textRayTotal.text = buildingController.Building.BuildingName + "(Добывается...)";
                    break;
            }
        }
        else
            textRayTotal.text = "";
    }
}
