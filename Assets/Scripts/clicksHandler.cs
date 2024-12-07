using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClicksHandler : MonoBehaviour
{
    // Зум
    [SerializeField] private float minZoom = 20f;  // Минимальное поле зрения (или близость)
    [SerializeField] private float maxZoom = 60f;  // Максимальное поле зрения (или дальность)
    private Camera _camera;
    
    public float sensitivity;
    public bool blockMove;
    private bool _isDragging;
    private Vector3 _lastMousePosition, _delta;
    private GameObject _choicedBear;
    private Bear selectedBear;
    [SerializeField] private TextMeshProUGUI textRayTotal;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private allScripts scripts;


    public void Start()
    {
        _camera = GetComponent<Camera>();   
        SetTimeScale(1f);
    }

    //Управление временем
    public void SetTimeScale(float timeScale) => Time.timeScale = timeScale;

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
            else if (hit.collider.gameObject.tag == "building")
            {
                Building building = hit.collider.gameObject.GetComponent<Building>();
                if (building.builded)
                    textRayTotal.text = building.buildingName;
                else
                    textRayTotal.text = building.buildingName + "(Строится...)";
            }
            else if (hit.collider.gameObject.tag == "materialStack")
            {
                Building building = hit.collider.gameObject.GetComponent<Building>();
                if (building.builded)
                    textRayTotal.text = building.buildingName;
                else
                    textRayTotal.text = building.buildingName + "(Добывается...)";
            }
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
        HandleZoom();
    }

    private void HandleZoom()
    {
        if (_camera.orthographic)
        {
            // Для ортографической камеры изменяем размер
            _camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 5;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            // изменяем поле зрения
            _camera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 5;
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, minZoom, maxZoom);
        }
    }
}
