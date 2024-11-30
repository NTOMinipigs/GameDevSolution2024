using UnityEngine;

public class ClicksHandler : MonoBehaviour
{
    public float sensitivity;
    public bool blockMove;
    private bool _isDragging;
    private Vector3 _lastMousePosition, _delta;
    private GameObject _choicedBear;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private allScripts scripts;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Выделение медведя
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
        {
            if (hit.collider.gameObject.CompareTag("bear"))
            {
                if (_choicedBear != hit.collider.gameObject)
                {
                    if (_choicedBear != null)
                        _choicedBear.GetComponent<BearMovement>().SetNormal();
                    _choicedBear = hit.collider.gameObject;
                    _choicedBear.GetComponent<BearMovement>().SetChoiced();
                }
            }
            else
            {
                if (_choicedBear != null)
                {
                    _choicedBear.GetComponent<BearMovement>().SetNormal();
                    _choicedBear = null;
                }
            }
        }
        else
        {
            if (_choicedBear != null)
            {
                _choicedBear.GetComponent<BearMovement>().SetNormal();
                _choicedBear = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
            {
                if (hit.collider.gameObject.tag == "bear")
                {
                    Bear selectedBear = scripts.colonyManager.GetBear(hit.collider.gameObject.name);
                    scripts.dialogManager.ActivateBearInteractionDialog(selectedBear);
                }
                else if (hit.collider.gameObject.tag == "materialStack")
                    hit.collider.gameObject.GetComponent<MaterialStack>().ActivateInteraction();
            }
            _isDragging = true;
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
            _isDragging = false;

        if (_isDragging && !blockMove)
        {
            _delta = (Input.mousePosition - _lastMousePosition) * sensitivity * Time.deltaTime;

            transform.position += new Vector3(-_delta.x, 0, -_delta.y);
            _lastMousePosition = Input.mousePosition;
        }
    }
}
