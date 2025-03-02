using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Класс созданный для отображения "уточняющих менюшек", при навождение курсора на другой UI элемент
/// </summary>
public class MouseSubMenuUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject subMenu;
    public void OnPointerEnter(PointerEventData eventData) => subMenu.SetActive(true);

    // Метод вызывается, когда курсор покидает объект
    public void OnPointerExit(PointerEventData eventData) => subMenu.SetActive(false);
}