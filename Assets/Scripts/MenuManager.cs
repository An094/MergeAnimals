using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject MusicSettingPopup;

    private void Start()
    {
        string CurrentBGM = PlayerPrefs.GetString("BGM", "Playwithme");
        AudioManager.Instance.PlayMusic(CurrentBGM);
    }
    public void OpenMusicSettingPopup()
    {
        MainMenu.SetActive(false);
        MusicSettingPopup.SetActive(true);
    }

    public void CloseMusicSettingPopup()
    {
        MainMenu.SetActive(true);
        MusicSettingPopup.SetActive(false);
    }
}
