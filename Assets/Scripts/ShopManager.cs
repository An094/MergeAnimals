using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Events;

public enum PosibleAction
{
    None,
    Use,
    Buy
}
public class ShopManager : MonoBehaviour
{
    [SerializeField] private int DefaultPrice = 100;
    [SerializeField] private SwipeController swipeController;
    [SerializeField] private TMPro.TMP_Text NumberOfDBallLabel;
    [SerializeField] private GameObject PriceWidget;
    [SerializeField] private GameObject UseOrBuyBtn;
    [SerializeField] private GameObject UseImage;
    [SerializeField] private GameObject BuyImage;
    [SerializeField] private GameObject TickWidget;

    [SerializeField] private RectTransform MenuMainRect;
    [SerializeField] private RectTransform ShopIconRect;
    private int NumberOfDB;
    private string CurrentAvatar;
    private int CurrentItemIndexInSwipe;

    private PosibleAction CurrentAction;

    public delegate void IntroCompleted();
    public IntroCompleted OnIntroCompleted;

    private void Awake()
    {
        swipeController.OnChangeItem += OnSwipeChangeItem;
    }
    // Start is called before the first frame update
    void Start()
    {
        NumberOfDB = PlayerPrefs.GetInt("DB", 0);
        NumberOfDBallLabel.text = NumberOfDB.ToString();
        CurrentAvatar = PlayerPrefs.GetString("Avatar", "Cat1");
        CurrentItemIndexInSwipe = swipeController.GetCurrentItem();
        UseOrBuyBtn.SetActive(false);
        PriceWidget.SetActive(false);
        TickWidget.SetActive(false);
        //For default avatar
        PlayerPrefs.SetInt("Cat1", 1);

        CurrentAction = PosibleAction.None;

        UpdateUI();

        Intro();
    }

    private void Intro()
    {
        MenuMainRect.anchoredPosition = new Vector2(-2000, 0f);
        ShopIconRect.transform.localScale = Vector3.zero;
        MenuMainRect.DOAnchorPos(new Vector2(0f, 0f), 1.5f).SetEase(Ease.OutQuint).OnComplete(() => ShopIconAnimation());
        
    }

    private void ShopIconAnimation()
    {
        ShopIconRect.transform.DOScale(1.2f, .5f).SetEase(Ease.OutBounce).OnComplete(() => OnIntroCompleted.Invoke());
    }

    void OnSwipeChangeItem(int index)
    {
        string avatarName = "Cat" + index.ToString();
        bool WasBought = PlayerPrefs.GetInt(avatarName, 0) != 0;
        CurrentAvatar = PlayerPrefs.GetString("Avatar", "Cat1");
        CurrentItemIndexInSwipe = index;
        if (WasBought && CurrentAvatar == avatarName)
        {
            CurrentAction = PosibleAction.None;//This mean item have been bought and setted as default avatar
        }
        else if (WasBought && CurrentAvatar != avatarName)
        {
            CurrentAction = PosibleAction.Use;
        }
        else
        {
            CurrentAction = PosibleAction.Buy;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        NumberOfDB = PlayerPrefs.GetInt("DB", 0);
        NumberOfDBallLabel.text = NumberOfDB.ToString();
        switch (CurrentAction)
        {
            case PosibleAction.Buy:
                {
                    UseOrBuyBtn.SetActive(true);
                    PriceWidget.SetActive(true);
                    BuyImage.SetActive(true);
                    UseImage.SetActive(false);
                    TickWidget.SetActive(false);
                    //TODO: Disable buy button when not enough DB

                    BuyImage.GetComponent<Button>().interactable = NumberOfDB >= 100;

                    break;
                }
            case PosibleAction.Use:
                {
                    UseOrBuyBtn.SetActive(true);
                    PriceWidget.SetActive(false);
                    BuyImage.SetActive(false);
                    UseImage.SetActive(true);
                    TickWidget.SetActive(false);
                    break;
                }
            case PosibleAction.None:
                {
                    UseOrBuyBtn.SetActive(false);
                    PriceWidget.SetActive(false);
                    TickWidget.SetActive(true);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    public void Buy()
    {
        NumberOfDB = PlayerPrefs.GetInt("DB", 0);
        if (NumberOfDB >= DefaultPrice)
        {
            NumberOfDB -= DefaultPrice;
            PlayerPrefs.SetInt("DB", NumberOfDB);
            string avatarName = "Cat" + CurrentItemIndexInSwipe.ToString();
            PlayerPrefs.SetInt(avatarName, 1);

            CurrentAction = PosibleAction.Use;
            UpdateUI();
        }

    }

    public void Use()
    {
        string avatarName = "Cat" + CurrentItemIndexInSwipe.ToString();
        PlayerPrefs.SetString("Avatar", avatarName);
        CurrentAction = PosibleAction.None;
        UpdateUI();
    }

    public void CloseMenu()
    {
        MenuMainRect.DOAnchorPos(new Vector2(2000, 0f), 1.5f).SetEase(Ease.InQuint).OnComplete(() => SceneManager.LoadScene("Menu"));
    }

    public void CheatDB()
    {
        NumberOfDB = PlayerPrefs.GetInt("DB", 0);
        PlayerPrefs.SetInt("DB", NumberOfDB + 100);
        UpdateUI();
    }

    public void DeleteProfile()
    {
        PlayerPrefs.DeleteKey("DB");
        PlayerPrefs.DeleteKey("Avatar");
        for (int i = 2; i <= 10; ++i)
        {
            PlayerPrefs.DeleteKey("Cat" + i);
        }
        UpdateUI();
    }
}
