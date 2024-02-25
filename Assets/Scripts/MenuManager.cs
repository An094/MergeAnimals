using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject Popups;
    [SerializeField] private GameObject AudioSettingPopup;
    [SerializeField] private GameObject RecordPopup;
    [SerializeField] private RectTransform MusicSettingPopupPanel;
    [SerializeField] private RectTransform RecordPopupPanel;
    [SerializeField] private CanvasGroup CanvasGround;//dark background
    [SerializeField] private float duration = 1.0f;

    [SerializeField] private RectTransform MenuHeader;
    [SerializeField] private RectTransform MenuBody;
    private Vector2 DefaultStartPosition = new Vector2(0.0f, 2000f);
    private Vector2 DefaultEndPosition = new Vector2(0.0f, -2000f);

    private void Start()
    {
        {
            string CurrentBGM = PlayerPrefs.GetString("BGM", "Playwithme");
            AudioManager.Instance.PlayMusic(CurrentBGM);
        }
        
        MusicSettingPopupPanel.anchoredPosition = DefaultStartPosition;
        RecordPopupPanel.anchoredPosition = DefaultStartPosition;

        ///MenuAnimation
        MenuHeader.anchoredPosition = new Vector3(0f, 2000f, 0f);
        MenuBody.anchoredPosition = new Vector3(0f, -2000f, 0f);
        StartMenuIntro();
    }

    private void StartMenuIntro()
    {
        MenuHeader.DOAnchorPos(new Vector3(0f, 300f, 0f), 1.5f, false).SetEase(Ease.OutQuint);
        MenuBody.DOAnchorPos(new Vector3(0f, -100f, 0f), 1.5f, false).SetEase(Ease.OutQuint);
    }

    private async Task StartMenuOuttro()
    {
        MenuHeader.DOAnchorPos(new Vector3(0f, 2000f, 0f), 1f, false).SetEase(Ease.InQuint);
        await MenuBody.DOAnchorPos(new Vector3(0f, -2000f, 0f), 1f, false).SetEase(Ease.InQuint).AsyncWaitForCompletion();
    }

    public async void OnPlayClicked()
    {
        await StartMenuOuttro();
        SceneManager.LoadScene("Dragon");
    }

    public async void OnShopClicked()
    {
        await StartMenuOuttro();
        SceneManager.LoadScene("Shop");
    }

    public void OpenMusicSettingPopup()
    {
        MainMenu.SetActive(false);
        Popups.SetActive(true);
        AudioSettingPopup.SetActive(true);
        RecordPopup.SetActive(false);
        MusicSettingPopupIntro();
    }

    public async void CloseMusicSettingPopup()
    {
        await MusicSettingPopupOuttro();
        MainMenu.SetActive(true);
        MusicSettingPopupPanel.anchoredPosition = DefaultStartPosition;
        Popups.SetActive(false);
        AudioSettingPopup.SetActive(false);
        RecordPopup.SetActive(false);
    }

    private void MusicSettingPopupIntro()
    {
        CanvasGround.DOFade(1, duration).SetUpdate(true);
        //MusicSettingPopupPanel.DOAnchorPosY(0, duration).SetUpdate(true);
        MusicSettingPopupPanel.DOAnchorPosY(0, duration).SetEase(Ease.OutQuint);
    }

    private async Task MusicSettingPopupOuttro()
    {
        CanvasGround.DOFade(0, duration).SetUpdate(true);
        //await MusicSettingPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetUpdate(true).AsyncWaitForCompletion();
        await MusicSettingPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetEase(Ease.InOutQuint).AsyncWaitForCompletion();
    }

    public void OpenRecordPopup()
    {
        MainMenu.SetActive(false);
        Popups.SetActive(true);
        AudioSettingPopup.SetActive(false);
        RecordPopup.SetActive(true);
        RecordPopupIntro();
    }

    public async void CloseRecordPopup()
    {
        await RecordPopupOuttro();
        MainMenu.SetActive(true);
        RecordPopupPanel.anchoredPosition = DefaultStartPosition;
        Popups.SetActive(false);
        AudioSettingPopup.SetActive(false);
        RecordPopup.SetActive(false);
    }

    private void RecordPopupIntro()
    {
        CanvasGround.DOFade(1, duration).SetUpdate(true);
        //MusicSettingPopupPanel.DOAnchorPosY(0, duration).SetUpdate(true);
        RecordPopupPanel.DOAnchorPosY(0, duration).SetEase(Ease.OutQuint);
    }

    private async Task RecordPopupOuttro()
    {
        CanvasGround.DOFade(0, duration).SetUpdate(true);
        //await MusicSettingPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetUpdate(true).AsyncWaitForCompletion();
        await RecordPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetEase(Ease.InOutQuint).AsyncWaitForCompletion();
    }
}
