// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable RedundantUsingDirective
using System;
// ReSharper restore RedundantUsingDirective
using System.Collections;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Runtime.UIManager.Orientation
{
    /// <summary>
    /// Detects the current screen orientation of the target device.
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(Canvas))]
    [DisallowMultipleComponent]
    public class OrientationDetector : SingletonBehaviour<OrientationDetector>
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Doozy/Orientation/Orientation Detector", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<OrientationDetector>("Orientation Detector", true, true);
        }
        #endif
        
        // ReSharper disable MemberCanBePrivate.Global
        private static string streamCategory => "Orientation";
        private static string streamName => nameof(OrientationDetector);
        // ReSharper restore MemberCanBePrivate.Global

        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> Signal stream for the OrientationDetector </summary>
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(streamCategory, streamName);

        private RectTransform m_RectTransform;
        /// <summary> Reference to the RectTransform component </summary>
        public RectTransform rectTransform => m_RectTransform ? m_RectTransform : m_RectTransform = GetComponent<RectTransform>();

        private Canvas m_Canvas;
        /// <summary> Reference to the Canvas component </summary>
        public Canvas canvas => m_Canvas ? m_Canvas : m_Canvas = GetComponent<Canvas>();

        /// <summary> Callback triggered when the device orientation changed </summary>
        public DetectedOrientationEvent OnOrientationChanged = new DetectedOrientationEvent();
        
        /// <summary> Callback triggered when the device orientation changed </summary>
        public ModyEvent OnAnyOrientation = new ModyEvent();
        
        /// <summary> Callback triggered when the device orientation changed to Portrait </summary>
        public ModyEvent OnPortraitOrientation = new ModyEvent();
        
        /// <summary> Callback triggered when the device orientation changed to Landscape </summary>
        public ModyEvent OnLandscapeOrientation = new ModyEvent();
        
        [SerializeField] private DetectedOrientation CurrentOrientation = DetectedOrientation.Unknown;
        /// <summary> Current detected device orientation </summary>
        public DetectedOrientation currentOrientation
        {
            get => CurrentOrientation;
            private set => CurrentOrientation = value;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        /// <summary> Previous logical screen orientation (previous Screen.orientation value) </summary>
        public ScreenOrientation previousScreenOrientation { get; private set; }

        /// <summary> Internal variable used to count evey orientation check. This is needed to cancel two notifications passes happening OnRectTransformDimensionsChange </summary>
        private int orientationCheckCount { get; set; }

        /// <summary> Coroutine that checks the device orientation every checkInterval seconds </summary>
        private Coroutine orientationCheckCoroutine { get; set; }
        /// <summary> Interval in seconds between each orientation check </summary>
        private float checkInterval { get; set; } = 0.1f;

        private bool firstOrientationCheck { get; set; } = true;
        
        /// <summary>
        /// Set the proper settings for the Canvas and RectTransform components
        /// </summary>
        public void Initialize()
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (canvas.isRootCanvas) return;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }

        private void Reset()
        {
            Initialize();
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            Initialize();
        }
        #endif

        protected override void Awake()
        {
            firstOrientationCheck = true;
            base.Awake();
            currentOrientation = DetectedOrientation.Unknown;
            Initialize();
        }

        private IEnumerator Start()
        {
            // CheckDeviceOrientation();
            yield return null;
            // CheckDeviceOrientation();
            yield return new WaitForEndOfFrame();
            CheckDeviceOrientation();
        }

        private void OnEnable()
        {
            StartCheckOrientation();
        }

        private void OnDisable()
        {
            StopCheckOrientation();
        }

        private void StartCheckOrientation()
        {
            if (orientationCheckCoroutine != null) return;
            orientationCheckCoroutine = StartCoroutine(CheckOrientation());
        }

        private void StopCheckOrientation()
        {
            if (orientationCheckCoroutine == null) return;
            StopCoroutine(orientationCheckCoroutine);
            orientationCheckCoroutine = null;
        }

        private IEnumerator CheckOrientation()
        {
            var wait = new WaitForSecondsRealtime(checkInterval);
            yield return new WaitForEndOfFrame();
            while (true)
            {
                yield return wait;
                CheckDeviceOrientation();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void OnRectTransformDimensionsChange()
        {
            orientationCheckCount++;
            if (orientationCheckCount < 2) return;
            orientationCheckCount = 0;
            CheckDeviceOrientation();
        }

        /// <summary>
        /// Check the current device orientation and send a signal with the orientation value if it has changed.
        /// <para/> This method is called automatically by the OrientationDetector.
        /// </summary>
        public void CheckDeviceOrientation() =>
            CheckDeviceOrientation(false);

        /// <summary>
        /// Check the current device orientation and send a signal with the orientation value if it has changed.
        /// </summary>
        /// <param name="forceUpdate"> Force the update of the orientation value, even if it hasn't changed (to trigger callbacks) </param>
        public void CheckDeviceOrientation(bool forceUpdate)
        {
            #if UNITY_EDITOR
            {
                //Portrait
                if (Screen.width < Screen.height)
                {
                    if (currentOrientation == DetectedOrientation.Portrait && !forceUpdate) return;
                    UpdateOrientation(DetectedOrientation.Portrait);
                    return;
                }

                //Landscape
                if (currentOrientation == DetectedOrientation.Landscape && !forceUpdate) return;
                UpdateOrientation(DetectedOrientation.Landscape);

                
            }
            #else //!UNITY_EDITOR
            {
                if (!firstOrientationCheck && previousScreenOrientation == Screen.orientation && !forceUpdate) return;

                firstOrientationCheck = false;

                switch (Screen.orientation)
                {
                    case ScreenOrientation.Portrait:
                        if (currentOrientation == DetectedOrientation.Portrait && !forceUpdate)
                            return; //if the current orientation is portrait, then we don't need to update anything
                        UpdateOrientation(DetectedOrientation.Portrait);
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                        if (currentOrientation == DetectedOrientation.Portrait && !forceUpdate)
                            return; //if the current orientation is portrait, then we don't need to update anything
                        UpdateOrientation(DetectedOrientation.Portrait);
                        break;
                    case ScreenOrientation.LandscapeLeft:
                        if (currentOrientation == DetectedOrientation.Landscape && !forceUpdate)
                            return; //if the current orientation is landscape, then we don't need to update anything
                        UpdateOrientation(DetectedOrientation.Landscape);
                        break;
                    case ScreenOrientation.LandscapeRight:
                        if (currentOrientation == DetectedOrientation.Landscape && !forceUpdate)
                            return; //if the current orientation is landscape, then we don't need to update anything
                        UpdateOrientation(DetectedOrientation.Landscape);
                        break;
                    case ScreenOrientation.AutoRotation:
                        //fallback to the default orientation -> Landscape
                        if (currentOrientation == DetectedOrientation.Landscape && !forceUpdate)
                            return; //if the current orientation is landscape, then we don't need to update anything
                        UpdateOrientation(DetectedOrientation.Landscape);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endif //UNITY_EDITOR
        }


        private void UpdateOrientation(DetectedOrientation orientation)
        {
            // Debug.Log($"[{Time.frameCount}] Orientation changed to {orientation}");
            stream.SendSignal(orientation);
            OnOrientationChanged.Invoke(orientation);
            OnAnyOrientation?.Execute();
            switch (orientation)
            {
                case DetectedOrientation.Portrait:
                    OnPortraitOrientation?.Execute();
                    break;
                case DetectedOrientation.Landscape:
                    OnLandscapeOrientation?.Execute();
                    break;
                case DetectedOrientation.Unknown:
                    //do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
            currentOrientation = orientation;
            #if !UNITY_EDITOR
            previousScreenOrientation = Screen.orientation;
            #endif
        }
    }
}
