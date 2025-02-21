using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Вверх вниз по UI элементам
/// Должен подгружаться первее чем UI элементы в которых он используется, чтобы подгрузка не случалась позже
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class AdaptiveScroll : MonoBehaviour
{
    public RectTransform content;

    public void UpdateContentSize()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        content.sizeDelta = Vector2.zero;

        foreach (RectTransform child in content)
        {
            content.sizeDelta = new Vector2(
                content.sizeDelta.x,
                content.sizeDelta.y + child.sizeDelta.y + LayoutUtility.GetMargin(child)
            );
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y / 0.6f);
    }

}

public static class LayoutUtility
{
    public static float GetMargin(RectTransform rectTransform)
    {
        var layoutElement = rectTransform.GetComponent<LayoutElement>();
        if (layoutElement != null && layoutElement.ignoreLayout)
            return 0f;

        return rectTransform.rect.height;
    }
}