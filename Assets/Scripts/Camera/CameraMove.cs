using System;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private bool _isDragging;
    private Vector3 _lastMousePosition, _delta;
    private GameObject _choicedBear;
    public float sensitivity;
    public bool blockMove;
    [SerializeField] private AllScripts scripts;
    [SerializeField] private LayerMask layerMaskInteract;
    public Vector2 minBounds; // Минимальные границы
    public Vector2 maxBounds; // Максимальные границы

    [SerializeField] public float minZoom = 20f;

    [SerializeField] public float maxZoom = 60f;
    private Camera _camera;

    private void Start() => _camera = GetComponent<Camera>();

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0)) // Начало нажатия левой кнопки
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
            {
                if (hit.collider.gameObject.CompareTag("bear"))
                {
                    Bear selectedBear = scripts.colonyManager.GetBear(hit.collider.gameObject.name);
                    if (selectedBear.tradition != Traditions.Drone)
                        scripts.dialogManager.ActivateBearInteractionDialog(selectedBear); // Говорим с медведем
                }
                else if (hit.collider.gameObject.CompareTag("materialStack") || hit.collider.gameObject.CompareTag("building"))
                    scripts.buildingSystem.SelectBuildingToInteraction(hit.collider.gameObject.GetComponent<BuildingController>());
            }

            _isDragging = true;
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) // Конец отжатия
            _isDragging = false;
    }

    public void MoveAndZoom(Vector3 posToMove, float zoomModif)
    {
        transform.position = new Vector3(posToMove.x, transform.position.y, posToMove.z - 35f);
        GetComponent<Camera>().fieldOfView = zoomModif;
    }
    
    private void LateUpdate() // Для плавного перемещения
    {
        _camera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 5;
        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, minZoom, maxZoom);
        
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