//﻿#define FREE_RESIZE

using System;
using System.IO;
using System.Collections;
using UnityEditor;
using UnityEngine;

 namespace Screenshot
{
    public enum Format
    {
        RAW,
        JPG,
        PNG,
        PPM
    };
    public struct ScreenshotInfo
    {
        public string Name;
        public string Platform;
        public string Language;
        public Format Format;
        public string Resolution;
    }
    public class ScreenshotUtils : MonoBehaviour
    {
        public static event Action<string> ChangeLanguage = delegate { };
        public static event Action<int, int> BeforeScreenshot = delegate { };
        public static event Action<int, int> AfterScreenshot = delegate { };
        
        public static bool NeedWaitForNextStep;
        public static bool WaitForNextStep { private set; get; }
        public static bool ScreenshotTime;

        private static ScreenAndLanguages _settings;
        private static float _timeForGamePrepare;
        private static float _timeForLanguageSwitch;
        private static Vector2 _offset;
        private static ScreenshotInfo screenshotInfo;
        private static IEnumerator _coroutine;
        
        private static double _lastTime;
        private static string _testPath;
        private static string _screenshotsPath = Directory.GetParent(Application.dataPath) + "/screenshots/";

        public static string UniqueFilename(ScreenshotInfo info)
        {
            var folder = _screenshotsPath + info.Platform + "/" + info.Language + "/";
            Directory.CreateDirectory(folder);

            return Path.Combine(folder, info.Resolution + "_" + info.Name + "." + info.Format.ToString().ToLower());
        }
        
        public static void NextStep()
        {
            WaitForNextStep = false;
        }
        public static void StopScreenshot()
        {
            ResetAll();
        }

        public static void StartScreenshot(ScreenAndLanguages settings, string screenshotName,
            float timeForLanguageSwitch, float timeForGamePrepare, Format format)
        {
            if (settings.Dimensions.Length == 0 || settings.Dimensions[0].Dimensions.Length == 0)
            {
                return;
            }
            
            _settings = settings;
            _timeForGamePrepare = timeForGamePrepare;
            _timeForLanguageSwitch = timeForLanguageSwitch;

            WaitForNextStep = false;
            ScreenshotTime = true;
            
            screenshotInfo = new ScreenshotInfo
            {
                Format = format,
                Name = screenshotName
            };
            GameViewUtils.SetSize(0);
            
            _lastTime = EditorApplication.timeSinceStartup;
            _coroutine = CaptureScreenshot();
            EditorApplication.update += ExecuteCoroutine;
        }

        private static void ExecuteCoroutine()
        {
            if (_coroutine != null)
            {
                _coroutine.MoveNext();
            }
        }

        private static IEnumerator CaptureScreenshot()
        {
            double delta = 0;
            #if FREE_RESIZE
            #region Detect offset
            
            ChangeSize((int) _settings.Dimensions[0].Dimensions[0].x, (int) _settings.Dimensions[0].Dimensions[0].y);
            var compareSize = _settings.Dimensions[0].Dimensions[0];
            _testPath = Directory.GetParent(Application.dataPath) + "/test" + UnityEngine.Random.Range(1000, 9999) + ".png";
            
            ScreenCapture.CaptureScreenshot(_testPath);
            while (!File.Exists(_testPath))
            {
                yield return null;
            }

            var tmpTexture = new Texture2D(1,1);
            var tmpBytes = File.ReadAllBytes(_testPath);
            tmpTexture.LoadImage(tmpBytes);
            _offset = new Vector2(compareSize.x - tmpTexture.width, compareSize.y - tmpTexture.height);
            File.Delete(_testPath);
            #endregion
            
            GameViewUtils.SetSize(GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, "Free Aspect"));
            #else
            CreateDimensions();
            #endif
            
            foreach (var language in _settings.Languages)
            {
                #region Change language

                ChangeLanguage(language);
                screenshotInfo.Language = language;
                
                delta = EditorApplication.timeSinceStartup - _lastTime;
                while (delta < _timeForLanguageSwitch)
                {
                    yield return null;
                    delta = EditorApplication.timeSinceStartup - _lastTime;
                }
                _lastTime = EditorApplication.timeSinceStartup;

                #endregion
                
                foreach (var pDimension in _settings.Dimensions)
                {
                    screenshotInfo.Platform = pDimension.PlatformName;
                    
                    foreach (var dimension in pDimension.Dimensions)
                    {
                        screenshotInfo.Resolution = (int) dimension.x + "x" + (int) dimension.y;
                        var path = UniqueFilename(screenshotInfo);
                        
                        ChangeSize((int) dimension.x, (int) dimension.y);

                        yield return new WaitForEndOfFrame();

                        Canvas.ForceUpdateCanvases();
                        EditorApplication.Step();
                        GameViewUtils.UpdateZoomAreaAndParent();
                        
                        delta = EditorApplication.timeSinceStartup - _lastTime;
                        File.Delete(path);
                        while (File.Exists(path) || delta < _timeForGamePrepare)
                        {
                            yield return null;
                            delta = EditorApplication.timeSinceStartup - _lastTime;
                        }
                        _lastTime = EditorApplication.timeSinceStartup;

                        #region Game Change after resize
                        BeforeScreenshot((int) dimension.x, (int) dimension.y);
                        
                        delta = EditorApplication.timeSinceStartup - _lastTime;
                        while (delta < _timeForGamePrepare)
                        {
                            yield return null;
                            delta = EditorApplication.timeSinceStartup - _lastTime;
                        }
                        _lastTime = EditorApplication.timeSinceStartup;
                        #endregion

                        Canvas.ForceUpdateCanvases();
                        EditorApplication.Step();
                        
                        yield return new WaitForEndOfFrame();
                        ScreenCapture.CaptureScreenshot(path);
                        yield return new WaitForEndOfFrame();
                        
                        while (!File.Exists(path))
                        {
                            yield return null;
                        }

                        AfterScreenshot((int) dimension.x, (int) dimension.y);
                        
                        #region Wait action if need
                        if (NeedWaitForNextStep) WaitForNextStep = true;
                        while (WaitForNextStep)
                        {
                            yield return null;
                        }
                        #endregion
                    }
                }
            }

            ResetAll();
        }

        private static void ResetAll()
        {
            ChangeSize(1024, 768);
            GameViewUtils.UpdateZoomAreaAndParent();
            ScreenshotTime = false;
            WaitForNextStep = false;
            if(string.IsNullOrEmpty(_testPath) && File.Exists(_testPath))
                File.Delete(_testPath);

            EditorApplication.update -= ExecuteCoroutine;
            _coroutine = null;
        }

#if !FREE_RESIZE
        private static void CreateDimensions()
        {
            foreach (var pDimension in _settings.Dimensions)
            {
                foreach (var dimension in pDimension.Dimensions)
                {
                    if (!GameViewUtils.SizeExists(GameViewSizeGroupType.Standalone, (int) dimension.x, (int) dimension.y))
                    {
                        GameViewUtils.AddCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GameViewSizeGroupType.Standalone, (int) dimension.x, (int) dimension.y, "ScreenShot:" + (int) dimension.x + "x" + (int) dimension.y);
                    }

                }
            }
        }
        private static void ChangeSize(int width, int height)
        {
            GameViewUtils.SetSize(GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, width, height));
        }
#else
        private static void ChangeSize(int width, int height)
        {
            var gameView = GameViewUtils.GetMainGameView();
            var rect = gameView.position;
            rect.width = width + _offset.x;
            rect.height = height + _offset.y;
            rect.x = 0;
            rect.y = 0;
            gameView.position = rect;

#if UNITY_EDITOR_WIN
            gameView.minSize = new Vector2(width + _offset.x, height + _offset.y);
            gameView.maxSize = new Vector2(width + _offset.x, height + _offset.y);
#endif
            gameView.Repaint();
        }
#endif
    }
}
