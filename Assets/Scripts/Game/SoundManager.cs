using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        masterSlider.value = PlayerPrefs.GetFloat("Master", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGM", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
    }

    // 전체 볼륨 조절
    public void SetMasterVolume(float volume)
    {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("Master", volume);
    }

    // 배경 음악 볼륨 조절
    public void SetBGMVolume(float volume)
    {
            audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("BGM", volume);
    }

    // 효과음 볼륨 조절
    public void SetSFXVolume(float volume)
    {
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFX", volume);
    }
}
