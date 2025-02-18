// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Base class for all UISelectable audio components
    /// </summary>
    public abstract class BaseUISelectableAudio : BaseUISelectableAnimator
    {
        /// <summary>
        /// Internal debug mode
        /// </summary>
        private static bool internalDebug => false;

        /// <summary>
        /// Get the current frame number in red color.
        /// This is mostly used for debugging as it helps identify the frame number when a log is printed.
        /// </summary>
        private static string debugCurrentFrameNumber => $"<color=#FF0000>{Time.frameCount}</color>";

        /// <summary>
        /// Frame count when the last state change happened.
        /// This is used to check if the state changed too fast and prevent the sound from playing multiple times in a row.
        /// </summary>
        private int frameCountAtLastStateChange { get; set; } = -1;

        protected override void OnSelectionStateChanged(UISelectionState state)
        {
            if (controller == null) return;

            if (controllerIsToggle)
            {
                switch (ToggleCommand)
                {
                    case CommandToggle.On when !controller.isOn:
                    case CommandToggle.Off when controller.isOn:
                        return;
                }
            }

            // check if the state changed -> if the state did not change, do not play the sound
            var previousState = controller.currentUISelectionState;
            bool stateDidNotChange = previousState == state;
            if (stateDidNotChange) return;

            // we make sure that if the state changed to Pressed, we play the sound regardless of the frame in which the state changed
            // this check is needed to bypass the frame count check as it makes for a better user experience
            bool newStateIsPressed = state == UISelectionState.Pressed;
            if (!newStateIsPressed)
            {
                // check if the state changed too fast -> if the state changed too fast, do not play the sound
                // here we check if the state changed in the current frame
                bool stateChangedTooFast = frameCountAtLastStateChange == Time.frameCount && state != UISelectionState.Pressed;
                if (stateChangedTooFast)
                {
                    if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] State changed too fast [from: {previousState} to: {state}] - do not play the sound");
                    return;
                }
            }
            // update the frame count at last state change
            frameCountAtLastStateChange = Time.frameCount;

            // check if the state is enabled -> if the state is not enabled, do not play the sound
            if (!IsStateEnabled(state)) return;

            // if the application is not playing, do not play the sound
            // this is useful when you are in the editor and you want to test the sounds
            if (!Application.isPlaying) return;


            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            switch (previousState)
            {
                // --- State was Normal ---------------------------------------------
                case UISelectionState.Normal:
                    if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Normal -> {state} - play the sound");
                    break;

                // --- State was Highlighted ----------------------------------------
                case UISelectionState.Highlighted:
                    switch (state)
                    {
                        case UISelectionState.Selected:
                            if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Highlighted -> {state} - do not play the sound");
                            return;
                    }
                    if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Highlighted -> {state} - play the sound");
                    break;

                // --- State was Pressed --------------------------------------------
                case UISelectionState.Pressed:
                    switch (state)
                    {
                        case UISelectionState.Normal:
                        case UISelectionState.Highlighted:
                        case UISelectionState.Selected:
                            if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Pressed -> {state} - do not play the sound");
                            return;
                    }
                    if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Pressed -> {state} - play the sound");
                    break;

                // --- State was Selected -------------------------------------------
                case UISelectionState.Selected:
                    switch (state)
                    {
                        case UISelectionState.Normal:
                        case UISelectionState.Highlighted:
                            if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Selected -> {state} - do not play the sound");
                            return;
                    }
                    if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Selected -> {state} - play the sound");
                    break;

                // --- State was Disabled -------------------------------------------
                case UISelectionState.Disabled:
                    if (internalDebug) Debug.Log($"[{debugCurrentFrameNumber}] Disabled -> {state} - play the sound");
                    break;

                // --- State was Unknown --------------------------------------------
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            StopAllReactions();
            Play(state);
        }

        public override bool IsStateEnabled(UISelectionState state) => true;
        public override void UpdateSettings() {}                           //ignored
        public override void ResetToStartValues(bool forced = false) {}    //ignored
        public override List<Heartbeat> SetHeartbeat<T>() { return null; } //ignored
    }
}
