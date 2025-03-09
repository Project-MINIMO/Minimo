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

namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Specialized audio component used to play sounds via the Soundy system by listening to a UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Containers/Addons/UIContainer Soundy Audio")]
    public class UIContainerSoundyAudio : BaseUIContainerAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Doozy/UI/Containers/Addons/UIContainer Soundy Audio", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIContainerSoundyAudio>("UIContainer Soundy Audio", false, true);
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

        [SerializeField] private SoundId ShowSoundId = new SoundId();
        /// <summary> Container Show SoundId </summary>
        public SoundId showSoundId => ShowSoundId;

        [SerializeField] private SoundId HideSoundId = new SoundId();
        /// <summary> Container Hide SoundId </summary>
        public SoundId hideSoundId => HideSoundId;

        protected override void Awake()
        {
            SoundyService.Initialize();
            base.Awake();
        }

        private void StopPlayingSounds()
        {
            if (m_AudioPlayer != null && m_AudioPlayer.isPlaying)
            {
                m_AudioPlayer.Stop();
                m_AudioPlayer = null;
            }
        }

        private void PlaySound(SoundId id)
        {
            // do not play sounds while in edit mode (unless the game is playing)
            #if UNITY_EDITOR 
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
            #endif
            
            if (audioPlayer.LoadSound(id))
                audioPlayer.Play();
        }

        /// <summary> Stop the currently playing sound, if any. </summary>
        public override void StopAllReactions()
        {
            StopPlayingSounds();
        }

        public override void Show()
        {
            if (!showSoundId.isValid) return;
            StopPlayingSounds();
            PlaySound(showSoundId);
        }

        public override void ReverseShow() =>
            Hide();

        public override void Hide()
        {
            if (!hideSoundId.isValid) return;
            StopPlayingSounds();
            PlaySound(hideSoundId);
        }

        public override void ReverseHide() =>
            Show();

        /// <summary> Ignored </summary>
        public override void UpdateSettings() {}
        /// <summary> Ignored </summary>
        public override void InstantShow() {}
        /// <summary> Ignored </summary>
        public override void InstantHide() {}
        /// <summary> Ignored </summary>
        public override void ResetToStartValues(bool forced = false) {}
        /// <summary> Ignored </summary>
        public override List<Heartbeat> SetHeartbeat<T>() { return null; }
    }
}

#endif
