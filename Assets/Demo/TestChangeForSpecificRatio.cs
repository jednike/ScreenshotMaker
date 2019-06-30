using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TestChangeForSpecificRatio : MonoBehaviour
{
    [SerializeField] private Image testImage;

    private void OnEnable()
    {
        Screenshot.ScreenshotUtils.BeforeScreenshot += ScreenshotUtilsOnBeforeScreenshot;
    }

    private void ScreenshotUtilsOnBeforeScreenshot(int newWidth, int newHeight)
    {
        var aspectRatio = (float) newWidth / newHeight;
        var rect = testImage.GetComponent<RectTransform>();
        if (aspectRatio < 0.5f)
        {
            rect.sizeDelta = new Vector2(0, 300);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(.5f, 1);
        }
        else
        {
            rect.sizeDelta = new Vector2(300, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, .5f);
        }
    }

    private void OnDisable()
    {
        Screenshot.ScreenshotUtils.BeforeScreenshot -= ScreenshotUtilsOnBeforeScreenshot;
    }
}
