using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public enum EVolumeType
{
    Master,
    BGM,
    SFX
}

public class SoundManager : ManagerBase
{
    [Serializable]
    public struct Sound
    {
        public string Name;
        public AudioClip Clip;
    }
    
    [Header("Audio Clips")]
    [SerializeField] private Sound[] _bgmClips;
    [SerializeField] private Sound[] _sfxClips;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmPlayer;
    [SerializeField] private GameObject _sfxPlayer;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _mixer;

    private Dictionary<string, AudioClip> _bgmDict = new();
    private Dictionary<string, AudioClip> _sfxDict = new();
    
    private List<AudioSource> _sfxSources = new();
    private Queue<AudioSource> _sfxPool = new();
    
    private readonly Dictionary<EVolumeType, bool> _muted = new()
    {
        { EVolumeType.Master, false },
        { EVolumeType.BGM, false },
        { EVolumeType.SFX, false }
    };

    protected override void Awake()
    {
        base.Awake();
        
        foreach (var clip in _bgmClips) _bgmDict[clip.Name] = clip.Clip;
        foreach (var clip in _sfxClips) _sfxDict[clip.Name] = clip.Clip;

        for (var i = 0; i < 5; i++)
        {
            var src = _sfxPlayer.AddComponent<AudioSource>();
            _sfxSources.Add(src);
            _sfxPool.Enqueue(src);
        }
    }

    #region BGM
    public void PlayBGM(string name)
    {
        if (!_bgmDict.TryGetValue(name, out var clip))
        {
            Debug.LogError($"[SoundManager] BGM '{name}' not found.");
            return;
        }

        _bgmPlayer.clip = clip;
        _bgmPlayer.loop = true;
        FadeInBGM(5);
    }

    public void StopBGM() => _bgmPlayer.Stop();
    
    public void FadeBGM(float targetVolume, float duration)
    {
        _bgmPlayer.DOKill();
        
        _bgmPlayer.DOFade(targetVolume, duration)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                if (!_bgmPlayer.isPlaying && targetVolume > 0f)
                    _bgmPlayer.Play();
            })
            .OnComplete(() =>
            {
                if (Mathf.Approximately(targetVolume, 0f))
                    _bgmPlayer.Stop();
            });
    }

    public void FadeInBGM(float duration) => FadeBGM(0.2f, duration);
    public void FadeOutBGM(float duration) => FadeBGM(0f, duration);
    #endregion

    #region SFX
    public AudioSource PlaySFX(string name)
    {
        if (!_sfxDict.TryGetValue(name, out var clip))
        {
            Debug.LogError($"[SoundManager] SFX '{name}' not found.");
            return null;
        }
        
        var src = _sfxPool.Count > 0
            ? _sfxPool.Dequeue()
            : CreateNewSource();

        src.playOnAwake = false;
        src.clip = clip;
        src.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];
        src.loop = false;
        src.Play();

        if (!src.loop)
        {
            StartCoroutine(ReturnToPoolAfter(src, clip.length));
        }
        
        return src;
    }
    
    private AudioSource CreateNewSource()
    {
        var src = _sfxPlayer.AddComponent<AudioSource>();
        _sfxSources.Add(src);
        return src;
    }
    
    private IEnumerator ReturnToPoolAfter(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (src.clip != null)
        {
            src.Stop();
            src.clip = null;
            _sfxPool.Enqueue(src);
        }
    }

    public void StopSFX(string name)
    {
        if (!_sfxDict.TryGetValue(name, out var clip))
        {
            Debug.LogError($"[SoundManager] SFX '{name}' not found.");
            return;
        }

        foreach (var src in _sfxSources)
        {
            if (src.clip == clip)
            {
                src.Stop();
                src.clip = null;
                _sfxPool.Enqueue(src);
            }
        }
    }
    
    public void StopAllSFX()
    {
        foreach (var src in _sfxSources)
        {
            src.Stop();
            src.clip = null;
        }

        _sfxPool.Clear();
        foreach (var src in _sfxSources)
        {
            _sfxPool.Enqueue(src);
        }
    }
    #endregion

    #region Mute Toggle

    public void ToggleMute(bool mute)
    {
        _bgmPlayer.volume = mute ? 0f : 0.2f;
    }
    public void ToggleMute(EVolumeType type)
    {
        _muted[type] = !_muted[type];
        var db = _muted[type] ? -80f : 0f;
        _mixer.SetFloat(ParamName(type), db);
    }
    
    private string ParamName(EVolumeType t) => t switch
    {
        EVolumeType.Master => "Master_Vol",
        EVolumeType.BGM    => "BGM_Vol",
        EVolumeType.SFX    => "SFX_Vol",
        _ => "Master_Vol",
    };

    public bool IsMuted(EVolumeType type) => _muted[type];
    #endregion
}
