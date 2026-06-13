using UnityEngine;
using UnityEngine.Audio;

public class BGM_Manager : MonoBehaviour
{
    public static BGM_Manager Instance { get; private set; }
    public AudioMixer audioMixer;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("BGM_Manager duplicate → Destroy");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        Debug.Log("BGM_Manager Awake OK, audioMixer = " + audioMixer);

        SetBGMVolume(PlayerPrefs.GetFloat("Music", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFX", 1f));
    }

    public void SetBGMVolume(float value)
    {
        float db = value > 0.001f ? Mathf.Log10(value) * 20f : -80f;
        bool ok = audioMixer.SetFloat("BGMVolume", db);
        Debug.Log($"SetBGMVolume {value} → {db}dB, ok={ok}");
        PlayerPrefs.SetFloat("Music", value);
    }

    public void SetSFXVolume(float value)
    {
        float db = value > 0.001f ? Mathf.Log10(value) * 20f : -80f;
        bool ok = audioMixer.SetFloat("SFXVolume", db);
        Debug.Log($"SetSFXVolume {value} → {db}dB, ok={ok}");
        PlayerPrefs.SetFloat("SFX", value);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlayBGM: clip is NULL!");
            return;
        }
        Debug.Log("PlayBGM: " + clip.name);
        audioSource.Stop();
        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void SetLoop(bool loop) => audioSource.loop = loop;
    public void StopBGM() => audioSource.Stop();
}