using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    private bool isMuted = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        muteToggle.onValueChanged.AddListener(SetMute);

        masterSlider.value = PlayerPrefs.GetFloat("Master", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGM", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXV", 1f);

        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        muteToggle.isOn = isMuted;
        SetMute(isMuted);
    }

    // ÀüÃ¼ º¼·ý Á¶Àý
    public void SetMasterVolume(float volume)
    {
        if (!isMuted)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("Master", volume);
        }
    }

    // ¹è°æ À½¾Ç º¼·ý Á¶Àý
    public void SetBGMVolume(float volume)
    {
        if (!isMuted)
        {
            audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("BGM", volume);
        }
    }

    // È¿°úÀ½ º¼·ý Á¶Àý
    public void SetSFXVolume(float volume)
    {
        if (!isMuted)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFX", volume);
        }
    }

    // ¹ÂÆ® ±â´É
    public void SetMute(bool mute)
    {
        isMuted = mute;

        masterSlider.interactable = !mute;
        bgmSlider.interactable = !mute;
        sfxSlider.interactable = !mute;

        if (mute)
        {
            audioMixer.SetFloat("Master", -100f);
            PlayerPrefs.SetInt("Muted", 1);
        }
        else
        {
            float masterVolume = PlayerPrefs.GetFloat("Master", 1f);
            audioMixer.SetFloat("Master", Mathf.Log10(masterVolume) * 20);
            PlayerPrefs.SetInt("Muted", 0);
        }
    }
}
