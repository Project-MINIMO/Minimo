// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor
{
    /// <summary>
    /// Calculates the arithmetic mean of all the referenced Progressors progress values.
    /// Useful if you need to combine the progress of multiple Progressors into one.
    /// This calculates the mean value of all the referenced Progressors progress values.
    /// </summary>
    [AddComponentMenu("Doozy/Reactor/Progressor Group")]
    public class ProgressorGroup : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Doozy/Reactor/Progressor Group", false, 6)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<ProgressorGroup>(false, true);
        }
        #endif

        private const float TOLERANCE = 0.001f;

        [SerializeField] protected float Progress;
        [SerializeField] private List<Progressor> Progressors = new List<Progressor>();
        [SerializeField] private List<ProgressTarget> ProgressTargets;
        [SerializeField] protected FloatEvent OnProgressChanged = new FloatEvent();
        [SerializeField] protected FloatEvent OnProgressIncremented = new FloatEvent();
        [SerializeField] protected FloatEvent OnProgressDecremented = new FloatEvent();
        [SerializeField] protected UnityEvent OnProgressReachedOne = new UnityEvent();
        [SerializeField] protected UnityEvent OnProgressReachedZero = new UnityEvent();

        /// <summary> Progressors sources that are used to calculate the mean progress value for this ProgressorGroup </summary>
        public List<Progressor> progressors
        {
            get
            {
                Initialize();
                return Progressors;
            }
        }

        /// <summary> Progress targets that get updated by this ProgressorGroup when activated </summary>
        public List<ProgressTarget> progressTargets
        {
            get
            {
                Initialize();
                return ProgressTargets;
            }
        }

        /// <summary> Current Progress </summary>
        public float progress
        {
            get => Progress;
            private set
            {
                Progress = Mathf.Clamp01(value);
                OnProgressChanged?.Invoke(Progress);
            }
        }

        /// <summary>
        /// Fired when the Progress value changed.
        /// Returns the new progress value.
        /// </summary>
        public FloatEvent onProgressChanged => OnProgressChanged;

        /// <summary>
        /// Fired when the Progress value increased.
        /// Returns the difference between the new and the previous progress value.
        /// <para/> Example: if the previous progress value was 0.5 and the new progress value is 0.7, the returned value will be 0.2
        /// </summary>
        public FloatEvent onProgressIncremented => OnProgressIncremented;

        /// <summary>
        /// Fired when the Progress value decreased.
        /// Returns the difference between the new and the previous progress value.
        /// <para/> Example: if the previous progress value was 0.5 and the new progress value is 0.3, the returned value will be -0.2
        /// </summary>
        public FloatEvent onProgressDecremented => OnProgressDecremented;

        /// <summary>
        /// Fired when the Progress value reached 1.
        /// Returns the new progress value (1).
        /// </summary>
        public UnityEvent onProgressReachedOne => OnProgressReachedOne;

        /// <summary>
        /// Fired when the Progress value reached 0.
        /// Returns the new progress value (0).
        /// </summary>
        public UnityEvent onProgressReachedZero => OnProgressReachedZero;

        private Coroutine updateCoroutine { get; set; }

        /// <summary> Initialization flag </summary>
        public bool initialized { get; set; }

        public void Initialize()
        {
            if (initialized) return;
            ProgressTargets ??= new List<ProgressTarget>();
            Progressors ??= new List<Progressor>();
            initialized = true;
        }

        /// <summary> Update the progress value based on the Progressors sources </summary>
        public void UpdateProgress()
        {
            if (Progressors.Count == 0) return; //no progressors to update from

            float totalProgress = 0;
            int progressorsCount = 0;

            for (int i = 0; i < Progressors.Count; i++)
            {
                if (Progressors[i] == null) continue;
                totalProgress += Progressors[i].progress;
                progressorsCount++;
            }

            float newProgress = totalProgress / progressorsCount; //calculate the mean progress value
            if (newProgress < TOLERANCE) newProgress = 0;         //if the new progress value is less than the tolerance, set it to 0
            if (newProgress > 1 - TOLERANCE) newProgress = 1;     //if the new progress value is greater than 1 minus the tolerance, set it to 1
            if (newProgress.Approximately(progress, TOLERANCE))   //check if the progress value changed 
                return;                                           //if not, exit 
            float previousProgress = progress;                    //store the previous progress value
            progress = newProgress;                               //set the new progress value

            if (previousProgress < progress)
            {
                //progress was incremented
                OnProgressIncremented?.Invoke(progress - previousProgress);
            }
            else if (previousProgress > progress)
            {
                //progress was decremented
                OnProgressDecremented?.Invoke(previousProgress - progress);
            }

            if (progress.Approximately(1, TOLERANCE))
            {
                //if the progress value reached 1
                OnProgressReachedOne?.Invoke();
            }
            else if (progress.Approximately(0, TOLERANCE))
            {
                //if the progress value reached 0
                OnProgressReachedZero?.Invoke();
            }

            ProgressTargets.RemoveNulls();
            ProgressTargets.ForEach(t => t.UpdateTarget(this));
        }

        private void Awake()
        {
            initialized = false;
            progress = -1;
            Initialize();
        }

        private void OnEnable()
        {
            Progressors ??= new List<Progressor>();
            StartUpdate();
        }

        private void OnDisable()
        {
            StopUpdate();
        }

        private void StartUpdate()
        {
            if (updateCoroutine != null) return;
            updateCoroutine = StartCoroutine(UpdateCoroutine());
        }

        private void StopUpdate()
        {
            if (updateCoroutine == null) return;
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }

        private IEnumerator UpdateCoroutine()
        {
            float updateInterval = ReactorSettings.GetRuntimeTickInterval();
            var wait = new WaitForSecondsRealtime(updateInterval);
            while (true)
            {
                yield return wait;
                UpdateProgress();
            }
        }

        /// <summary> Remove null and duplicate targets </summary>
        private void ValidateTargets() =>
            ProgressTargets = progressTargets.Where(t => t != null).Distinct().ToList();


    }
}
