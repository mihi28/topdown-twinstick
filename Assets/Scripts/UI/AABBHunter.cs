using UnityEngine;
using UnityEngine.UI;

public class CanvasHunterV2 : MonoBehaviour
{
    void LateUpdate()
    {
        RectTransform[] allUI = FindObjectsByType<RectTransform>();
        
        foreach (RectTransform rt in allUI)
        {
            // Only care about things actually trying to render graphics
            if (rt.GetComponent<CanvasRenderer>() != null)
            {
                // 1. Catch absolute zero scale
                if (rt.localScale.x == 0 || rt.localScale.y == 0 || rt.localScale.z == 0)
                {
                    Debug.LogError($"<color=orange>CAUGHT IT!</color> Zero Scale detected on: {rt.name} (Parent: {rt.parent?.name})");
                    Debug.Break();
                }

                // 2. Catch absolute zero width or height
                if (rt.rect.width == 0 || rt.rect.height == 0)
                {
                    Debug.LogError($"<color=orange>CAUGHT IT!</color> Zero Width/Height detected on: {rt.name} (Parent: {rt.parent?.name})");
                    Debug.Break();
                }
            }
        }
    }
}