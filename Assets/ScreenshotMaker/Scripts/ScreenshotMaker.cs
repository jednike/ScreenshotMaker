using UnityEngine;

namespace Screenshot
{
	[ExecuteInEditMode]
	public class ScreenshotMaker : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] private ScreenAndLanguages settings;
		public Format format = Format.JPG;

		[SerializeField] private new Camera camera;

		[SerializeField] private float timeForLanguageSwitch = 0.1f;
		[SerializeField] private float timeForGamePrepare = 0.1f;
		[SerializeField] private KeyCode screenshotKey = KeyCode.K;
		[SerializeField] private string screenshotName = "Default";

		private void Awake()
		{
			if (!camera)
				camera = Camera.main;
		}

		private void Update()
		{
			if (Input.GetKeyUp(screenshotKey))
				CaptureScreenshot();
		}
#endif
		public void CaptureScreenshot()
		{
			ScreenshotUtils.StartScreenshot(settings, screenshotName, timeForLanguageSwitch, timeForGamePrepare, format);
		}

		public void StopScreenshot()
		{
			ScreenshotUtils.StopScreenshot();
		}
	}
}