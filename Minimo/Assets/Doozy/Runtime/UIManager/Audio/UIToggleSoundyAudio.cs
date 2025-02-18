// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if DOOZY_SOUNDY

using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.Soundy;
using Doozy.Runtime.Soundy.Ids;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable IdentifierTypo

namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Specialized audio component used to play sounds via the Soundy system by listening to a UIToggle (controller) isOn state changes
    /// </summary>
    [AddComponentMenu("Doozy/UI/Components/Addons/UIToggle Soundy Audio")]
    public class UIToggleSoundyAudio : BaseUIToggleAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Doozy/UI/Components/Addons/UIToggle Soundy Audio", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIToggleSoundyAudio>("UIToggle Soundy Audio", false, true);
        }
        #endif
        
        private AudioPlayer m_AudioPlayer;
        /// <summary> AudioPlayer used to play the sound </summary>
        private AudioPlayer audioPlayer
        {
            get
            {
                if (m_AudioPlayer != null) return m_AudioPlayer;
                m_AudioPlayer = SoundyService.GetSoundPlayer();
                if (m_AudioPlayer == null) return null;
                m_AudioPlayer.onFinish = () => m_AudioPlayer = null;
                return m_AudioPlayer;
            }
        }
        
        [SerializeField] private SoundId OnSoundId = new SoundId();
        /// <summary> Toggle On SoundId </summary>
        public SoundId onSoundId => OnSoundId;
        
        [SerializeField] private SoundId OffSoundId = new SoundId();
        /// <summary> Toggle Off SoundId </summary>
        public SoundId offSoundId => OffSoundId;

        protected override bool onAnimationIsActive => hasController && controller.isOn && onSoundId != null && onSoundId.isValid && audioPlayer != null && audioPlayer.isPlaying;
        protected override bool offAnimationIsActive => hasController && !controller.isOn && offSoundId != null && offSoundId.isValid && audioPlayer != null && audioPlayer.isPlaying;
        protected override UnityAction playOnAnimation => () =>
        {
            // do not play sounds while in edit mode (unless the game is playing)
            #if UNITY_EDITOR 
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
            #endif
            
            // if the audio player is playing a sound, let it play and get a new one to play the on sound
            if (m_AudioPlayer != null && m_AudioPlayer.isPlaying) 
                m_AudioPlayer = null;
           
            if(audioPlayer.LoadSound(onSoundId))
                audioPlayer.Play();
        };
        
        protected override UnityAction playOffAnimation => () =>
        {
            // do not play sounds while in edit mode (unless the game is playing)
            #if UNITY_EDITOR 
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
            #endif
            
            if(offSoundId == null || !offSoundId.isValid) return;
            
            // if the audio player is playing a sound, let it play and get a new one to play the off sound
            if (m_AudioPlayer != null && m_AudioPlayer.isPlaying) 
                m_AudioPlayer = null;
           
            if(audioPlayer.LoadSound(offSoundId))
                audioPlayer.Play();
        };
        
        protected override UnityAction reverseOnAnimation => () => playOffAnimation.Invoke();
        protected override UnityAction reverseOffAnimation => () => playOnAnimation.Invoke();
        /// <summary> Action disabled </summary>
        protected override UnityAction instantPlayOnAnimation => () => {}; //do nothing
        /// <summary> Action disabled </summary>
        protected override UnityAction instantPlayOffAnimation => () => {}; //do nothing
        protected override UnityAction stopOnAnimation => StopSound;
        protected override UnityAction stopOffAnimation => StopSound;
        
        /// <summary> Action disabled </summary>
        protected override UnityAction addResetToOnStateCallback => () => {}; //do nothing
        /// <summary> Action disabled </summary>
        protected override UnityAction removeResetToOnStateCallback => () => {}; //do nothing
        /// <summary> Action disabled </summary>
        protected override UnityAction addResetToOffStateCallback => () => {}; //do nothing
        /// <summary> Action disabled </summary>
        protected override UnityAction removeResetToOffStateCallback => () => {}; //do nothing

        protected override void Awake()
        {
            SoundyService.Initialize();
            base.Awake();
        }
        
        public override void UpdateSettings()
        {
            // if (!hasController) return;
            
            //do nothing
        }
        
        public override void StopAllReactions() =>
            StopSound();

        private void StopSound()
        {
            if (!m_AudioPlayer) return;
            m_AudioPlayer.Stop();
            m_AudioPlayer = null;
        }
        
        public override void ResetToStartValues(bool forced = false) {}
        public override List<Heartbeat> SetHeartbeat<Theartbeat>() => null;
    }
}

#endif