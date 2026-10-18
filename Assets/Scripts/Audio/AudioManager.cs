using System;
using System.Collections.Generic;
using UnityEngine;

// Simple centralized audio manager.
// Define named sounds in the inspector, then play them from anywhere with:
//   AudioManager.Instance.Play("FishCaught");
//   AudioManager.Instance.PlayMusic("Ambient");
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;

        [Tooltip("Randomize pitch slightly each time this sound plays.")]
        public bool randomizePitch;
        [Range(0f, 0.5f)] public float pitchRandomRange = 0.05f;

        [HideInInspector] public AudioSource source;
    }

    [Header("Sound effects (one-shot, overlapping allowed)")]
    [SerializeField] private Sound[] sfxSounds;

    [Header("Music tracks (looping, one at a time)")]
    [SerializeField] private Sound[] musicSounds;

    [Header("Mixer-less volume controls")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;

    private readonly Dictionary<string, Sound> _sfxLookup = new Dictionary<string, Sound>();
    private readonly Dictionary<string, Sound> _musicLookup = new Dictionary<string, Sound>();

    private Sound _currentMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildSources(sfxSounds, _sfxLookup, isMusic: false);
        BuildSources(musicSounds, _musicLookup, isMusic: true);
    }

    private void BuildSources(Sound[] sounds, Dictionary<string, Sound> lookup, bool isMusic)
    {
        if (sounds == null) return;

        foreach (var sound in sounds)
        {
            if (string.IsNullOrEmpty(sound.name))
            {
                Debug.LogWarning("AudioManager: a sound has no name and will be skipped.");
                continue;
            }

            var source = gameObject.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.loop = isMusic ? true : sound.loop;
            source.playOnAwake = false;
            sound.source = source;

            if (!lookup.TryAdd(sound.name, sound))
            {
                Debug.LogWarning($"AudioManager: duplicate sound name '{sound.name}', only the first one will be used.");
            }
        }
    }

    // Plays a one-shot sound effect by name. Multiple calls can overlap.
    public void Play(string soundName)
    {
        if (!_sfxLookup.TryGetValue(soundName, out var sound) || sound.clip == null)
        {
            Debug.LogWarning($"AudioManager: sound '{soundName}' not found or has no clip.");
            return;
        }

        sound.source.pitch = sound.randomizePitch
            ? sound.pitch + UnityEngine.Random.Range(-sound.pitchRandomRange, sound.pitchRandomRange)
            : sound.pitch;

        sound.source.volume = sound.volume * sfxVolume * masterVolume;
        sound.source.PlayOneShot(sound.clip);
    }

    // Plays a music track by name, stopping any currently playing music track first.
    public void PlayMusic(string trackName)
    {
        if (!_musicLookup.TryGetValue(trackName, out var sound) || sound.clip == null)
        {
            Debug.LogWarning($"AudioManager: music track '{trackName}' not found or has no clip.");
            return;
        }

        if (_currentMusic == sound && sound.source.isPlaying) return;

        StopMusic();

        sound.source.pitch = sound.pitch;
        sound.source.volume = sound.volume * musicVolume * masterVolume;
        sound.source.Play();
        _currentMusic = sound;
    }

    public void StopMusic()
    {
        if (_currentMusic != null)
        {
            _currentMusic.source.Stop();
            _currentMusic = null;
        }
    }

    public void StopSound(string soundName)
    {
        if (_sfxLookup.TryGetValue(soundName, out var sound))
        {
            sound.source.Stop();
        }
    }

    public void SetMasterVolume(float value) => masterVolume = Mathf.Clamp01(value);
    public void SetSfxVolume(float value) => sfxVolume = Mathf.Clamp01(value);
    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        if (_currentMusic != null)
        {
            _currentMusic.source.volume = _currentMusic.volume * musicVolume * masterVolume;
        }
    }
}
