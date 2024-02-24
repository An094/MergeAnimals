using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AudioButtonInfo
{
    public string Name;
    public Button _Button;
}


public class AudioSettingMenu : MonoBehaviour
{
    [SerializeField] private List<AudioButtonInfo> ListAudioButton;
    [SerializeField] private Slider _Slider;
    [SerializeField] private GameObject _BGMToggleBtn;
    [SerializeField] private Sprite[] ToggleImages;

    private Image CurrentToggleImage;
    private bool MusicIsOn = true;
    private int ToggleImageIndex = 0;
    private void Awake()
    {
        foreach (AudioButtonInfo info in ListAudioButton)
        {
            info._Button.onClick.RemoveAllListeners();
            info._Button.onClick.AddListener(() => ChangeBGM(info));
        }

        _Slider.onValueChanged.RemoveAllListeners();
        _Slider.onValueChanged.AddListener(OnSliderValueChanged);

        _BGMToggleBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        _BGMToggleBtn.GetComponent<Button>().onClick.AddListener(ToggleMusic);

        CurrentToggleImage = _BGMToggleBtn.GetComponent<Image>();
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        string CurrentBGM = PlayerPrefs.GetString("BGM", "Playwithme");
        float CurrentBGMVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        ToggleImageIndex = PlayerPrefs.GetInt("IsMusicMuted", 1);
        MusicIsOn = ToggleImageIndex != 0;
        AudioButtonInfo btnInfo = ListAudioButton.Find(p => p.Name == CurrentBGM);
        if(btnInfo != null)
        {
            btnInfo._Button.interactable = false;
        }
        _Slider.value = CurrentBGMVolume;
        CurrentToggleImage.sprite = ToggleImages[ToggleImageIndex];
    }

    private void ChangeBGM(AudioButtonInfo InButtonInfo)
    {
        foreach (AudioButtonInfo info in ListAudioButton)
        {
            info._Button.interactable = info.Name != InButtonInfo.Name;
        }
        AudioManager.Instance.PlayMusic(InButtonInfo.Name);

        PlayerPrefs.SetString("BGM", InButtonInfo.Name);
    }

    private void OnSliderValueChanged(float InValue)
    {
        AudioManager.Instance.MusicVolume(InValue);
    }

    private void ToggleMusic()
    {
        MusicIsOn = !MusicIsOn;
        ToggleImageIndex = 1 - ToggleImageIndex;
        CurrentToggleImage.sprite = ToggleImages[ToggleImageIndex];
        AudioManager.Instance.ToggleMusic();
    }
}
