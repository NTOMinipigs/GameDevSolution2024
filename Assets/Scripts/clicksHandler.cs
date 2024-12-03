using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClicksHandler : MonoBehaviour
{
    public float sensitivity;
    public bool blockMove;
    private bool _isDragging;
    private Vector3 _lastMousePosition, _delta;
    private GameObject _choicedBear;
    private Bear selectedBear;
    [SerializeField] private TextMeshProUGUI textRayTotal;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private allScripts scripts;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Выделение выбранного предмета
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
        {
            selectedBear = null;
            if (hit.collider.gameObject.tag == "bear")
            {
                selectedBear = scripts.colonyManager.GetBear(hit.collider.gameObject.name);
                textRayTotal.text = selectedBear.TraditionStr;
            }
            else if (hit.collider.gameObject.tag == "materialStack" || hit.collider.gameObject.tag == "building")
                textRayTotal.text = hit.collider.gameObject.GetComponent<Building>().buildingName;
        }
        else
            textRayTotal.text = "";

        if (Input.GetMouseButtonDown(0)) // Начало нажатия левой кнопки
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
            {
                if (hit.collider.gameObject.tag == "bear")
                {
                    Bear selectedBear = scripts.colonyManager.GetBear(hit.collider.gameObject.name);
                    scripts.dialogManager.ActivateBearInteractionDialog(selectedBear); // Говорим с медведем
                }
                else if (hit.collider.gameObject.tag == "materialStack" || hit.collider.gameObject.tag == "building")
                    scripts.buildingSystem.SelectBuildingToInteraction(hit.collider.gameObject.GetComponent<Building>());
            }
            _isDragging = true;
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) // Конец отжатия
            _isDragging = false;

        if (_isDragging && !blockMove) // Пока мышка держится, ну и блокировки нету
        {
            _delta = (Input.mousePosition - _lastMousePosition) * sensitivity * Time.deltaTime;

            transform.position += new Vector3(-_delta.x, 0, -_delta.y); // Перемещение камеры, пока зажата левая кнопка мыши
            _lastMousePosition = Input.mousePosition;
        }
    }
}
