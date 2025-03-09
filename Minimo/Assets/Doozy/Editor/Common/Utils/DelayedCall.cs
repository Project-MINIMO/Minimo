// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEditor;

// ReSharper disable DelegateSubtraction
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor.Common.Utils
{
    /// <summary> Executes a delayed call in the Editor </summary>
    public class DelayedCall
    {
        /// <summary> Triggered when the delayed call starts </summary>
        public Action onStart { get; set; }

        /// <summary> Triggered every frame until the delayed call finishes </summary>
        public Action onUpdate { get; set; }

        /// <summary> Triggered when the delayed call finishes </summary>
        public Action onFinish { get; set; }

        /// <summary> Triggered when the delayed call is canceled </summary>
        public Action onCancel { get; set; }
        
        /// <summary> Delay in seconds </summary>
        public float delay { get; private set; }

        /// <summary> Checks if the delayed call is running </summary>
        public bool isRunning { get; private set; }

        /// <summary> EditorApplication.timeSinceStartup when the delayed call started </summary>
        private double startupTime { get; set; }

        /// <summary>
        /// Creates a delayed call. You need to set a delay and a callback (onFinish) before starting it.
        /// eg. DelayedCall dc = new DelayedCall().SetDelay(1f).SetOnFinish(() => Debug.Log("Delayed call finished")).Start();
        /// </summary>
        public DelayedCall()
        {
        }
        
        /// <summary>
        /// Creates a delayed call that will execute after the specified delay.
        /// This delayed call will automatically start.
        /// </summary>
        /// <param name="delay"> Delay in seconds </param>
        /// <param name="callback"> Callback to execute when the delayed call finishes (onFinish) </param>
        public DelayedCall(float delay, Action callback)
        {
            this.delay = delay;
            onFinish = callback;
            Start();
        }

        private void Update()
        {
            onUpdate?.Invoke();
            if (EditorApplication.timeSinceStartup - startupTime < delay) return;
            if (EditorApplication.update != null) EditorApplication.update -= Update;
            onFinish?.Invoke();
        }

        /// <summary> Starts the delayed call </summary>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall Start()
        {
            startupTime = EditorApplication.timeSinceStartup;
            onStart?.Invoke();
            isRunning = true;
            if (EditorApplication.update != null) EditorApplication.update -= Update;
            EditorApplication.update += Update;
            
            return this;
        }

        /// <summary> Stops the delayed call from running </summary>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall Stop()
        {
            isRunning = false;
            if (EditorApplication.update != null) EditorApplication.update -= Update;
            return this;
        }

        /// <summary> Cancels the delayed call and triggers the onCancel callback </summary>
        public void Cancel()
        {
            Stop();
            onCancel?.Invoke();
        }
        
        /// <summary> Sets the delay in seconds </summary>
        /// <param name="value"> Delay in seconds </param>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall SetDelay(float value)
        {
            delay = value;
            return this;
        }

        /// <summary> Adds a callback that will be triggered when the delayed call starts </summary>
        /// <param name="callback"> Callback to add </param>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall OnStart(Action callback)
        {
            onStart += callback;
            return this;
        }

        /// <summary> Adds a callback that will be triggered every frame until the delayed call finishes </summary>
        /// <param name="callback"> Callback to add </param>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall OnUpdate(Action callback)
        {
            onUpdate += callback;
            return this;
        }

        /// <summary> Adds a callback that will be triggered when the delayed call finishes </summary>
        /// <param name="callback"> Callback to add </param>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall OnFinish(Action callback)
        {
            onFinish += callback;
            return this;
        }

        /// <summary> Adds a callback that will be triggered when the delayed call is canceled </summary>
        /// <param name="callback"> Callback to add </param>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall OnCancel(Action callback)
        {
            onCancel += callback;
            return this;
        }

        /// <summary> Removes all callbacks that will be triggered when the delayed call starts </summary>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall ClearOnStart()
        {
            onStart = null;
            return this;
        }

        /// <summary> Removes all callbacks that will be triggered every frame until the delayed call finishes </summary>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall ClearOnUpdate()
        {
            onUpdate = null;
            return this;
        }

        /// <summary> Removes all callbacks that will be triggered when the delayed call finishes </summary>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall ClearOnFinish()
        {
            onFinish = null;
            return this;
        }

        /// <summary> Removes all callbacks that will be triggered when the delayed call is canceled </summary>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall ClearOnCancel()
        {
            onCancel = null;
            return this;
        }

        /// <summary> Removes all callbacks that will be triggered when the delayed call starts, every frame until the delayed call finishes, when the delayed call finishes and when the delayed call is canceled </summary>
        /// <returns> Self (for method chaining) </returns>
        public DelayedCall ClearAllCallbacks()
        {
            ClearOnStart();
            ClearOnUpdate();
            ClearOnFinish();
            ClearOnCancel();
            return this;
        }

        public static DelayedCall Run(float delay, Action callback) =>
            new DelayedCall(delay, callback);
    }
}
