using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public GameManager gameManager;

    public AudioSource mainMenuBGM;
    public AudioLowPassFilter mainMenuBGMFilter;
    public AudioSource gameBGM;
    public AudioLowPassFilter gameBGMFilter;

    public AudioSource enemyDeathSound;
    public AudioSource clickSound;
    public AudioSource playerDeathSound;
    public AudioSource trophySound;

    public float volumeInterval;

    public GameObject bgmSoundBar;
    public GameObject sfxSoundBar;
    public AudioMixer audioMixer;

    //public float bgmMaxVolume = 0.5f;
    //public float sfxMaxVolume = 0.5f;
    public float bgmVolume;
    public float sfxVolume;

    private List<AudioSource> bgmList;
    private List<AudioSource> sfxList;
    
    void Start()
    {
        bgmList = new List<AudioSource>() 
        {
            mainMenuBGM, gameBGM
        };
        sfxList = new List<AudioSource>()
        {
            enemyDeathSound, clickSound, playerDeathSound, trophySound,
        };
        UpdateBGMVolume(0.5f);
        UpdateSFXVolume(0.5f);
        UpdateSoundBars();
    }

    void Update()
    {
    }
    public void LowerMainMenuFilter()
    {
        mainMenuBGMFilter.cutoffFrequency = 2000;
    }
    public void RaiseMainMenuFilter()
    {
        mainMenuBGMFilter.cutoffFrequency = 7000;
    }
    public void LowerGameFilter()
    {
        gameBGMFilter.cutoffFrequency = 2000;
    }
    public void RaiseGameFilter()
    {
        gameBGMFilter.cutoffFrequency = 7000;
    }
    public void StartGameBGM()
    {
        if (gameBGMFilter.cutoffFrequency == 7000)
        {
            gameBGM.Play();
            mainMenuBGM.Stop();
            audioMixer.SetFloat("gameVolume", -80);
            StartCoroutine(FadeAudioMixer(audioMixer, 3, 1, "gameVolume"));
        }
        RaiseGameFilter();
    }
    public void StartMainMenuBGM()
    {
        mainMenuBGM.Play();
        gameBGM.Stop();
        audioMixer.SetFloat("mainMenuVolume", -80);
        StartCoroutine(FadeAudioMixer(audioMixer, 3, 1, "mainMenuVolume"));
        RaiseMainMenuFilter();
        RaiseGameFilter();
    }
    private IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (audioSource.volume == 0)
        {
            audioSource.Stop();
        }
        yield break;
    }
    private IEnumerator FadeAudioMixer(AudioMixer audioMixer, float duration, float targetVolume, string name)
    {
        float currentTime = 0;
        float start = 0;
        if (targetVolume == 0)
        {
            start = 1;
            targetVolume = 0.0001f;
        }
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioMixer.SetFloat(name, 20f * Mathf.Log10(Mathf.Lerp(start, targetVolume, currentTime / duration)));
            yield return null;
        }
        yield break;
    }
    public void IncreaseBGMVolume()
    {
        UpdateBGMVolume(bgmVolume + volumeInterval);
    }
    public void DecreaseBGMVolume()
    {
        UpdateBGMVolume(bgmVolume - volumeInterval);
    }
    public void IncreaseSFXVolume()
    {
        UpdateSFXVolume(sfxVolume + volumeInterval);
    }
    public void DecreaseSFXVolume() 
    {
        UpdateSFXVolume(sfxVolume - volumeInterval);
    }
    public void UpdateBGMVolume(float volume)
    {
        bgmVolume = volume;
        foreach (AudioSource bgm in bgmList)
        {
            bgm.volume = bgmVolume;
        }
        UpdateSoundBars();
    }
    public void UpdateSFXVolume(float volume) 
    {
        sfxVolume = volume;
        foreach (AudioSource sfx in sfxList)
        {
            sfx.volume = sfxVolume;
        }
        UpdateSoundBars();
    }
    public void UpdateSoundBars()
    {
        for (int i = 0; i < 10; i++)
        {
            bgmSoundBar.transform.GetChild(i).gameObject.SetActive(false);
            sfxSoundBar.transform.GetChild(i).gameObject.SetActive(false);
            if (i < bgmList.Count)
            {
                bgmList[i].volume = (float)decimal.Round((decimal)bgmList[0].volume, 1, System.MidpointRounding.ToEven);
            }
            if (i < sfxList.Count)
            {
                sfxList[i].volume = (float)decimal.Round((decimal)sfxList[0].volume, 1, System.MidpointRounding.ToEven);
            }
        }
        for (int i = 0; i < Mathf.Round(bgmList[0].volume * 10); i++)
        {
            bgmSoundBar.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < Mathf.Round(sfxList[0].volume * 10); i++)
        {
            sfxSoundBar.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void PlayClickSound()
    {
        clickSound.Play();
    }
}
