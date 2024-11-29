using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class adaptiveScroll : MonoBehaviour
{
    public RectTransform content; 

    public void UpdateContentSize()
    {
        content.sizeDelta = Vector2.zero;

        foreach (RectTransform child in content)
        {
            content.sizeDelta = new Vector2(
                content.sizeDelta.x,
                content.sizeDelta.y + child.sizeDelta.y + LayoutUtility.GetMargin(child)
            );
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x,content.sizeDelta.y / 0.6f);
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