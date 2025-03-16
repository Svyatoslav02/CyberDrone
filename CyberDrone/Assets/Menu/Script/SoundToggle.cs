using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    public Slider volumeSlider;
    private AudioSource menuMusic;

    void Start()
    {
        menuMusic = transform.Find("Music").GetComponent<AudioSource>();

        if (menuMusic != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MenuVolume", menuMusic.volume);
            menuMusic.volume = volumeSlider.value;
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float volume)
    {
        if (menuMusic == null) return;

        menuMusic.volume = volume;
        PlayerPrefs.SetFloat("MenuVolume", volume);
    }
}
