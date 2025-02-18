// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if DOOZY_SOUNDY

using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Soundy;
using Doozy.Runtime.Soundy.Ids;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Specialized audio component used to play a set of sounds via the Soundy system by listening to a UISelectable (controller)selection state changes.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Components/Addons/UISelectable Soundy Audio")]
    public class UISelectableSoundyAudio : BaseUISelectableAudio
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Doozy/UI/Components/Addons/UISelectable Soundy Audio", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UISelectableSoundyAudio>("UISelectable Soundy Audio", false, true);
        }
        #endif

        private AudioPlayer m_NormalAudioPlayer;
        /// <summary> AudioPlayer used to play a sound when the Selectable is in the Normal state </summary>
        private AudioPlayer normalAudioPlayer => GetAudioPlayer(m_NormalAudioPlayer);

        private AudioPlayer m_HighlightedAudioPlayer;
        /// <summary> AudioPlayer used to play a sound when the Selectable is in the Highlighted state </summary>
        private AudioPlayer highlightedAudioPlayer => GetAudioPlayer(m_HighlightedAudioPlayer);

        private AudioPlayer m_PressedAudioPlayer;
        /// <summary> AudioPlayer used to play a sound when the Selectable is in the Pressed state </summary>
        private AudioPlayer pressedAudioPlayer => GetAudioPlayer(m_PressedAudioPlayer);

        private AudioPlayer m_SelectedAudioPlayer;
        /// <summary> AudioPlayer used to play a sound when the Selectable is in the Selected state </summary>
        private AudioPlayer selectedAudioPlayer => GetAudioPlayer(m_SelectedAudioPlayer);

        private AudioPlayer m_DisabledAudioPlayer;
        /// <summary> AudioPlayer used to play a sound when the Selectable is in the Disabled state </summary>
        private AudioPlayer disabledAudioPlayer => GetAudioPlayer(m_DisabledAudioPlayer);

        [SerializeField] private SoundId NormalSoundId = new SoundId();
        /// <summary> SoundId for the Selectable Normal state </summary>
        public SoundId normalSoundId => NormalSoundId;

        [SerializeField] private SoundId HighlightedSoundId = new SoundId();
        /// <summary> SoundId for the Selectable Highlighted state </summary>
        public SoundId highlightedSoundId => HighlightedSoundId;

        [SerializeField] private SoundId PressedSoundId = new SoundId();
        /// <summary> SoundId for the Selectable Pressed state </summary>
        public SoundId pressedSoundId => PressedSoundId;

        [SerializeField] private SoundId SelectedSoundId = new SoundId();
        /// <summary> SoundId for the Selectable Selected state </summary>
        public SoundId selectedSoundId => SelectedSoundId;

        [SerializeField] private SoundId DisabledSoundId = new SoundId();
        /// <summary> SoundId for the Selectable Disabled state </summary>
        public SoundId disabledSoundId => DisabledSoundId;

        private static AudioPlayer GetAudioPlayer(AudioPlayer player) =>
            player != null ? player : SoundyService.GetSoundPlayer();

        private static void PlaySound(SoundId id, AudioPlayer audioPlayer)
        {
            // do not play sounds while in edit mode (unless the game is playing)
            #if UNITY_EDITOR 
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
            #endif
            
            if (audioPlayer == null) return;
            if (audioPlayer.isPlaying) audioPlayer.Stop();
            if (audioPlayer.LoadSound(id)) audioPlayer.Play();
        }

        private static void StopPlaying(AudioPlayer audioPlayer)
        {
            if (audioPlayer == null || !audioPlayer.isPlaying) return;
            audioPlayer.Stop();
        }

        private static void RecyclePlayer(AudioPlayer player)
        {
            if (player == null) return;
            if (player.isPlaying) player.Stop();
            player.Recycle();
        }
        
        protected override void Awake()
        {
            SoundyService.Initialize();
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RecyclePlayer(m_NormalAudioPlayer);
            RecyclePlayer(m_HighlightedAudioPlayer);
            RecyclePlayer(m_PressedAudioPlayer);
            RecyclePlayer(m_SelectedAudioPlayer);
            RecyclePlayer(m_DisabledAudioPlayer);
        }

        public override void StopAllReactions()
        {
            StopPlaying(m_NormalAudioPlayer);
            StopPlaying(m_HighlightedAudioPlayer);
            StopPlaying(m_PressedAudioPlayer);
            StopPlaying(m_SelectedAudioPlayer);
            StopPlaying(m_DisabledAudioPlayer);
        }

        public override void Play(UISelectionState state)
        {
            switch (state)
            {
                case UISelectionState.Normal:
                    StopPlaying(m_NormalAudioPlayer);
                    PlaySound(normalSoundId, normalAudioPlayer);
                    break;
                case UISelectionState.Highlighted:
                    StopPlaying(m_HighlightedAudioPlayer);
                    PlaySound(highlightedSoundId, highlightedAudioPlayer);
                    break;
                case UISelectionState.Pressed:
                    StopPlaying(m_PressedAudioPlayer);
                    PlaySound(pressedSoundId, pressedAudioPlayer);
                    break;
                case UISelectionState.Selected:
                    StopPlaying(m_SelectedAudioPlayer);
                    PlaySound(selectedSoundId, selectedAudioPlayer);
                    break;
                case UISelectionState.Disabled:
                    StopPlaying(m_DisabledAudioPlayer);
                    PlaySound(disabledSoundId, disabledAudioPlayer);
                    break;
                default:
                    return;
            }

        }
    }
}

#endif
