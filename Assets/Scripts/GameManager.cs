using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using CandyCoded.HapticFeedback;

public class MergeInfo
{
    public GameObject firstObj;
    public GameObject secondObj;
    public int currentIndex;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static List<MergeInfo> mergeInfos = new List<MergeInfo>();
    public int CurrentScore { get; set; }

    public delegate void FadeInCompleted();
    public FadeInCompleted OnFadeInCompleted;

    [SerializeField] private GameObject AppearEffect;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;
    [SerializeField] private Image _gameOverPanel;
    [SerializeField] private float _fadeTime = 2f;
    [SerializeField] private GameObject CloudPrefab;
    [SerializeField] private GameObject FruitChartForMobile;
    [SerializeField] private GameObject FruitChart;
    [SerializeField] private GameObject TutorialText;
    [SerializeField] private List<Sprite> avatarsprites;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private GameObject IncreaseWidget;
    [SerializeField] private RectTransform DestinationForIncreaseWidget;
    [SerializeField] private RectTransform DefaultPositionForIncreaseWidget;

    [SerializeField] private GameObject Popups;
    [SerializeField] private GameObject AudioSettingPopup;
    [SerializeField] private GameObject ExitConfirmPopup;
    [SerializeField] private GameObject ReplayConfirmPopup;
    [SerializeField] private RectTransform MusicSettingPopupPanel;
    [SerializeField] private RectTransform ExitConfirmPopupPanel;
    [SerializeField] private RectTransform ReplayConfirmPopupPanel;
    [SerializeField] private CanvasGroup CanvasGround;//dark background
    [SerializeField] private float duration = 1.0f;
    private Vector2 DefaultStartPosition = new Vector2(0.0f, 2000f);
    private Vector2 DefaultEndPosition = new Vector2(0.0f, -2000f);

    float elapsedTime;

    private Vector2 ScreenBounds;
    private const float PADDING = 2f;
    public float TimeTillGameOver = 1.5f;

    bool HasHorse = false;
    bool HasBuffalo = false;
    bool HasTiger = false;
    bool HasDragon = false;

    private int LastScoreTriggerIncreaseDB = 0;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += FadeGame;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= FadeGame;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _scoreText.text = CurrentScore.ToString("0");

        //if (Application.isMobilePlatform)
        //{
        //    //FruitChartForMobile.SetActive(true);
        //    //FruitChart.SetActive(false);
        //    TutorialText.SetActive(false);
        //}
        //else
        //{
        //    //FruitChartForMobile.SetActive(false);
        //    //FruitChart.SetActive(true);
        //    TutorialText.SetActive(true);
        //}
    }

    private void Start()
    {
        string avatar = PlayerPrefs.GetString("Avatar", "Cat1");
        int numberAvatarInList = Convert.ToInt32(avatar.Substring(3));
        playerSprite.sprite = avatarsprites[numberAvatarInList - 1];

        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        _bestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        StartCoroutine(SpawnClouds());

        MusicSettingPopupPanel.anchoredPosition = new Vector3(0f, 2000f, 0f);
        ExitConfirmPopupPanel.anchoredPosition = new Vector3(0f, 2000f, 0f);
        ReplayConfirmPopupPanel.anchoredPosition = new Vector3(0f, 2000f, 0f);
        
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            float interval = UnityEngine.Random.Range(8f, 10f);
            int side = UnityEngine.Random.Range(0, 2);
            float height = UnityEngine.Random.Range(-ScreenBounds.y + PADDING, ScreenBounds.y - PADDING);
            float speed = UnityEngine.Random.Range(0.25f, 0.5f);
            Vector2 position = new Vector2(side != 0 ? ScreenBounds.x + PADDING : -ScreenBounds.x - PADDING, height);

            GameObject cloudObj = ObjectPoolManager.SpawnObject(CloudPrefab, position, Quaternion.identity);
            Cloud cloud = cloudObj.GetComponent<Cloud>();
            cloud.IsMovingToRight = side == 0;
            cloud.Speed = speed;
            yield return new WaitForSeconds(interval);
        }
    }

    public void IncreaseScore(int amount)
    {
        CurrentScore += amount;
        if (CurrentScore > PlayerPrefs.GetInt("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", CurrentScore);
            _bestScoreText.text = CurrentScore.ToString();
        }

        int tmp = CurrentScore / 100;
        if (tmp > 0 && tmp > LastScoreTriggerIncreaseDB)
        {
            LastScoreTriggerIncreaseDB = tmp;

            int CurrentDB = PlayerPrefs.GetInt("DB", 0);
            PlayerPrefs.SetInt("DB", CurrentDB + 10);

            //Play animation
            IncreaseDBAnimation();
        }
        _scoreText.text = CurrentScore.ToString("0");
    }

    public void GameOver()
    {
        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame()
    {
        _gameOverPanel.gameObject.SetActive(true);

        Color startColor = _gameOverPanel.color;
        startColor.a = 0f;
        _gameOverPanel.color = startColor;

        float elapsedTime = 0f;
        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;

            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / _fadeTime));
            startColor.a = newAlpha;
            _gameOverPanel.color = startColor;

            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FadeGame(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeGameIn());
    }

    private IEnumerator FadeGameIn()
    {
        _gameOverPanel.gameObject.SetActive(true);
        Color startColor = _gameOverPanel.color;
        startColor.a = 1f;
        _gameOverPanel.color = startColor;

        float elapsedTime = 0f;
        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;

            float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / _fadeTime));
            startColor.a = newAlpha;
            _gameOverPanel.color = startColor;

            yield return null;
        }

        _gameOverPanel.gameObject.SetActive(false);
        AudioManager.Instance.PlaySFX("IWillDoMyBest");
        OnFadeInCompleted.Invoke();
    }

    public void CombineObject(GameObject obj1, GameObject obj2, int index)
    {
        Destroy(obj1);
        Destroy(obj2);

        Vector3 combinedFruitPos = (obj1.transform.position + obj2.transform.position) / 2f + new Vector3(0f, 0.02f, 0f);
        GameObject appearEffect = ObjectPoolManager.SpawnObject(AppearEffect, combinedFruitPos, Quaternion.identity);

        StartCoroutine(SpawnNewObject(index, combinedFruitPos));

        //Record
        string NameOfNextObject;

        switch (index)
        {
            case 6:
                {
                    NameOfNextObject = "Pig";
                    break;
                }
            case 7:
                {
                    NameOfNextObject = "Horse";
                    break;
                }
            case 8:
                {
                    NameOfNextObject = "Buffalo";
                    break;
                }
            case 9:
                {
                    NameOfNextObject = "Tiger";
                    break;
                }
            case 10:
                {
                    NameOfNextObject = "Dragon";
                    break;
                }
            default:
                {
                    NameOfNextObject = "";
                    break;
                }
        }

        int NumberInRecordOfObject = PlayerPrefs.GetInt(NameOfNextObject, 0);
        PlayerPrefs.SetInt(NameOfNextObject, NumberInRecordOfObject + 1);
    }

    private IEnumerator SpawnNewObject(int index, Vector3 spawnPosition)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameObject nextObj = FruitSelector.instance.Fruits[index + 1];
        GameObject go = Instantiate(nextObj, GameManager.instance.transform);
        go.transform.position = spawnPosition;

        ColliderInformer informer = go.GetComponent<ColliderInformer>();
        if (informer != null)
        {
            informer.WasCombinedIn = true;
        }
        //appearEffect.transform.localScale = go.transform.localScale;
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < 0.2f)
        {
            return;
        }

        if (mergeInfos.Count > 0)
        {
            MergeInfo info = mergeInfos[0];
            if (info != null)
            {
                SimpleCombineObjects(info.firstObj, info.secondObj, info.currentIndex);
            }
            mergeInfos.RemoveAt(0);
        }
    }

    private void SimpleCombineObjects(GameObject obj1, GameObject obj2, int index)
    {
        elapsedTime = 0.0f;

        Vector3 combinedFruitPos = (obj1.transform.position + obj2.transform.position) / 2f;

        GameObject nextObj = FruitSelector.instance.Fruits[index + 1];
        GameObject go = Instantiate(nextObj, combinedFruitPos, Quaternion.identity);

        ColliderInformer informer = go.GetComponent<ColliderInformer>();
        if (informer != null)
        {
            informer.WasCombinedIn = true;
        }

        GameObject appearEffect = ObjectPoolManager.SpawnObject(AppearEffect, combinedFruitPos, Quaternion.identity);

        Destroy(obj1);
        Destroy(obj2);

        if(Application.isMobilePlatform)
        {
            MediumVibration();
        }

        //Record
        string NameOfNextObject;

        switch (index)
        {
            case 6:
                {
                    NameOfNextObject = "Pig";
                    break;
                }
            case 7:
                {
                    NameOfNextObject = "Horse";
                    if(!HasHorse)
                    {
                        HasHorse = true;
                        AudioManager.Instance.PlaySFX("Congratulation");
                    }
                    break;
                }
            case 8:
                {
                    NameOfNextObject = "Buffalo";
                    if (!HasBuffalo)
                    {
                        HasBuffalo = true;
                        AudioManager.Instance.PlaySFX("Congratulation");
                    }
                    break;
                }
            case 9:
                {
                    NameOfNextObject = "Tiger";
                    if (!HasTiger)
                    {
                        HasTiger = true;
                        AudioManager.Instance.PlaySFX("Congratulation");
                    }
                    break;
                }
            case 10:
                {
                    NameOfNextObject = "Dragon";
                    if (!HasDragon)
                    {
                        HasDragon = true;
                        AudioManager.Instance.PlaySFX("Congratulation");
                    }
                    break;
                }
            default:
                {
                    NameOfNextObject = "";
                    break;
                }
        }

        int NumberInRecordOfObject = PlayerPrefs.GetInt(NameOfNextObject, 0);
        PlayerPrefs.SetInt(NameOfNextObject, NumberInRecordOfObject + 1);
    }

    void IncreaseDBAnimation()
    {
        IncreaseWidget.SetActive(true);
        RectTransform rectTransform = IncreaseWidget.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = DefaultPositionForIncreaseWidget.anchoredPosition;
        if (rectTransform != null)
        {
            rectTransform.DOAnchorPos(DestinationForIncreaseWidget.anchoredPosition, 2f).SetEase(Ease.InOutSine);
            //Fade to alpha=1 starting from alpha=0 immediately
            IncreaseWidget.GetComponent<CanvasGroup>().DOFade(0f, 2f).From(1f).OnComplete(() => ResetPosition());
        }
    }

    void ResetPosition()
    {
        RectTransform rectTransform = IncreaseWidget.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = DefaultPositionForIncreaseWidget.anchoredPosition;
        }
        IncreaseWidget.SetActive(false);
    }


    public void OpenMusicSettingPopup()
    {
        Popups.SetActive(true);
        AudioSettingPopup.SetActive(true);
        ExitConfirmPopup.SetActive(false);
        ReplayConfirmPopup.SetActive(false);
        MusicSettingPopupIntro();
    }

    public async void CloseMusicSettingPopup()
    {
        await MusicSettingPopupOuttro();
        MusicSettingPopupPanel.anchoredPosition = DefaultStartPosition;
        Popups.SetActive(false);
        AudioSettingPopup.SetActive(false);
        ExitConfirmPopup.SetActive(false);
        ReplayConfirmPopup.SetActive(false);
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

    public void OpenExitPopup()
    {
        Popups.SetActive(true);
        AudioSettingPopup.SetActive(false);
        ExitConfirmPopup.SetActive(true);
        ReplayConfirmPopup.SetActive(false);
        ExitConfirmPopupIntro();
    }

    public async void CloseExitPopup()
    {
        await ExitConfirmPopupOuttro();
        ExitConfirmPopupPanel.anchoredPosition = DefaultStartPosition;
        Popups.SetActive(false);
        AudioSettingPopup.SetActive(false);
        ExitConfirmPopup.SetActive(false);
        ReplayConfirmPopup.SetActive(false);
    }

    private void ExitConfirmPopupIntro()
    {
        CanvasGround.DOFade(1, duration).SetUpdate(true);
        //MusicSettingPopupPanel.DOAnchorPosY(0, duration).SetUpdate(true);
        ExitConfirmPopupPanel.DOAnchorPosY(0, duration).SetEase(Ease.OutQuint);
    }

    private async Task ExitConfirmPopupOuttro()
    {
        CanvasGround.DOFade(0, duration).SetUpdate(true);
        //await MusicSettingPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetUpdate(true).AsyncWaitForCompletion();
        await ExitConfirmPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetEase(Ease.InOutQuint).AsyncWaitForCompletion();
    }

    public void OpenReplayPopup()
    {
        Popups.SetActive(true);
        AudioSettingPopup.SetActive(false);
        ExitConfirmPopup.SetActive(false);
        ReplayConfirmPopup.SetActive(true);
        ReplayConfirmPopupIntro();
    }

    public async void CloseReplayPopup()
    {
        await ReplayConfirmPopupOuttro();
        ReplayConfirmPopupPanel.anchoredPosition = DefaultStartPosition;
        Popups.SetActive(false);
        AudioSettingPopup.SetActive(false);
        ExitConfirmPopup.SetActive(false);
        ReplayConfirmPopup.SetActive(false);
    }

    private void ReplayConfirmPopupIntro()
    {
        CanvasGround.DOFade(1, duration).SetUpdate(true);
        //MusicSettingPopupPanel.DOAnchorPosY(0, duration).SetUpdate(true);
        ReplayConfirmPopupPanel.DOAnchorPosY(0, duration).SetEase(Ease.OutQuint);
    }

    private async Task ReplayConfirmPopupOuttro()
    {
        CanvasGround.DOFade(0, duration).SetUpdate(true);
        //await MusicSettingPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetUpdate(true).AsyncWaitForCompletion();
        await ReplayConfirmPopupPanel.DOAnchorPosY(DefaultEndPosition.y, duration).SetEase(Ease.InOutQuint).AsyncWaitForCompletion();
    }

    public void OnExitClicked()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlaySFXWhenThrow()
    {
        string[] Names = { "Jump1", "Jump2", "Jump3" };

        int index = UnityEngine.Random.Range(0, Names.Length);

        AudioManager.Instance.PlaySFX(Names[index]);
    }

    public void LightVibration()
    {
        HapticFeedback.LightFeedback();
    }

    public void MediumVibration()
    {
        HapticFeedback.MediumFeedback();
    }

    public void HeavyVibration()
    {
        HapticFeedback.HeavyFeedback();
    }
}
