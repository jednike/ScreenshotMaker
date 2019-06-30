using UnityEditor;
using UnityEngine;

namespace Screenshot
{
    public class ScreenshotMakerWindow: EditorWindow
    {
        private ScreenAndLanguages _settings;
		
        private Format _format = Format.JPG;

        private string _path;
        private Rect _rect;
        private RenderTexture _renderTexture;
        private Texture2D _screenShot;
    
        private Camera _camera;

        private float _timeForLanguageSwitch = 0.1f;
        private float _timeForGamePrepare = 0.1f;
        private string _screenshotName = "Default";
    
        [MenuItem("Window/Screenshot")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ScreenshotMakerWindow));
        }

        private void OnGUI()
        {
            _settings = (ScreenAndLanguages) EditorGUILayout.ObjectField("Settings", _settings, typeof(ScreenAndLanguages), false, null);
            _camera = (Camera) EditorGUILayout.ObjectField("Settings", _camera, typeof(Camera), true, null);
            _format = (Format) EditorGUILayout.EnumPopup("Image Format", _format);

            _timeForLanguageSwitch = EditorGUILayout.FloatField("Time for language switch", _timeForLanguageSwitch);
            _timeForGamePrepare = EditorGUILayout.FloatField("Time for game prepare", _timeForGamePrepare);
            _screenshotName = EditorGUILayout.TextField("Screenshot name", _screenshotName);
            ScreenshotUtils.NeedWaitForNextStep = EditorGUILayout.Toggle("Need wait for click", ScreenshotUtils.NeedWaitForNextStep);
            if (!ScreenshotUtils.ScreenshotTime)
            {
                if (GUILayout.Button("Make screenshot"))
                {
                    if (!_settings)
                    {
                        Debug.LogError("Please, set settings");
                        return;
                    }

                    if (!_camera)
                    {
                        _camera = Camera.main;
                        if (!_camera)
                        {
                            Debug.LogError("Please, set camera");
                            return;
                        }
                    }
                    ScreenshotUtils.StartScreenshot(_settings, _screenshotName, _timeForLanguageSwitch, _timeForGamePrepare, _format);
                }
            }
            else
            {
                if (ScreenshotUtils.ScreenshotTime)
                {
                    if (ScreenshotUtils.WaitForNextStep && GUILayout.Button("Next Screenshot", GUILayout.MinHeight(20)))
                    {
                        ScreenshotUtils.NextStep();
                    }
                    if(GUILayout.Button("Stop Screenshot", GUILayout.MinHeight(20)))
                    {
                        ScreenshotUtils.StopScreenshot();
                    }
                }
            }
        }
    }
}