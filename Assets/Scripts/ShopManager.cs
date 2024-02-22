using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private int NumberOfDB;
    private string CurrentAvatar;
    private int CurrentItemIndexInSwipe;

    private PosibleAction CurrentAction;

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
        NumberOfDB = PlayerPrefs.GetInt("DB", 1000);
        if (NumberOfDB > DefaultPrice)
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
        ///Animation

        SceneManager.LoadScene("Menu");
    }

    public void CheatDB()
    {
        NumberOfDB = PlayerPrefs.GetInt("DB", 1000);
        PlayerPrefs.SetInt("DB", NumberOfDB + 1000);
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
