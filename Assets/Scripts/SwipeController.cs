using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SwipeController;

public class SwipeController : MonoBehaviour
{
    public delegate void ChangeItem(int index);
    public ChangeItem OnChangeItem;
    [SerializeField] private int MaxPage;
    [SerializeField] Vector3 PageStep;
    [SerializeField] private RectTransform LevelPagesRect;
    [SerializeField] float TweenTime;
    [SerializeField] LeanTweenType TweenType;
    [SerializeField] Button PreviousBtn, NextBtn;

    private int CurrentPage;
    private Vector3 TargetPos;

    private void Awake()
    {
        CurrentPage = 1;
        TargetPos = LevelPagesRect.localPosition;
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
}
