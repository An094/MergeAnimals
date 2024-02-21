using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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
    private Vector2 ScreenBounds;
    private const float PADDING = 2f;
    public float TimeTillGameOver = 1.5f;

    private bool CanCombine = true;
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

        if (Application.isMobilePlatform)
        {
            //FruitChartForMobile.SetActive(true);
            //FruitChart.SetActive(false);
            TutorialText.SetActive(false);
        }
        else
        {
            //FruitChartForMobile.SetActive(false);
            //FruitChart.SetActive(true);
            TutorialText.SetActive(true);
        }
    }

    private void Start()
    {
        string avatar = PlayerPrefs.GetString("Avatar", "Cat1");
        int numberAvatarInList = Convert.ToInt32(avatar.Substring(3));
        playerSprite.sprite = avatarsprites[numberAvatarInList - 1];

        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        _bestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        StartCoroutine(SpawnClouds());
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
    }

    public void CombineObject(GameObject obj1, GameObject obj2, int index)
    {
        CanCombine = false;
        Destroy(obj1);
        Destroy(obj2);

        Vector3 combinedFruitPos = (obj1.transform.position + obj2.transform.position) / 2f + new Vector3(0f, 0.02f, 0f);
        GameObject appearEffect = ObjectPoolManager.SpawnObject(AppearEffect, combinedFruitPos, Quaternion.identity);

        StartCoroutine(SpawnNewObject(index, combinedFruitPos));
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

        CanCombine = true;
        //appearEffect.transform.localScale = go.transform.localScale;
    }

    private void FixedUpdate()
    {
        //while(mergeInfos.Count > 0)
        if(mergeInfos.Count > 0 && CanCombine)
        {
            MergeInfo info = mergeInfos[0];
            if (info != null)
            {
                CombineObject(info.firstObj, info.secondObj, info.currentIndex);
            }
            mergeInfos.RemoveAt(0);
        }
    }
}
