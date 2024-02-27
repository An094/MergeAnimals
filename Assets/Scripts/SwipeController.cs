using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static SwipeController;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    public delegate void ChangeItem(int index);
    public ChangeItem OnChangeItem;
    [SerializeField] private int MaxPage;
    [SerializeField] Vector3 PageStep;
    [SerializeField] private RectTransform LevelPagesRect;
    [SerializeField] float TweenTime;
    [SerializeField] LeanTweenType TweenType;
    [SerializeField] Button PreviousBtn, NextBtn;

    [SerializeField] ShopManager ShopManager;
    private int CurrentPage;
    private Vector3 TargetPos;
    private float DragThreashold;

    private void Awake()
    {
        ShopManager.OnIntroCompleted += SwipeAnimation;
        CurrentPage = 1;
        TargetPos = LevelPagesRect.localPosition;
        UpdateArrowButton();
    }

    private void Start()
    {
        DragThreashold = Screen.width / 15;
        string avatar = PlayerPrefs.GetString("Avatar", "Cat1");
        int numberAvatarInList = Convert.ToInt32(avatar.Substring(3));
        TargetPos += PageStep * (numberAvatarInList - CurrentPage);
        CurrentPage = numberAvatarInList;
        OnChangeItem.Invoke(CurrentPage);
        //LevelPagesRect.LeanMoveLocal(TargetPos, TweenTime).setEase(TweenType);
        UpdateArrowButton();
    }

    private void SwipeAnimation()
    {
        LevelPagesRect.LeanMoveLocal(TargetPos, TweenTime).setEase(TweenType);
        UpdateArrowButton();
    }

    public void Next()
    {
        if(CurrentPage < MaxPage)
        {
            CurrentPage++;
            TargetPos += PageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if(CurrentPage > 1)
        {
            CurrentPage--;
            TargetPos -= PageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        OnChangeItem.Invoke(CurrentPage);
        LevelPagesRect.LeanMoveLocal(TargetPos, TweenTime).setEase(TweenType);
        UpdateArrowButton() ;
    }

    void UpdateArrowButton()
    {
        NextBtn.interactable = true;
        PreviousBtn.interactable = true;
        if(CurrentPage == 1)
        {
            PreviousBtn.interactable = false;
        }
        else if(CurrentPage == MaxPage)
        {
            NextBtn.interactable = false;
        }
    }

    public int GetCurrentItem()
    {
        return CurrentPage;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Math.Abs(eventData.position.x - eventData.pressPosition.x) > DragThreashold)
        {
            if (eventData.position.x > eventData.pressPosition.x)
            {
                Previous();
            }
            else
            {
                Next();
            }
        }
        else
        {
            MovePage();
        }
    }
}
