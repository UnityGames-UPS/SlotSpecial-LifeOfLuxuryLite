using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource bg_adudio;
    [SerializeField] internal AudioSource audioPlayer_wl;
    [SerializeField] internal AudioSource audioPlayer_button;
    [SerializeField] internal AudioSource audioSpin_button;
    [SerializeField] private AudioSource bonusBGAudioSource;
    [SerializeField] private AudioSource diamondSoundAudioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioClip diamondAudioClip;

    private bool isForceMuted = false;
    private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();

    private void Start()
    {
        if (bg_adudio) bg_adudio.Play();
        audioPlayer_button.clip = clips[clips.Length - 1];
        audioSpin_button.clip = clips[clips.Length - 2];
        diamondSoundAudioSource.clip = diamondAudioClip;
    }

    private IEnumerable<AudioSource> AllSources()
    {
        yield return bg_adudio;
        yield return audioPlayer_wl;
        yield return audioPlayer_button;
        yield return audioSpin_button;
        yield return bonusBGAudioSource;
        yield return diamondSoundAudioSource;
    }

    // Focus-driven — called from BOTH OnFocusChanged (JS path) and OnApplicationFocus (native path).
    internal void SetMuteAll(bool forceMute)
    {
        if (forceMute == isForceMuted) return;
        isForceMuted = forceMute;

        foreach (var source in AllSources())
        {
            if (source == null) continue;
            if (forceMute)
            {
                preFocusMuteState[source] = source.mute;
                source.mute = true;
            }
            else
            {
                source.mute = preFocusMuteState.TryGetValue(source, out bool prev) ? prev : source.mute;
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        SetMuteAll(!focus);
    }

    internal void PlayDiamondAudio()
    {
        diamondSoundAudioSource.loop = false;
        diamondSoundAudioSource.Play();
    }

    internal void SwitchBGSound(bool isbonus)
    {
        if (isbonus)
        {
            if (bonusBGAudioSource) bonusBGAudioSource.enabled = true;
            if (bg_adudio) bg_adudio.enabled = false;
        }
        else
        {
            if (bonusBGAudioSource) bonusBGAudioSource.enabled = false;
            if (bg_adudio) bg_adudio.enabled = true;
        }
    }

    internal void PlayWLAudio(string type)
    {
        audioPlayer_wl.loop = false;
        int index = 0;
        switch (type)
        {
            case "spin":
                index = 0;
                audioPlayer_wl.loop = true;
                break;
            case "win":
                index = 1;
                break;
            case "megaWin":
                index = 2;
                break;
        }
        StopWLAaudio();
        audioPlayer_wl.clip = clips[index];
        audioPlayer_wl.Play();
    }


    internal void PlayButtonAudio()
    {
        audioPlayer_button.Play();
    }

    internal void PlaySpinButtonAudio()
    {
        audioSpin_button.Play();
    }

    internal void StopWLAaudio()
    {
        audioPlayer_wl.Stop();
        audioPlayer_wl.loop = false;
    }

    internal void StopBgAudio()
    {
        bg_adudio.Stop();
    }

    internal void ToggleMute(bool toggle, string type = "all")
    {
        switch (type)
        {
            case "bg":
                bg_adudio.mute = toggle;
                bonusBGAudioSource.mute = toggle;
                break;
            case "button":
                audioPlayer_button.mute = toggle;
                audioSpin_button.mute = toggle;
                break;
            case "wl":
                audioPlayer_wl.mute = toggle;
                diamondSoundAudioSource.mute = toggle;
                break;
            case "all":
                audioPlayer_wl.mute = toggle;
                bg_adudio.mute = toggle;
                audioPlayer_button.mute = toggle;
                audioSpin_button.mute = toggle;
                break;
        }
    }

}
