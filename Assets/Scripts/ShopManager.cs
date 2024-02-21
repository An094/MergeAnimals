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

    private void Awake()
    {
        swipeController.OnChangeItem += OnSwipeChangeItem;
    }
    // Start is called before the first frame update
    void Start()
    {
        //for debugging
        PlayerPrefs.SetInt("DB", 1000);
        NumberOfDB = PlayerPrefs.GetInt("DB", 1000);
        NumberOfDBallLabel.text = NumberOfDB.ToString();
        CurrentAvatar = PlayerPrefs.GetString("Avatar", "Cat1");
        CurrentItemIndexInSwipe = swipeController.GetCurrentItem();
        UseOrBuyBtn.SetActive(false);
        PriceWidget.SetActive(false);
        TickWidget.SetActive(false);
        //For default avatar
        PlayerPrefs.SetInt("Cat1", 1);

        UpdateUI(PosibleAction.None);
    }

    void OnSwipeChangeItem(int index)
    {
        PosibleAction action = PosibleAction.None;

        string avatarName = "Cat" + index.ToString();
        bool WasBought = PlayerPrefs.GetInt(avatarName, 0) != 0;
        CurrentAvatar = PlayerPrefs.GetString("Avatar", "Cat1");
        CurrentItemIndexInSwipe = index;
        if (WasBought && CurrentAvatar == avatarName)
        {
            action = PosibleAction.None;//This mean item have been bought and setted as default avatar
        }
        else if (WasBought && CurrentAvatar != avatarName)
        {
            action = PosibleAction.Use;
        }
        else
        {
            action = PosibleAction.Buy;
        }
        UpdateUI(action);
    }

    private void UpdateUI(PosibleAction InBehavior)
    {
        NumberOfDBallLabel.text = NumberOfDB.ToString();
        switch (InBehavior)
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
            UpdateUI(PosibleAction.Use);
        }

    }

    public void Use()
    {
        string avatarName = "Cat" + CurrentItemIndexInSwipe.ToString();
        PlayerPrefs.SetString("Avatar", avatarName);
        UpdateUI(PosibleAction.None);
    }

    public void CloseMenu()
    {
        ///Animation

        SceneManager.LoadScene("Menu");
    }

}
