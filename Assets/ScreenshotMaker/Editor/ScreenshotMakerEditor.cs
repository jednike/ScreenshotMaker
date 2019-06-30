using UnityEditor;
using UnityEngine;

namespace Screenshot
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotMakerEditor : Editor
    {
        private ScreenshotMaker _target;

        private void OnEnable()
        {
            _target = (ScreenshotMaker) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ScreenshotUtils.NeedWaitForNextStep =
                EditorGUILayout.Toggle("Need wait for click", ScreenshotUtils.NeedWaitForNextStep);

            if (!ScreenshotUtils.ScreenshotTime && GUILayout.Button("Capture Screenshot", GUILayout.MinHeight(20)))
            {
                _target.CaptureScreenshot();
            }

            if (ScreenshotUtils.ScreenshotTime)
            {
                if (ScreenshotUtils.WaitForNextStep && GUILayout.Button("Next Screenshot", GUILayout.MinHeight(20)))
                {
                    ScreenshotUtils.NextStep();
                }

                if (GUILayout.Button("Stop Screenshot", GUILayout.MinHeight(20)))
                {
                    _target.StopScreenshot();
                }
            }
        }
    }
}