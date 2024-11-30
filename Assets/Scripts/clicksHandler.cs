using UnityEngine;

public class ClicksHandler : MonoBehaviour
{
    public float sensitivity;
    public bool blockMove;
    private bool _isDragging;
    private Vector3 _lastMousePosition, delta;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private allScripts scripts;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
            {
                // TODO: изменить на НЕ ТОЛЬКО медведей + обработчик взаимодейтсвий
                Bear selectedBear = scripts.colonyManager.GetBear(hit.collider.gameObject.name);
                Debug.Log(selectedBear.bearName);
            }
            else
            {
                _isDragging = true;
                _lastMousePosition = Input.mousePosition;                
            }
        }

        if (Input.GetMouseButtonUp(0))
            _isDragging = false;

        if (_isDragging && !blockMove)
        {
            delta = (Input.mousePosition - _lastMousePosition) * sensitivity * Time.deltaTime;

            transform.position += new Vector3(-delta.x, 0, -delta.y);
            _lastMousePosition = Input.mousePosition;
        }
    }
}
