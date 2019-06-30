using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TestLanguagesManager : MonoBehaviour
{
    [SerializeField] private Text helloText;

    private void OnEnable()
    {
        Screenshot.ScreenshotUtils.ChangeLanguage += ScreenshotUtilsOnChangeLanguage;
    }

    private void ScreenshotUtilsOnChangeLanguage(string newLanguage)
    {
        switch (newLanguage)
        {
            case "ru":
                helloText.text = "Привет";
                break;
            case "en":
                helloText.text = "Hello";
                break;
        }
    }
    
    private void OnDisable()
    {
        Screenshot.ScreenshotUtils.ChangeLanguage -= ScreenshotUtilsOnChangeLanguage;
    }
}
