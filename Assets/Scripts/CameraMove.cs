using System;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private bool _isDragging;
    private Vector3 _lastMousePosition, _delta;
    private GameObject _choicedBear;
    public float sensitivity;
    public bool blockMove;
    [SerializeField] private allScripts scripts;
    [SerializeField] private LayerMask layerMaskInteract;
    public Vector2 minBounds; // Минимальные границы
    public Vector2 maxBounds; // Максимальные границы
    
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

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
    }

    private void LateUpdate() // Для плавного перемещения
    {
        if (_isDragging && !blockMove) // Пока мышка держится, ну и блокировки нету
        {
            _delta = (Input.mousePosition - _lastMousePosition) * sensitivity * Time.deltaTime;

            Vector3 newPosition = transform.position + new Vector3(-_delta.x, 0, -_delta.y);
            // Ограничиваем
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y);

            transform.position = newPosition;
            _lastMousePosition = Input.mousePosition;
        }
    }
}